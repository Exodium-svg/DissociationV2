namespace Application.Module;

using Application.Module.DiscordRequests;
using Application.Utils;
using static Application.Utils.LoggingMessages.Error;

// Stub for a way to setup the Application without a txt file in the future. 

/// <summary>
/// 
/// </summary>
public class SetupModule(Logger logger, DiscordRequestModule discordRequestModule)
{
    private readonly DiscordRequestModule discordRequestModule = discordRequestModule; 
    private readonly Logger logger = logger;  

    /// <summary>
    /// 
    /// </summary>
    internal class SetupWizard(Logger logger)
    {
        private readonly Logger logger = logger;         
    }
};
