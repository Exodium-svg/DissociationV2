namespace Application.Db.Models;

public class GuildMember
{
    public int ID { get; set; }
    public Snowflake UserFlake { get; set; }
    public Snowflake GuildFlake { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public int Level { get; set; }
    public DateTime AddedTime { get; set; } = DateTime.UtcNow;
    public DateTime LastSeenTime { get; set; } = DateTime.UtcNow;
}
