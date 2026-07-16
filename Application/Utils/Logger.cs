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
        Console.WriteLine($"[{level}] {message}");
    }
}

/// <summary>
/// Logger class, we're able to write Logger services which can respond their own way to Log event types. This way we may later on add different methods for logging.
/// </summary>
public class Logger
{
    List<ILoggerService> _logServices;

    public Logger(IEnumerable<ILoggerService> services) => _logServices = new List<ILoggerService>(services);

    public void Log(string message, LogLevel level = LogLevel.Info)
    {
        foreach (var service in _logServices)
            service.Log(message, level);
    }
}