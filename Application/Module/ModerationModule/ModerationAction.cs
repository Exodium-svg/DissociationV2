using static Application.Utils.LoggingMessages.Error;

using Application.Db.Models;

namespace Application.Module.ModerationModule;

/// <summary>
/// 
/// </summary>
public enum Actions
{
    /// <summary>
    /// 
    /// </summary>
    Note = 0,

    /// <summary>
    /// 
    /// </summary>
    Warn = 1,

    /// <summary>
    /// 
    /// </summary>
    Timeout = 2,
    
    /// <summary>
    /// 
    /// </summary>
    RemoveTimeout = 3,

    /// <summary>
    /// 
    /// </summary>
    Kick = 4,

    /// <summary>
    /// 
    /// </summary>
    Ban = 5,
    
    /// <summary>
    /// 
    /// </summary>
    Unban = 6,

    /// <summary>
    /// 
    /// </summary>
    DeleteMessage = 7,
    
    /// <summary>
    /// 
    /// </summary>
    PurgeMessages = 8,

    /// <summary>
    /// 
    /// </summary>
    ChangeNickname = 9,
    
    /// <summary>
    /// 
    /// </summary>
    ResetNickname = 10,

    /// <summary>
    /// 
    /// </summary>
    AddRole = 11,
    
    /// <summary>
    /// 
    /// </summary>
    RemoveRole = 12,

    /// <summary>
    /// 
    /// </summary>
    Mute = 13,

    /// <summary>
    /// 
    /// </summary>
    Unmute = 14,

    /// <summary>
    /// 
    /// </summary>
    Deafen = 15,
    
    /// <summary>
    /// 
    /// </summary>
    Undeafen = 16,

    /// <summary>
    /// 
    /// </summary>
    MoveVoiceChannel = 17,

    /// <summary>
    /// 
    /// </summary>
    DisconnectVoice = 18
}

/// <summary>
/// 
/// </summary>
public interface IActionType
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    bool Commit();
};

public class DisciplinaryAction : IActionType
{
    /// <summary>
    /// 
    /// </summary>
    public string? ReasonForAction;

    /// <summary>
    ///
    /// </summary>
    public Rule? RuleInfraction;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Commit()
    {
        throw new NotImplementedException(NOT_DONE);
    }
};

/// <summary>
/// 
/// </summary>
public class SafeAction : IActionType
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Commit()
    {
        throw new NotImplementedException(NOT_DONE);
    }
};

/// <summary>
/// 
/// </summary>
public class AutomatedAction : IActionType
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Commit()
    {
        throw new NotImplementedException(NOT_DONE);
    }
}

/// <summary>
/// 
/// </summary>
public class ModerationAction
{
    /// <summary>
    /// Local database identifier.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required IActionType Action { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required Snowflake UserAffectedFlake { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required Snowflake EnforcerUser { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? AdditionalNotes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsAnnoymous { get; set; }

}