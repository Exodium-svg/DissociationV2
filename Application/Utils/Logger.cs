namespace Application.Utils;

/// <summary>
/// Severity labels used by logger services when writing application messages.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Diagnostic information that is useful while debugging.
    /// </summary>
    Debug,

    /// <summary>
    /// General application progress or state information.
    /// </summary>
    Info,

    /// <summary>
    /// Recoverable issue that should be visible during normal operation.
    /// </summary>
    Warning,

    /// <summary>
    /// Failure that prevents the current operation from completing.
    /// </summary>
    Error,

    /// <summary>
    /// Severe failure that may require the application to stop or restart.
    /// </summary>
    Critical,

    /// <summary>
    /// Timing or throughput information used to measure application behavior.
    /// </summary>
    PeformanceMetric,
}

/// <summary>
/// Receives log messages from the application.
/// </summary>
public interface ILoggerService
{
    /// <summary>
    /// Writes a message at the supplied severity level.
    /// </summary>
    /// <param name="message">Message text to write.</param>
    /// <param name="level">Severity level for the message.</param>
    public void Log(string message, LogLevel level = LogLevel.Info);
}

/// <summary>
/// Logger service that writes color-coded messages to the console.
/// </summary>
public class ConsoleLoggerService : ILoggerService
{
    /// <summary>
    /// Writes a message to the console with a colored level prefix.
    /// </summary>
    /// <param name="message">Message text to write.</param>
    /// <param name="level">Severity level used for the console prefix.</param>
    public void Log(string message, LogLevel level = LogLevel.Info)
    {
        ConsoleColor oldColor = Console.ForegroundColor;

        Console.ForegroundColor = GetLoggerLevelColor(level);
        Console.Write($"[{level}]");

        Console.ForegroundColor = oldColor;
        Console.WriteLine($" {message}");
    }

    /// <summary>
    /// Gets the console color associated with a log level.
    /// </summary>
    /// <param name="level">Severity level to colorize.</param>
    /// <returns>The console color used for the level prefix.</returns>
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
/// Dispatches log messages to one or more logger services.
/// </summary>
public class Logger(IEnumerable<ILoggerService> services)
{
    /// <summary>
    /// Logger services that receive each message.
    /// </summary>
    List<ILoggerService> _logServices = [.. services];

    /// <summary>
    /// Sends a message to every registered logger service.
    /// </summary>
    /// <param name="message">Message text to write.</param>
    /// <param name="level">Severity level for the message.</param>
    public void Log(string message, LogLevel level = LogLevel.Info)
    {
        foreach (var service in _logServices)
            service.Log(message, level);
    }
}
