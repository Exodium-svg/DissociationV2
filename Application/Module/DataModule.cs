using Application.Db;
using Application.Db.Models;
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
public class DataModule
{
    /// <summary>
    /// 
    /// We would collect what we need at setup time.. 
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    DataModule()
    {
        throw new NotImplementedException(NOT_DONE);
    }
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
