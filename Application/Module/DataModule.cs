using Application.Db;
using Application.Db.Models;
using Application.Module.DiscordRequests;
using Application.Utils;
using Discord;
using static Application.Utils.LoggingMessages.Error;

namespace Application.Module;

// Collects data for use in the application
// Channels 
// Users
// Starboard Entires
// Moderator Actions 

/// <summary>
/// 
/// </summary>
/// <param name="logger"></param>
/// <param name="discordService"></param>
public class DataModule (Logger logger, DiscordRequestModule discordService)
{
    
    /// <summary>
    /// 
    /// </summary>
    private readonly Logger logger = logger; 

    /// <summary>
    /// 
    /// </summary>
    private readonly DiscordRequestModule discordService = discordService; 

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<Channel[]> CollectChannels() {
        throw new NotImplementedException(NOT_DONE);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<GuildMember[]> CollectGuildMember()
    {
        throw new NotImplementedException(NOT_DONE); 
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<StarboardEntry[]> CollectStarboardEntries()
    {
        throw new NotImplementedException(NOT_DONE); 
    }

}
