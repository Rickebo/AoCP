namespace Common.Updates;

/// <summary>
/// Represents a problem update emitted while execution is still in progress.
/// </summary>
public class OngoingProblemUpdate : ProblemUpdate
{
    /// <inheritdoc />
    public override string Type => "ongoing";
}

