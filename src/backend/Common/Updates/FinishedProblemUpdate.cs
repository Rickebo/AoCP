namespace Common.Updates;

/// <summary>
/// Represents a terminal update produced when a problem run has completed.
/// </summary>
public class FinishedProblemUpdate : ProblemUpdate
{
    /// <summary>
    /// Gets the update type string used for completion notifications.
    /// </summary>
    public override string Type => "finished";

    /// <summary>
    /// Gets or sets a value indicating whether the run completed successfully.
    /// </summary>
    public bool Successful { get; set; }

    /// <summary>
    /// Gets or sets the textual solution produced by the problem run.
    /// </summary>
    public string? Solution { get; set; }

    /// <summary>
    /// Gets or sets the error message associated with a failed run.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the elapsed runtime of the problem in nanoseconds.
    /// </summary>
    public long ElapsedNanoseconds { get; set; }

    /// <summary>
    /// Creates a successful update from a formatted solution value.
    /// </summary>
    /// <param name="solution">Solution value that can be formatted as a string.</param>
    /// <returns>A successful <see cref="FinishedProblemUpdate"/> instance.</returns>
    public static FinishedProblemUpdate FromSolution(IFormattable solution) =>
        FromSolution(solution.ToString() ?? string.Empty);

    /// <summary>
    /// Creates a successful update from a string solution.
    /// </summary>
    /// <param name="solution">Solution text.</param>
    /// <returns>A successful <see cref="FinishedProblemUpdate"/> instance.</returns>
    public static FinishedProblemUpdate FromSolution(string solution) =>
        new()
        {
            Solution = solution ?? string.Empty,
            Error = null,
            Successful = true
        };

    /// <summary>
    /// Creates a failed update carrying an error description.
    /// </summary>
    /// <param name="error">Error message describing the failure.</param>
    /// <param name="solution">Optional solution text when partial results exist.</param>
    /// <returns>A failed <see cref="FinishedProblemUpdate"/> instance.</returns>
    public static FinishedProblemUpdate FromError(string error, string? solution = null) =>
        new()
        {
            Solution = solution,
            Error = error ?? string.Empty,
            Successful = false
        };
}

