namespace Application.Db.Models;

/// <summary>
/// Stored starboard state for a Discord message.
/// </summary>
public class StarboardEntry
{
    /// <summary>
    /// Creates an empty starboard entry for database materialization.
    /// </summary>
    public StarboardEntry()
    {
    }

    /// <summary>
    /// Creates a starboard entry for a Discord message.
    /// </summary>
    /// <param name="flake">Discord message snowflake.</param>
    /// <param name="totalReactions">Total reactions counted for the message.</param>
    /// <param name="specifiedReactions">Specific configured reactions counted for the message.</param>
    /// <param name="posted">Whether the message has already been posted to the starboard.</param>
    public StarboardEntry(Snowflake flake, int totalReactions, int specifiedReactions, bool posted)
    {
        Flake = flake;
        TotalReactions = totalReactions;
        SpecifiedReactions = specifiedReactions;
        Posted = posted;
    }

    /// <summary>
    /// Local database identifier.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Discord message snowflake.
    /// </summary>
    public Snowflake Flake { get; set; }

    /// <summary>
    /// Total reactions counted for the message.
    /// </summary>
    public int TotalReactions { get; set; }

    /// <summary>
    /// Specific configured reactions counted for the message.
    /// </summary>
    public int SpecifiedReactions { get; set; }

    /// <summary>
    /// Time when the starboard entry was first stored.
    /// </summary>
    public DateTime AddedTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether the message has already been posted to the starboard.
    /// </summary>
    public bool Posted { get; set; }
}
