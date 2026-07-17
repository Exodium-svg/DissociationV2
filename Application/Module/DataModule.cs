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

public class DataModule
{
    // We would collect what we need at setup time.. 
    DataModule()
    {
        throw new NotImplementedException(NOT_DONE);
    }
    public Task<Channel[]> CollectChannels() {
        throw new NotImplementedException(NOT_DONE);
    }

    public Task<GuildMember[]> CollectGuildMember()
    {
        throw new NotImplementedException(NOT_DONE); 
    }

    public Task<StarboardEntry[]> CollectStarboardEntries()
    {
        throw new NotImplementedException(NOT_DONE); 
    }

}
