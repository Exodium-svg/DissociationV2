namespace Application.Utils.LoggingMessages;

public static class Success
{
    public const string COMMAND_REGISTER_SUCCESS = "Succesfully registered on!";
    public const string SETTING_READ_SUCCESS = "Succesfully Grabed all settings!";
}

public static class Error
{
    public const string NO_TOKEN = "Not token set, please add discord.token to your settings file as string!";
    public const string NO_GUILD = "Debug guild ID has not been set on guild.test_id, are we supposed to be a debug version?";
    public const string CMD_FAIL = "Failed to register commands!";

    public const string NO_SETTINGS = "Failed to Grab any settings? Is this intentional?";
    public const string EMPTY_VALUE = "Setting '{0}' has an empty value.";
    public const string EMPTY_KEY = "Skipped line {0}: the setting key is empty.";
    public const string INVALID_LINE = "Skipped line {0}: the setting is not valid.";

    public const string NO_STAR_CHANNEL = "Failed to log to starChannel, invalid ID?";
    public const string BAD_STAR_CHANNEL = "Failed to add message to starboard, please ensure the ID given for the StarChannel is valid!";

    public const string NOT_DONE = "Not Implemented";
}
