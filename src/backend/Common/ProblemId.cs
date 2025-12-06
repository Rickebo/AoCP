namespace Common;

public sealed record ProblemId
{
    public int Year { get; init; }

    public string Source { get; init; } = string.Empty;

    public string Author { get; init; } = string.Empty;

    public string SetName { get; init; } = string.Empty;

    public string ProblemName { get; init; } = string.Empty;

    public string DisplayName =>
        $"{Source}/{Year}/{Author}/{SetName}/{ProblemName}";

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

