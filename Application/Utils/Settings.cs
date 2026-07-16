using System.Collections.Concurrent;

namespace Application.Utils;

public class Settings
{
    ConcurrentDictionary<string, object> entries = new();
    public Settings(string filePath)
    {
        if (File.Exists(filePath))
        {
            string text = File.ReadAllText(filePath);
            ReadValues(text);
        }
    }
    public Settings() { }
    public Settings(byte[] data)
    {
        string text = System.Text.Encoding.UTF8.GetString(data);
        ReadValues(text);
    }

    internal void ReadValues(string text)
    {
        var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            // skip comments and empty lines
            if (line.Length == 0 || line.StartsWith("#") || line.StartsWith("//"))
                continue;

            int eqIndex = line.IndexOf('=');
            if (eqIndex == -1)
                continue; // invalid line, skip

            string key = line[..eqIndex].Trim();
            string rawValue = line[(eqIndex + 1)..].Trim();

            if (string.IsNullOrEmpty(key))
                continue;

            object value = ParseValue(rawValue);
            entries[key] = value;
        }
    }

    private static object ParseValue(string value)
    {
        // quoted string (supports both " and ')
        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
            (value.StartsWith("'") && value.EndsWith("'")))
        {
            return value[1..^1]; // remove quotes
        }

        if (bool.TryParse(value, out bool b))
            return b;

        //if (int.TryParse(value, out int i))
        //    return i;

        //if (long.TryParse(value, out long l))
        //    return l;

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
