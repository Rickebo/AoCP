namespace Backend.Problems.Year2023;

public class Day1 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2023, 12, 01, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "First Problem";
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Second Problem";
    }
}