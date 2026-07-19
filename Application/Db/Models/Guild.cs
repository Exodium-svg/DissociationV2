namespace Application.Db.Models;

/// <summary>
/// Stored Discord guild known to the application.
/// </summary>
public class Guild
{
    /// <summary>
    /// Creates an empty guild record for database materialization.
    /// </summary>
    public Guild()
    {
    }

    /// <summary>
    /// Creates a guild record from Discord guild data.
    /// </summary>
    /// <param name="flake">Discord guild snowflake.</param>
    /// <param name="name">Current guild name.</param>
    public Guild(Snowflake flake, string name)
    {
        Flake = flake;
        Name = name;
    }

    /// <summary>
    /// Local database identifier.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Discord guild snowflake.
    /// </summary>
    public Snowflake Flake { get; set; }

    /// <summary>
    /// Current guild name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Time when the guild was first stored.
    /// </summary>
    public DateTime AddedTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Time when the guild was last seen by the application.
    /// </summary>
    public DateTime LastSeenTime { get; set; } = DateTime.UtcNow;
}
