namespace Common.Updates;

/// <summary>
/// Signals that a problem has started running.
/// </summary>
public class StartProblemUpdate : OngoingProblemUpdate
{ 
    /// <summary>
    /// Gets the update type string used for start notifications.
    /// </summary>
    public override string Type => "start";
}

