namespace Application.Utils.LoggingMessages;

/// <summary>
/// Standard log messages for successful operations.
/// </summary>
public static class Success
{
    /// <summary>
    /// Message written after slash commands are registered successfully.
    /// </summary>
    public const string COMMAND_REGISTER_SUCCESS = "Succesfully registered on!";

    /// <summary>
    /// Message written after settings are read successfully.
    /// </summary>
    public const string SETTING_READ_SUCCESS = "Succesfully Grabed all settings!";
}

/// <summary>
/// Standard log messages for errors and warnings.
/// </summary>
public static class Error
{
    /// <summary>
    /// Message written when the Discord token setting is missing.
    /// </summary>
    public const string NO_TOKEN = "Not token set, please add discord.token to your settings file as string!";

    /// <summary>
    /// Message written when the debug guild setting is missing.
    /// </summary>
    public const string NO_GUILD = "Debug guild ID has not been set on guild.test_id, are we supposed to be a debug version?";

    /// <summary>
    /// Message written when command registration fails.
    /// </summary>
    public const string CMD_FAIL = "Failed to register commands!";

    /// <summary>
    /// Message written when no settings were loaded.
    /// </summary>
    public const string NO_SETTINGS = "Failed to Grab any settings? Is this intentional?";

    /// <summary>
    /// Message template written when a setting has no value.
    /// </summary>
    public const string EMPTY_VALUE = "Setting '{0}' has an empty value.";

    /// <summary>
    /// Message template written when a settings line has no key.
    /// </summary>
    public const string EMPTY_KEY = "Skipped line {0}: the setting key is empty.";

    /// <summary>
    /// Message template written when a settings line cannot be parsed.
    /// </summary>
    public const string INVALID_LINE = "Skipped line {0}: the setting is not valid.";

    /// <summary>
    /// Message written when a starboard channel cannot be found or used.
    /// </summary>
    public const string NO_STAR_CHANNEL = "Failed to log to starChannel, invalid ID?";

    /// <summary>
    /// Message written when a message cannot be added to the starboard channel.
    /// </summary>
    public const string BAD_STAR_CHANNEL = "Failed to add message to starboard, please ensure the ID given for the StarChannel is valid!";

    /// <summary>
    /// Message written for code paths that have not been implemented yet.
    /// </summary>
    public const string NOT_DONE = "Not Implemented";
}
