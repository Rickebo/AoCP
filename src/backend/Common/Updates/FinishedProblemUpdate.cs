namespace Common.Updates;

public class FinishedProblemUpdate : ProblemUpdate
{
    public override string Type => "finished";

    public bool Successful { get; set; }

    public string? Solution { get; set; }

    public string? Error { get; set; }

    public long ElapsedNanoseconds { get; set; }

    public static FinishedProblemUpdate FromSolution(IFormattable solution) =>
        FromSolution(solution.ToString() ?? string.Empty);

    public static FinishedProblemUpdate FromSolution(string solution) =>
        new()
        {
            Solution = solution ?? string.Empty,
            Error = null,
            Successful = true
        };

    public static FinishedProblemUpdate FromError(string error, string? solution = null) =>
        new()
        {
            Solution = solution,
            Error = error ?? string.Empty,
            Successful = false
        };
}

