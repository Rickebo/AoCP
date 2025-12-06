namespace Common.Updates;

/// <summary>
/// Marks the beginning of a problem execution.
/// </summary>
public class StartProblemUpdate : OngoingProblemUpdate
{ 
    /// <inheritdoc />
    public override string Type => "start";
}

