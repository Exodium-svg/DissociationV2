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

    /// <summary>
    /// Message template written when a Discord interaction module is found.
    /// </summary>
    public const string MODULE_FOUND = "{0} found";

    /// <summary>
    /// Message template written when temporary guild data collection completes.
    /// </summary>
    public const string GUILD_DATA_COLLECTED = "Collected {0} channels and {1} guild members for guild {2}. Data was not persisted.";
}

/// <summary>
/// Standard log messages for diagnostic progress.
/// </summary>
public static class Debug
{
    /// <summary>
    /// Message template written when a data collection request is queued.
    /// </summary>
    public const string DATA_COLLECTION_QUEUED = "Queueing {0} collection for guild {1}.";

    /// <summary>
    /// Message template written when a Discord request is enqueued.
    /// </summary>
    public const string DISCORD_REQUEST_ENQUEUED = "Enqueued Discord request {0} with priority {1}. Queue size: {2}.";

    /// <summary>
    /// Message template written when a Discord request is dequeued.
    /// </summary>
    public const string DISCORD_REQUEST_DEQUEUED = "Dequeued Discord request {0}. Queue size: {1}.";

    /// <summary>
    /// Message template written when a Discord request starts executing.
    /// </summary>
    public const string DISCORD_REQUEST_EXECUTING = "Executing Discord request {0}.";

    /// <summary>
    /// Message template written when a Discord request completes.
    /// </summary>
    public const string DISCORD_REQUEST_COMPLETED = "Completed Discord request {0}.";
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

    /// <summary>
    /// Message written for when a Request hits an exception.
    /// </summary>
    public const string REQUEST_EXCEPTION = "The request being fulfilled hit an error";

    /// <summary>
    /// Message template written when an entity cannot be found in a source.
    /// </summary>
    public const string ENTITY_NOT_FOUND_IN_SOURCE = "Failed to find {0} {1} in {2}.";

    /// <summary>
    /// Message template written when a Discord request times out.
    /// </summary>
    public const string DISCORD_REQUEST_TIMED_OUT = "Discord request {0} timed out after {1} seconds.";

    /// <summary>
    /// Message template written when a timed-out Discord request later fails.
    /// </summary>
    public const string TIMED_OUT_DISCORD_REQUEST_FAILED = "Timed-out Discord request {0} later failed: {1}";
}

/// <summary>
/// Standard log messages for temporary data inspection.
/// </summary>
public static class Data
{
    /// <summary>
    /// Message template written for a collected channel.
    /// </summary>
    public const string COLLECTED_CHANNEL = "Collected channel: {0} | guild {1} | {2}";

    /// <summary>
    /// Message template written for a collected guild member.
    /// </summary>
    public const string COLLECTED_GUILD_MEMBER = "Collected guild member: {0} | guild {1} | {2} | {3}";
}
