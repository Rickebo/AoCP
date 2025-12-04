namespace Common;

/// <summary>
/// Identifies a single Advent of Code problem across source, year, author, and name.
/// </summary>
public sealed record ProblemId
{
    /// <summary>
    /// Gets the Advent of Code year associated with the problem.
    /// </summary>
    public int Year { get; init; }

    /// <summary>
    /// Gets the origin of the problem set (for example, a repository or event name).
    /// </summary>
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// Gets the author of the problem implementation.
    /// </summary>
    public string Author { get; init; } = string.Empty;

    /// <summary>
    /// Gets the logical set or grouping the problem belongs to.
    /// </summary>
    public string SetName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the specific problem name within the set.
    /// </summary>
    public string ProblemName { get; init; } = string.Empty;

    /// <summary>
    /// Gets a human-readable identifier combining all ProblemId components.
    /// </summary>
    public string DisplayName =>
        $"{Source}/{Year}/{Author}/{SetName}/{ProblemName}";

    /// <summary>
    /// Creates a validated <see cref="ProblemId"/> instance from the provided components.
    /// </summary>
    /// <param name="year">Advent of Code year.</param>
    /// <param name="source">Problem source identifier.</param>
    /// <param name="author">Author name.</param>
    /// <param name="setName">Problem set name.</param>
    /// <param name="problemName">Problem name.</param>
    /// <returns>A populated <see cref="ProblemId"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when any textual component is null or whitespace.</exception>
    public static ProblemId Create(
        int year,
        string source,
        string author,
        string setName,
        string problemName
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(author);
        ArgumentException.ThrowIfNullOrWhiteSpace(setName);
        ArgumentException.ThrowIfNullOrWhiteSpace(problemName);

        return new ProblemId
        {
            Year = year,
            Source = source,
            Author = author,
            SetName = setName,
            ProblemName = problemName
        };
    }
}

