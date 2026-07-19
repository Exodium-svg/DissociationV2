namespace Application.Db.Models;

/// <summary>
/// Stored Discord channel known to the application.
/// </summary>
public class Channel
{
    /// <summary>
    /// Creates an empty channel record for database materialization.
    /// </summary>
    public Channel()
    {
    }

    /// <summary>
    /// Creates a channel record from Discord channel data.
    /// </summary>
    /// <param name="channelFlake">Discord channel snowflake.</param>
    /// <param name="guildFlake">Discord guild snowflake that owns the channel.</param>
    /// <param name="name">Current channel name.</param>
    public Channel(Snowflake channelFlake, Snowflake guildFlake, string name)
    {
        ChannelFlake = channelFlake;
        GuildFlake = guildFlake;
        Name = name;
    }

    /// <summary>
    /// Local database identifier.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Discord channel snowflake.
    /// </summary>
    public Snowflake ChannelFlake { get; set; }

    /// <summary>
    /// Discord guild snowflake that owns the channel.
    /// </summary>
    public Snowflake GuildFlake { get; set; }

    /// <summary>
    /// Current channel name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Time when the channel was first stored.
    /// </summary>
    public DateTime AddedTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Time when the channel was last seen by the application.
    /// </summary>
    public DateTime LastSeenTime { get; set; } = DateTime.UtcNow;
}
