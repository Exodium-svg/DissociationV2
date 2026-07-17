using System.Collections.Concurrent;

namespace Application.Utils;

/// <summary>
/// Settings file is used for our environment variables.
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
    private const string SETTING_READ_SUCCESS = "Succesfully Grabed all settings!";
    private const string SETTING_READ_FAILURE = "Failed to Grab any settings? Is this intentional?";
    private const string READ_EMPTY_VALUE = "Setting '{0}' has an empty value.";
    private const string READ_EMPTY_KEY = "Skipped line {0}: the setting key is empty.";
    private const string READ_INVALID_LINE = "Skipped line {0}: the setting is not valid.";
    private enum LineParseResult
    {
        Success,
        Empty,
        EmptyValue,
        EmptyKey,
        Invalid
    }
    ConcurrentDictionary<string, object> entries = new();
    readonly Logger logger;
    public Settings(Logger logger, string filePath)
    {
        this.logger = logger;
        if (File.Exists(filePath))
        {
            string text = File.ReadAllText(filePath);
            entries = ReadValues(text);
        }
    }
    public Settings(Logger logger)
    {
        this.logger = logger;
    }
    public Settings(Logger logger, byte[] data)
    {
        this.logger = logger;
        string text = System.Text.Encoding.UTF8.GetString(data);
        entries = ReadValues(text);
    }

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

        if (lineNumber == 0) logger.Log(SETTING_READ_FAILURE, LogLevel.Warning);
        else logger.Log(SETTING_READ_SUCCESS, LogLevel.Info);

        return entries;
    }

    // Helpers to make code a bit more readable
    private static string[] SplitIntoLines(string buffer) => buffer.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    // Internal Helper predicates
    private static bool IsQuotedString(string str) => (str.StartsWith('\"') && str.EndsWith('\"')) || (str.StartsWith('\'') && str.EndsWith('\''));

    private static bool IsEmptyLine(string line) => line.Length == 0 || line.StartsWith('#') || line.StartsWith("//") || string.IsNullOrWhiteSpace(line);

    private bool LogAndReturn(string message, LogLevel level, bool result)
    {
        logger.Log(message, level);
        return result;
    }

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

    private bool ShouldRegister(LineParseResult result, int lineNumber, (string Key, string Value) keyValue) => result switch
    {
        LineParseResult.Success => true,
        LineParseResult.EmptyValue => LogAndReturn(
            string.Format(READ_EMPTY_VALUE, keyValue.Key),
            LogLevel.Warning,
            true),
        LineParseResult.EmptyKey => LogAndReturn(
            string.Format(READ_EMPTY_KEY, lineNumber),
            LogLevel.Error,
            false),
        LineParseResult.Invalid => LogAndReturn(
            string.Format(READ_INVALID_LINE, lineNumber),
            LogLevel.Error,
            false),
        _ => false
    };

    private static object ParseValue(string value)
    {
        // Simplifed it to a simple boolean expression on Shared
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

    public T Get<T>(string key, T fallback)
    {
        if (!entries.TryGetValue(key, out var value))
            return fallback;

        if (value.GetType() != typeof(T))
            return fallback;

        return (T)value;
    }
}
