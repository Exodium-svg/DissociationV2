using Application.Utils;

namespace Application.Tests;

/// <summary>
/// In-memory logger used by tests to inspect log entries without writing to the console.
/// </summary>
internal sealed class TestLoggerService : ILoggerService
{
    /// <summary>
    /// Gets the messages recorded by the test logger.
    /// </summary>
    public List<(string Message, LogLevel Level)> Entries { get; } = [];

    /// <summary>
    /// Records a log entry in memory.
    /// </summary>
    /// <param name="message">Message text to record.</param>
    /// <param name="level">Severity level associated with the message.</param>
    public void Log(string message, LogLevel level = LogLevel.Info)
    {
        Entries.Add((message, level));
    }
}
