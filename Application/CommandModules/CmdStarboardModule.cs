using Application.Module;
using Discord;
using Discord.Interactions;

namespace Application.CommandModules;
public class CmdStarboardModule(StarboardModule starboard) : InteractionModuleBase<SocketInteractionContext>
{

    [CommandContextType([InteractionContextType.Guild])]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    [SlashCommand("set-reaction-count", "Used to set the required amount of reactions before a message is posted.")]
    public async Task SetRequiredReactions(int count)
    {
        // maybe we need something other than settings, as we need to be able to save values aswell?
        starboard.RequiredReactions = count;
    }

    [CommandContextType([InteractionContextType.Guild])]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    [SlashCommand("set-reaction-count-specific", "Used to set the required amount of a specific emote")]
    public async Task SetRequiredSpecificReaction(int? count, IEmote? emote)
    {
        if(null != emote)
            starboard.SpecificReaction = emote.Name;

        if (null != count)
            starboard.RequiredSpecificReactions = count.Value;
    }

    [CommandContextType([InteractionContextType.Guild])]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    [SlashCommand("add-to-board", "Command used to add a message to the board.")]
    public async Task PostToBoard(IUserMessage message)
    {
        var channel = message.Channel as IGuildChannel;

        if(null == channel)
        {
            await RespondAsync("message is not in a guild channel?", ephemeral: true);
            return;
        }

        await starboard.AddToBoard(channel.Guild, message);
    }
}

