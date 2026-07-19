namespace Application.Db.Models;

/// <summary>
/// Stored Discord guild member known to the application.
/// </summary>
public class GuildMember
{
    /// <summary>
    /// Creates an empty guild member record for database materialization.
    /// </summary>
    public GuildMember()
    {
    }

    /// <summary>
    /// Creates a guild member record from Discord member data.
    /// </summary>
    /// <param name="userFlake">Discord user snowflake.</param>
    /// <param name="guildFlake">Discord guild snowflake where the user was seen.</param>
    /// <param name="username">Current username.</param>
    /// <param name="nickname">Current guild nickname, when one is set.</param>
    public GuildMember(Snowflake userFlake, Snowflake guildFlake, string username, string? nickname = null)
    {
        UserFlake = userFlake;
        GuildFlake = guildFlake;
        Username = username;
        Nickname = nickname;
    }

    /// <summary>
    /// Local database identifier.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Discord user snowflake.
    /// </summary>
    public Snowflake UserFlake { get; set; }

    /// <summary>
    /// Discord guild snowflake where the user was seen.
    /// </summary>
    public Snowflake GuildFlake { get; set; }

    /// <summary>
    /// Current username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Current guild nickname, when one is set.
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// Application level assigned to the member.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Time when the member was first stored.
    /// </summary>
    public DateTime AddedTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Time when the member was last seen by the application.
    /// </summary>
    public DateTime LastSeenTime { get; set; } = DateTime.UtcNow;
}
