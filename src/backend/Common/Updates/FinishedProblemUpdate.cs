namespace Common.Updates;

/// <summary>
/// Signals that a problem execution has completed.
/// </summary>
public class FinishedProblemUpdate : ProblemUpdate
{
    /// <inheritdoc />
    public override string Type => "finished";

    /// <summary>
    /// Gets or sets a value indicating whether the run completed successfully.
    /// </summary>
    public bool Successful { get; set; }

    /// <summary>
    /// Gets or sets the rendered solution when available.
    /// </summary>
    public string? Solution { get; set; }

    /// <summary>
    /// Gets or sets an error message when the run fails.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the elapsed execution time in nanoseconds.
    /// </summary>
    public long ElapsedNanoseconds { get; set; }

    /// <summary>
    /// Creates a successful update from a formattable solution.
    /// </summary>
    /// <param name="solution">Solution value to render.</param>
    /// <returns>A <see cref="FinishedProblemUpdate"/> marked as successful.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="solution"/> is null.</exception>
    public static FinishedProblemUpdate FromSolution(IFormattable solution)
    {
        ArgumentNullException.ThrowIfNull(solution);
        return FromSolution(solution.ToString() ?? string.Empty);
    }

    /// <summary>
    /// Creates a successful update from a solution string.
    /// </summary>
    /// <param name="solution">Solution string to emit.</param>
    /// <returns>A <see cref="FinishedProblemUpdate"/> marked as successful.</returns>
    public static FinishedProblemUpdate FromSolution(string solution) =>
        new()
        {
            Solution = solution ?? string.Empty,
            Error = null,
            Successful = true
        };

    /// <summary>
    /// Creates a failed update with an error message and optional partial solution.
    /// </summary>
    /// <param name="error">Error message describing the failure.</param>
    /// <param name="solution">Optional partial solution computed before failing.</param>
    /// <returns>A <see cref="FinishedProblemUpdate"/> marked as unsuccessful.</returns>
    public static FinishedProblemUpdate FromError(string error, string? solution = null) =>
        new()
        {
            Solution = solution,
            Error = error ?? string.Empty,
            Successful = false
        };
}

