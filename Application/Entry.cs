using Application;
using Application.Db;
using Application.Module;
using Application.Module.DiscordRequests;
using Application.Utils;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

/// <summary>
/// Application startup entry point and service registration root.
/// </summary>
internal class Entry
{
    /// <summary>
    /// Default settings file loaded when the service provider is created.
    /// </summary>
    private const string DEFAULT_SETTINGS_FILE = "settings.txt";

    /// <summary>
    /// Builds application services, prepares the database, initializes the Discord application, and waits for shutdown.
    /// </summary>
    /// <returns>A task that completes when the process exits.</returns>
    public static async Task Main()
    {
        IServiceProvider provider = CreateServices();

        var databaseContext = provider.GetRequiredService<DatabaseContext>();

        databaseContext.Database.EnsureCreated();

        var application = provider.GetRequiredService<App>();

        await application.InitializeAsync();
        
        await Process.GetCurrentProcess().WaitForExitAsync();
    }

    /// <summary>
    /// Registers application services and builds the dependency injection provider.
    /// </summary>
    /// <returns>The configured service provider.</returns>
    public static IServiceProvider CreateServices()
    {
        ServiceCollection collection = new();

        collection.AddSingleton(new Logger([new ConsoleLoggerService()]));
        collection.AddSingleton(x => new Settings(x.GetRequiredService<Logger>(), DEFAULT_SETTINGS_FILE));
        collection.AddSingleton<DatabaseContext>();
        collection.AddSingleton<DiscordSocketClient>();
        collection.AddSingleton<DiscordRequestService>();
        collection.AddSingleton<StarboardModule>();
        collection.AddSingleton<DataModule>();
        collection.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        collection.AddSingleton<App>();

        return collection.BuildServiceProvider();
    }
}
