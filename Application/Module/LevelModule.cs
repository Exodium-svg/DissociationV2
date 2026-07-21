using Application.Module.DiscordRequests;
using Application.Utils;

namespace Application.Module;

/// <summary>
/// 
/// </summary>
/// <param name="logger"></param>
/// <param name="discordRequestModule"></param>
public class LevelModule(Logger logger, DiscordRequestModule discordRequestModule)
{
    /// <summary>
    /// 
    /// </summary>
    private readonly Logger logger = logger;
    
    /// <summary>
    /// 
    /// </summary>
    private readonly DiscordRequestModule discordRequestModule = discordRequestModule; 
}