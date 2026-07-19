namespace Application.Db.Models;

/// <summary>
/// Stored Discord message collected from a channel.
/// </summary>
public class Post
{
    /// <summary>
    /// Creates an empty post record for database materialization.
    /// </summary>
    public Post()
    {
    }

    /// <summary>
    /// Creates a post record from Discord message data.
    /// </summary>
    /// <param name="messageFlake">Discord message snowflake.</param>
    /// <param name="channelFlake">Discord channel snowflake where the message was sent.</param>
    /// <param name="guildFlake">Discord guild snowflake where the message was sent.</param>
    /// <param name="authorFlake">Discord user snowflake for the message author.</param>
    /// <param name="content">Message content captured by the application.</param>
    /// <param name="createdTime">Time when Discord created the message.</param>
    public Post(
        Snowflake messageFlake,
        Snowflake channelFlake,
        Snowflake guildFlake,
        Snowflake authorFlake,
        string content,
        DateTime createdTime)
    {
        MessageFlake = messageFlake;
        ChannelFlake = channelFlake;
        GuildFlake = guildFlake;
        AuthorFlake = authorFlake;
        Content = content;
        CreatedTime = createdTime;
    }

    /// <summary>
    /// Local database identifier.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Discord message snowflake.
    /// </summary>
    public Snowflake MessageFlake { get; set; }

    /// <summary>
    /// Discord channel snowflake where the message was sent.
    /// </summary>
    public Snowflake ChannelFlake { get; set; }

    /// <summary>
    /// Discord guild snowflake where the message was sent.
    /// </summary>
    public Snowflake GuildFlake { get; set; }

    /// <summary>
    /// Discord user snowflake for the message author.
    /// </summary>
    public Snowflake AuthorFlake { get; set; }

    /// <summary>
    /// Message content captured by the application.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Time when Discord created the message.
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// Time when the message was first stored.
    /// </summary>
    public DateTime AddedTime { get; set; } = DateTime.UtcNow;
}
