
#if DEBUG
using Application.Utils;
using Discord.Interactions;

namespace Application.CommandModules;
public class CmdDebugModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "used to check availability")]
    public async Task Ping() => await RespondAsync("Pong!");
}

#endif