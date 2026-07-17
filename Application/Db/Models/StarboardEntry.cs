namespace Application.Db.Models;
public class StarboardEntry
{
    public int Id { get; set; }
    public Snowflake Flake { get; set; }
    public int TotalReactions { get; set; }
    public int SpecifiedReactions { get; set; }
    public DateTime AddedTime { get; set; }
    public bool Posted { get; set; }

    public StarboardEntry(Snowflake flake, int totalReactions, int specifiedReactions, bool posted)
    {
        Flake = flake;
        TotalReactions = totalReactions;
        SpecifiedReactions = specifiedReactions;
        AddedTime = DateTime.UtcNow;
        Posted = posted; 
    }
}

