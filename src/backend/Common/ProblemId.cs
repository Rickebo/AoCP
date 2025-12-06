namespace Common;

/// <summary>
/// Identifies a specific Advent of Code problem within the platform.
/// </summary>
public sealed record ProblemId
{
    /// <summary>
    /// Gets the Advent of Code year that owns the problem.
    /// </summary>
    public int Year { get; init; }

    /// <summary>
    /// Gets the problem source (e.g. <c>AOC</c>).
    /// </summary>
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// Gets the author for the problem implementation.
    /// </summary>
    public string Author { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the problem set (day).
    /// </summary>
    public string SetName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the display-friendly problem name.
    /// </summary>
    public string ProblemName { get; init; } = string.Empty;

    /// <summary>
    /// Gets a canonical display name composed from the identifier parts.
    /// </summary>
    public string DisplayName =>
        $"{Source}/{Year}/{Author}/{SetName}/{ProblemName}";

    /// <inheritdoc />
    public override string ToString() => DisplayName;

    /// <summary>
    /// Creates a validated <see cref="ProblemId"/> instance.
    /// </summary>
    /// <param name="year">Year of the Advent of Code challenge.</param>
    /// <param name="source">Problem source identifier.</param>
    /// <param name="author">Name of the solution author.</param>
    /// <param name="setName">Name of the problem set/day.</param>
    /// <param name="problemName">Display name of the problem.</param>
    /// <returns>A constructed <see cref="ProblemId"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="year"/> is negative.</exception>
    /// <exception cref="ArgumentException">Thrown when any string parameter is null or whitespace.</exception>
    public static ProblemId Create(
        int year,
        string source,
        string author,
        string setName,
        string problemName
    )
    {
        ArgumentOutOfRangeException.ThrowIfNegative(year);
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

