using Application;
using Application.Db;
using Application.Module;
using Application.Utils;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

/// <summary>
/// Entrypoint of our application, note services are required to be registered in CreateServices for them to work.
/// </summary>
internal class Entry
{
    // To avoid Magic numbers.. oOOHHooo 
    private const string DEFAULT_SETTINGS_FILE = "settings.txt";
    public static async Task Main()
    {
        IServiceProvider provider = CreateServices();

        var databaseContext = provider.GetRequiredService<DatabaseContext>();

        databaseContext.Database.EnsureCreated();

        var application = provider.GetRequiredService<App>();

        await application.InitializeAsync();
        
        await Process.GetCurrentProcess().WaitForExitAsync();
    }

    public static IServiceProvider CreateServices()
    {
        ServiceCollection collection = new();

        collection.AddSingleton(new Logger([new ConsoleLoggerService()]));
        collection.AddSingleton(x => new Settings(x.GetRequiredService<Logger>(), DEFAULT_SETTINGS_FILE));
        collection.AddSingleton<DatabaseContext>();
        collection.AddSingleton<DiscordSocketClient>();
        collection.AddSingleton<StarboardModule>();
        collection.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        collection.AddSingleton<App>();

        return collection.BuildServiceProvider();
    }
}