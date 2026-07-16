using Application.Utils;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace Application
{
    public class App(DiscordSocketClient client, InteractionService interactions, Settings settings, IServiceProvider services, Logger logger)
    {
        bool commandsRegistered = false;

        public async Task InitializeAsync()
        {
            string token = settings.Get("discord.token", string.Empty);

            if(string.Empty == token)
            {
                logger.Log($"Not token set, please add discord.token to your settings file as string!");
                Console.ReadLine();
                Environment.Exit(0);
            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await interactions.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            client.InteractionCreated += InteractionCreated;
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
                await interactions.RegisterCommandsToGuildAsync(guildSnowflake, true);
#else
                await interactions.RegisterCommandsGloballyAsync();
#endif
                commandsRegistered = true;
            }
            string username = client.CurrentUser.GlobalName;

            logger.Log($"Succesfully registered on: {username}", LogLevel.Info);
            // Other shit?
        }

        private Task InteractionCreated(SocketInteraction interaction)
        {
            throw new NotImplementedException();
        }

        private Task OnLogEvent(LogMessage logInfo)
        {
            switch(logInfo.Severity)
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
}
