using Application;
using Application.Utils;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

internal class Entry
{
    public static async Task Main()
    {
        IServiceProvider provider = CreateServices();

        App application = provider.GetRequiredService<App>();

        await application.InitializeAsync();
        
        await Process.GetCurrentProcess().WaitForExitAsync();
    }


    public static IServiceProvider CreateServices()
    {
        ServiceCollection collection = new();

        Settings settings = new("settings.txt");

        collection.AddSingleton<Settings>(settings);
        collection.AddSingleton<DiscordSocketClient>();
        collection.AddSingleton<Logger>(new Logger([new ConsoleLoggerService()]));
        collection.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        collection.AddSingleton<App>();

        return collection.BuildServiceProvider();
    }
}