namespace Common.Updates;

/// <summary>
/// Base class for updates that occur while a problem is still executing.
/// </summary>
public class OngoingProblemUpdate : ProblemUpdate
{
    /// <summary>
    /// Gets the update type string used for ongoing updates.
    /// </summary>
    public override string Type => "ongoing";
}

