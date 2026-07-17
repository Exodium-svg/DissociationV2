using Application.Db;
using Application.Db.Models;
using Application.Utils;
using Discord;

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
        throw new NotImplementedException("Not Implemented");
    }
    public Task<Channel[]> CollectChannels() {
        throw new NotImplementedException("Not Implemented");
    }

    public Task<GuildMember[]> CollectGuildMember()
    {
        throw new NotImplementedException("Not Implemented"); 
    }

    public Task<StarboardEntry[]> CollectStarboardEntries()
    {
        throw new NotImplementedException("Not Implemented"); 
    }

}