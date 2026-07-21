using Application.Module.DiscordRequests;
using Application.Utils;

namespace Application.Module.ModerationModule;

/// <summary>
/// 
/// </summary>
/// <param name="logger"></param>
/// <param name="discordRequestModule"></param>
public class ModerationModule(Logger logger, DiscordRequestModule discordRequestModule)
{
    /// <summary>
    /// 
    /// </summary>
    private Logger logger = logger; 

    /// <summary>
    /// 
    /// </summary>
    private DiscordRequestModule discordRequestModule = discordRequestModule;   
};