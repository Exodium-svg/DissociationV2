using Application.Module;
using Application.Utils;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

/// <summary>
/// Root of our application, here we initialize everything via ServiceProvider.
/// </summary>
public class App(DiscordSocketClient client, InteractionService interactions, Settings settings, IServiceProvider services, Logger logger, StarboardModule starboardModule)
{
    bool commandsRegistered = false;
    private const string COMMAND_REGISTER_SUCCESS = "Succesfully registered on!";
    private const string COMMAND_REGISTER_FAILED = "Failed to register commands!";
    private const string NO_TOKEN_MSG_CRITICAL = "Not token set, please add discord.token to your settings file as string!";
    private const string NO_GUILD_ID_CRITICAL = "Debug guild ID has not been set on guild.test_id, are we supposed to be a debug version?";

    public async Task InitializeAsync()
    {
        string token = settings.Get("discord.token", string.Empty);

        if (string.Empty == token)
        {
            logger.Log(NO_TOKEN_MSG_CRITICAL, LogLevel.Critical);
            Console.ReadLine();
            Environment.Exit(0);
        }

        await client.LoginAsync(TokenType.Bot, token);
        await interactions.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        await client.StartAsync();


        client.Log += OnLogEvent;
        client.Ready += OnReady;
    }

    private async Task OnReady()
    {
        // We might be calling this function again upon reconnecting.
        if (false == commandsRegistered)
        {
#if DEBUG
            Snowflake guildSnowflake = settings.Get<ulong>("guild.test_id", 0);

            if (0 == guildSnowflake)
            {
                logger.Log(NO_GUILD_ID_CRITICAL, LogLevel.Critical);
                Console.ReadLine();
                Environment.Exit(0);
            }


            try
            {
                await interactions.RegisterCommandsToGuildAsync(guildSnowflake, true);

                foreach (var module in interactions.Modules)
                    logger.Log($"{module.Name} found");
            }
            catch (Exception ex)
            {
                logger.Log($"{COMMAND_REGISTER_FAILED}: {ex}", LogLevel.Error);
            }
#else
            // Note updating global commands takes up to an hour!
            await interactions.RegisterCommandsGloballyAsync();
#endif
            commandsRegistered = true;
        }
        string username = client.CurrentUser.Username;

        logger.Log($"{COMMAND_REGISTER_SUCCESS}: {username}", LogLevel.Info);


        // For some reason it has to be a lambda, otherwise it simply doesn't work.
        client.InteractionCreated += async interaction =>
        {
            var scope = services.CreateScope();
            var ctx = new SocketInteractionContext(client, interaction);
            await interactions.ExecuteCommandAsync(ctx, scope.ServiceProvider);
        };

        client.ReactionAdded += OnReactionAdded;
    }

    private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
    {
        IMessageChannel channel = await cachedChannel.GetOrDownloadAsync();

        if (channel is not IGuildChannel guildChannel)
            return; // we may ignore

        IUserMessage message = await cachedMessage.GetOrDownloadAsync();

        bool shouldPost = await starboardModule.UpdateAndCheckPost(message, reaction);

        if (shouldPost)
            await starboardModule.AddToBoard(guildChannel.Guild, message);
    }

    private Task OnLogEvent(LogMessage logInfo)
    {
        switch (logInfo.Severity)
        {
            case LogSeverity.Info:
                logger.Log(logInfo.Message, LogLevel.Info);
                break;
            case LogSeverity.Error:
                logger.Log(logInfo.Message, LogLevel.Error);
                break;
            case LogSeverity.Warning:
                logger.Log(logInfo.Message, LogLevel.Warning);
                break;
            case LogSeverity.Debug:
                logger.Log(logInfo.Message, LogLevel.Debug);
                break;
            case LogSeverity.Verbose:
                // Not needed.
                break;
            case LogSeverity.Critical:
                logger.Log(logInfo.Exception.Message, LogLevel.Critical);
                break;
        }


        return Task.CompletedTask;
    }
}

