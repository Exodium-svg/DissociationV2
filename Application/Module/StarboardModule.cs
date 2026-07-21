using Application.Db;
using Application.Db.Models;
using Application.Utils;
using Discord;
using static Application.Utils.LoggingMessages.Error;

namespace Application.Module;
public class StarboardModule
{
    public int RequiredReactions { get; set; }
    public int RequiredSpecificReactions { get; set; }
    public Snowflake BoardFlake { get; set; }
    public string SpecificReaction { get; set; }
    readonly Logger logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="logger"></param>
    public StarboardModule(Settings settings, Logger logger)
    {
        this.logger = logger;
        RequiredReactions = settings.Get<int>("starboard.reactions.required", 1);
        RequiredReactions = settings.Get<int>("starboard.reactions.required_specific", 1);
        SpecificReaction = settings.Get<string>("starboard.reactions.id", "\uD83D\uDD25");

        BoardFlake = settings.Get<ulong>("starboard.channel.flake", 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="reaction"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAndCheckPost(IUserMessage message, IReaction reaction)
    {

        using DatabaseContext context = new();

        Snowflake messageFlake = message.Id;

        var reactions = message.Reactions;

        List<Snowflake> uniqueUsers = new();

        await foreach (IUser user in message.GetReactionUsersAsync(reaction.Emote, RequiredReactions).Flatten())
        {
            if (user.IsBot || user.IsWebhook || uniqueUsers.Contains(user.Id))
                continue;

            uniqueUsers.Add(user.Id);
        }

        var entry = context.StarboardEntries.Where((entry) => entry.Flake == messageFlake).FirstOrDefault();

        bool shouldPost = uniqueUsers.Count > RequiredReactions && false == entry?.Posted;

        if(null == entry)
        {
            int specificReactions = 0;
            if (reaction.Emote.Name == SpecificReaction)
                specificReactions++;

            entry = new StarboardEntry(messageFlake, 1, specificReactions, false);

            context.StarboardEntries.Add(entry);
        }
        else
            context.StarboardEntries.Update(entry);
        
        await context.SaveChangesAsync();

        return shouldPost;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task AddToBoard(IGuild guild, IUserMessage message)
    {
        if (await guild.GetChannelAsync(BoardFlake) is not ITextChannel starChannel)
        {
            logger.Log(NO_STAR_CHANNEL, LogLevel.Warning);
            var ownerUser = await guild.GetOwnerAsync();

            var dmChannel = await ownerUser.CreateDMChannelAsync();

            await dmChannel.SendMessageAsync(BAD_STAR_CHANNEL);
            return;
        }

        var user = message.Author;

        // TODO: support for files such as images and ensure a channel is not NSFW maybe(?)
        EmbedBuilder builder = new EmbedBuilder();
        builder
            .WithAuthor($"{user.GlobalName}", user.GetAvatarUrl())
            .WithDescription(message.Content + $"\n\n[Original Message]({message.GetJumpUrl()})")
            .WithTimestamp(message.CreatedAt);

        using DatabaseContext context = new();

        var entry = context.StarboardEntries.Where((entry) => entry.Flake == message.Id).FirstOrDefault();

        if (null == entry)
        {
            entry = new(message.Id, 0, 0, true);

            context.StarboardEntries.Add(entry);
            return;
        }
        else
        {
            entry.Posted = true;
            context.StarboardEntries.Update(entry);
        }

        var dbSaveTask = context.SaveChangesAsync();
        var messageTask = starChannel.SendMessageAsync(embed: builder.Build());

        Task.WaitAll([messageTask, dbSaveTask]);
    }
}

