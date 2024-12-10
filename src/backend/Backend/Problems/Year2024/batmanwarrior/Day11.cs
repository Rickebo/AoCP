using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day11 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 11);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Temp";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            string[] rows = input.SplitLines();

            reporter.Report(FinishedProblemUpdate.FromSolution(""));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            string[] rows = input.SplitLines();

            reporter.Report(FinishedProblemUpdate.FromSolution(""));
            return Task.CompletedTask;
        }
    }
}