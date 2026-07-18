using System.Text;
using Application.Utils;

namespace Application.Tests;

/// <summary>
/// Tests parsing and fallback behavior for application settings using in-memory mock data.
/// </summary>
[TestClass]
public sealed class SettingsTest
{
    /// <summary>
    /// Verifies that supported mock setting values are parsed into their expected runtime types.
    /// </summary>
    [TestMethod]
    public void ParsesMockSettings()
    {
        const string mockSettings = """
            string_value = "mock text"
            single_quoted = 'mock single'
            bool_value = true
            ulong_value = 12345
            float_value = 1.5
            raw_value = unquoted
            """;

        Settings settings = CreateSettings(mockSettings);

        Assert.AreEqual("mock text", settings.Get("string_value", string.Empty));
        Assert.AreEqual("mock single", settings.Get("single_quoted", string.Empty));
        Assert.IsTrue(settings.Get("bool_value", false));
        Assert.AreEqual(12345UL, settings.Get("ulong_value", 0UL));
        Assert.AreEqual(1.5f, settings.Get("float_value", 0f));
        Assert.AreEqual("unquoted", settings.Get("raw_value", string.Empty));
    }

    /// <summary>
    /// Verifies that a missing setting key returns the supplied fallback value.
    /// </summary>
    [TestMethod]
    public void MissingKeyUsesFallback()
    {
        Settings settings = CreateSettings("existing = true");

        bool value = settings.Get("missing", false);

        Assert.IsFalse(value);
    }

    /// <summary>
    /// Verifies that a setting with a different stored type returns the supplied fallback value.
    /// </summary>
    [TestMethod]
    public void TypeMismatchUsesFallback()
    {
        Settings settings = CreateSettings("number = 42");

        string value = settings.Get("number", "fallback");

        Assert.AreEqual("fallback", value);
    }

    /// <summary>
    /// Verifies that settings input without registered values emits a warning log entry.
    /// </summary>
    [TestMethod]
    public void EmptySettingsLogsWarning()
    {
        var loggerService = new TestLoggerService();
        var logger = new Logger([loggerService]);

        _ = new Settings(logger, Encoding.UTF8.GetBytes("""
            # comment
            // another comment
            """));

        Assert.IsTrue(loggerService.Entries.Any(entry => entry.Level == LogLevel.Warning));
    }

    /// <summary>
    /// Verifies that later values overwrite earlier values for duplicate setting keys.
    /// </summary>
    [TestMethod]
    public void DuplicateKeyUsesLastValue()
    {
        Settings settings = CreateSettings("""
            repeated = "first"
            repeated = "second"
            """);

        Assert.AreEqual("second", settings.Get("repeated", string.Empty));
    }

    /// <summary>
    /// Verifies that malformed settings lines are ignored while valid lines are still stored.
    /// </summary>
    [TestMethod]
    public void InvalidLinesAreIgnored()
    {
        var loggerService = new TestLoggerService();
        var logger = new Logger([loggerService]);

        var settings = new Settings(logger, Encoding.UTF8.GetBytes("""
            valid = "stored"
            invalid line
            = missing_key
            """));

        Assert.AreEqual("stored", settings.Get("valid", string.Empty));
        Assert.AreEqual("fallback", settings.Get("invalid line", "fallback"));
        Assert.IsTrue(loggerService.Entries.Any(entry => entry.Level == LogLevel.Error));
    }

    /// <summary>
    /// Verifies that a key with no value is stored as an empty string.
    /// </summary>
    [TestMethod]
    public void EmptyValueIsStored()
    {
        Settings settings = CreateSettings("empty = ");

        Assert.AreEqual(string.Empty, settings.Get("empty", "fallback"));
    }

    /// <summary>
    /// Documents the current parser behavior for values that overflow the float parser.
    /// </summary>
    [TestMethod]
    public void HugeNumberBecomesFloatInfinity()
    {
        Settings settings = CreateSettings("huge = 1.7976931348623157E+308");

        Assert.IsTrue(float.IsPositiveInfinity(settings.Get("huge", 0f)));
    }

    /// <summary>
    /// Creates a settings instance from UTF-8 encoded mock text.
    /// </summary>
    /// <param name="mockSettings">Settings text to parse.</param>
    /// <returns>A settings instance backed by the supplied mock data.</returns>
    private static Settings CreateSettings(string mockSettings)
    {
        var logger = new Logger([new TestLoggerService()]);
        return new Settings(logger, Encoding.UTF8.GetBytes(mockSettings));
    }
}
