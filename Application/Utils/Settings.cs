using System.Collections.Concurrent;

using static Application.Utils.LoggingMessages.Error;
using static Application.Utils.LoggingMessages.Success;

namespace Application.Utils;

/// <summary>
/// Loads and reads application settings from text, files, or UTF-8 encoded data.
/// 
/// Format:
/// 
/// string: key = "{value}"
/// ulong: key = numeric
/// bool: key = true
/// float: key = float
/// double: key = double
/// </summary>
public class Settings
{
    /// <summary>
    /// Result produced when a single settings line is parsed.
    /// </summary>
    private enum LineParseResult
    {
        /// <summary>
        /// The line contains a key and a value.
        /// </summary>
        Success,

        /// <summary>
        /// The line is empty or contains a comment.
        /// </summary>
        Empty,

        /// <summary>
        /// The line contains a key but no value.
        /// </summary>
        EmptyValue,

        /// <summary>
        /// The line contains a value but no key.
        /// </summary>
        EmptyKey,

        /// <summary>
        /// The line does not match the expected key-value format.
        /// </summary>
        Invalid
    }

    /// <summary>
    /// Parsed settings keyed by their setting name.
    /// </summary>
    ConcurrentDictionary<string, object> entries = new();

    /// <summary>
    /// Logger used to report parsing warnings and errors.
    /// </summary>
    readonly Logger logger;

    /// <summary>
    /// Creates a settings reader and loads values from a file when it exists.
    /// </summary>
    /// <param name="logger">Logger used to report parsing results.</param>
    /// <param name="filePath">Path to the settings file to read.</param>
    public Settings(Logger logger, string filePath)
    {
        this.logger = logger;
        if (File.Exists(filePath))
        {
            string text = File.ReadAllText(filePath);
            entries = ReadValues(text);
        }
    }

    /// <summary>
    /// Creates an empty settings reader.
    /// </summary>
    /// <param name="logger">Logger used to report parsing results.</param>
    public Settings(Logger logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Creates a settings reader from UTF-8 encoded settings data.
    /// </summary>
    /// <param name="logger">Logger used to report parsing results.</param>
    /// <param name="data">UTF-8 encoded settings text.</param>
    public Settings(Logger logger, byte[] data)
    {
        this.logger = logger;
        string text = System.Text.Encoding.UTF8.GetString(data);
        entries = ReadValues(text);
    }

    /// <summary>
    /// Parses a settings text buffer into typed key-value entries.
    /// </summary>
    /// <param name="text">Settings text to parse.</param>
    /// <returns>The parsed settings entries.</returns>
    internal ConcurrentDictionary<string, object> ReadValues(string text)
    {
        var entries = new ConcurrentDictionary<string, object>();

        int lineNumber = 0;

        foreach (string line in SplitIntoLines(text))
        {
            LineParseResult result = TryParseLine(line, out var keyValue);

            if (!ShouldRegister(result, lineNumber, keyValue))
                continue;

            lineNumber++;
            entries[keyValue.Key] = ParseValue(keyValue.Value);
        }

        if (lineNumber == 0) logger.Log(NO_SETTINGS, LogLevel.Warning);
        else logger.Log(SETTING_READ_SUCCESS, LogLevel.Info);

        return entries;
    }

    // Helpers to make code a bit more readable
    private static string[] SplitIntoLines(string buffer) => buffer.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    // Internal Helper predicates
    /// <summary>
    /// Checks whether a setting value is wrapped in matching single or double quotes.
    /// </summary>
    /// <param name="str">Value text to inspect.</param>
    /// <returns><see langword="true"/> when the value is quoted; otherwise, <see langword="false"/>.</returns>
    private static bool IsQuotedString(string str) => (str.StartsWith('\"') && str.EndsWith('\"')) || (str.StartsWith('\'') && str.EndsWith('\''));

    /// <summary>
    /// Checks whether a settings line should be ignored before parsing.
    /// </summary>
    /// <param name="line">Line text to inspect.</param>
    /// <returns><see langword="true"/> when the line is empty, whitespace, or a comment.</returns>
    private static bool IsEmptyLine(string line) => line.Length == 0 || line.StartsWith('#') || line.StartsWith("//") || string.IsNullOrWhiteSpace(line);

    /// <summary>
    /// Logs a message and returns the supplied result value.
    /// </summary>
    /// <param name="message">Message text to write.</param>
    /// <param name="level">Severity level for the message.</param>
    /// <param name="result">Boolean value to return after logging.</param>
    /// <returns>The supplied <paramref name="result"/> value.</returns>
    private bool LogAndReturn(string message, LogLevel level, bool result)
    {
        logger.Log(message, level);
        return result;
    }

    /// <summary>
    /// Parses a single settings line into a key and raw value.
    /// </summary>
    /// <param name="line">Line text to parse.</param>
    /// <param name="keyValue">Parsed key and raw value when the line contains them.</param>
    /// <returns>The parse result for the line.</returns>
    private static LineParseResult TryParseLine(string line, out (string Key, string Value) keyValue)
    {
        keyValue = (string.Empty, string.Empty);
        line = line.Trim();

        if (IsEmptyLine(line))
            return LineParseResult.Empty;

        int separator = line.IndexOf('=');

        if (separator < 0)
            return LineParseResult.Invalid;

        string key = line[..separator].Trim();
        string value = line[(separator + 1)..].Trim();

        if (key.Length == 0)
            return LineParseResult.EmptyKey;

        keyValue = (key, value);

        return (value.Length == 0)
            ? LineParseResult.EmptyValue
            : LineParseResult.Success;
    }

    /// <summary>
    /// Determines whether a parsed line should be registered as a setting.
    /// </summary>
    /// <param name="result">Parse result for the current line.</param>
    /// <param name="lineNumber">Number of settings lines registered before this line.</param>
    /// <param name="keyValue">Parsed key and raw value for the current line.</param>
    /// <returns><see langword="true"/> when the line should be stored as a setting.</returns>
    private bool ShouldRegister(LineParseResult result, int lineNumber, (string Key, string Value) keyValue) => result switch
    {
        LineParseResult.Success => true,
        LineParseResult.EmptyValue => LogAndReturn(
            string.Format(EMPTY_VALUE, keyValue.Key),
            LogLevel.Warning,
            true),
        LineParseResult.EmptyKey => LogAndReturn(
            string.Format(EMPTY_KEY, lineNumber),
            LogLevel.Error,
            false),
        LineParseResult.Invalid => LogAndReturn(
            string.Format(INVALID_LINE, lineNumber),
            LogLevel.Error,
            false),
        _ => false
    };

    /// <summary>
    /// Converts a raw setting value into the closest supported runtime type.
    /// </summary>
    /// <param name="value">Raw setting value text.</param>
    /// <returns>A string, boolean, unsigned integer, float, or double value.</returns>
    private static object ParseValue(string value)
    {
        if (IsQuotedString(value))
            return value[1..^1]; // remove quotes

        if (bool.TryParse(value, out bool b))
            return b;

        if (ulong.TryParse(value, out ulong ul))
            return ul;

        if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float f))
            return f;

        if (double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double d))
            return d;

        return value;
    }

    /// <summary>
    /// Gets a typed setting value or returns a fallback when the key is missing or the type does not match.
    /// </summary>
    /// <typeparam name="T">Expected setting value type.</typeparam>
    /// <param name="key">Setting key to read.</param>
    /// <param name="fallback">Value to return when the setting is missing or has a different type.</param>
    /// <returns>The stored setting value when it exists with type <typeparamref name="T"/>; otherwise, <paramref name="fallback"/>.</returns>
    public T Get<T>(string key, T fallback)
    {
        if (!entries.TryGetValue(key, out var value))
            return fallback;

        if (value.GetType() != typeof(T))
            return fallback;

        return (T)value;
    }
}
