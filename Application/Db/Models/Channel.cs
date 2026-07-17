namespace Application.Db.Models;

public class Channel
{
    public int ID { get; set; }
    public Snowflake Flake { get; set; }
    public Snowflake GuildFlake { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime AddedTime { get; set; } = DateTime.UtcNow;
    public DateTime LastSeenTime { get; set; } = DateTime.UtcNow;
}
