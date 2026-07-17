namespace Application.Db.Models;

public class StarboardEntry(Snowflake flake, int totalReactions, int specifiedReactions, bool posted)
{
    public int ID { get; set; }
    public Snowflake Flake { get; set; } = flake;
    public int TotalReactions { get; set; } = totalReactions;
    public int SpecifiedReactions { get; set; } = specifiedReactions;
    public DateTime AddedTime { get; set; } = DateTime.UtcNow;
    public bool Posted { get; set; } = posted;
}

