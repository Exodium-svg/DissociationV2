namespace Application.Utils;

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Critical,
    PeformanceMetric,
}

public interface ILoggerService
{
    public void Log(string message, LogLevel level = LogLevel.Info);
}

public class ConsoleLoggerService : ILoggerService
{
    public void Log(string message, LogLevel level = LogLevel.Info)
    {
        ConsoleColor oldColor = Console.ForegroundColor;

        Console.ForegroundColor = GetLoggerLevelColor(level);
        Console.Write($"[{level}]");

        Console.ForegroundColor = oldColor;
        Console.WriteLine($" {message}");
    }

    // Essentually Set colors here. .
    private static ConsoleColor GetLoggerLevelColor(LogLevel level) => level switch
    {
        LogLevel.Debug => ConsoleColor.Gray,
        LogLevel.Info => ConsoleColor.Green,
        LogLevel.Warning => ConsoleColor.Yellow,
        LogLevel.Error => ConsoleColor.Red,
        LogLevel.Critical => ConsoleColor.Magenta,
        LogLevel.PeformanceMetric => ConsoleColor.Cyan,
        _ => ConsoleColor.White
    };
}

/// <summary>
/// Logger class, we're able to write Logger services which can respond their own way to Log event types. This way we may later on add different methods for logging.
/// </summary>
public class Logger(IEnumerable<ILoggerService> services)
{
    List<ILoggerService> _logServices = [.. services];

    public void Log(string message, LogLevel level = LogLevel.Info)
    {
        foreach (var service in _logServices)
            service.Log(message, level);
    }
}