namespace Application.Db.Models;

public class Rule {
    
    /// <summary>
    /// Local database identifier.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string RuleName {get; set;}

    /// <summary>
    /// 
    /// </summary>
    public string? RuleDescription {get; set;}
}; 