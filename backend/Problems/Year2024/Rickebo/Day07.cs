using System.IO.IsolatedStorage;
using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024.Rickebo;

public class Day07 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 07);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Bridge Repair";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = Parse(input)
                        .Where(s => IsPossible(s, 0, 0))
                        .Sum(s => s.Target)
                        .ToString()
                }
            );

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = Parse(input)
                        .Where(s => IsPossible(s, 0, 0, true))
                        .Sum(s => s.Target)
                        .ToString()
                }
            );

            return Task.CompletedTask;
        }
    }

    private static bool IsPossible(
        NumberSeries series,
        int index,
        long sum,
        bool allowConcat = false
    )
    {
        if (sum > series.Target) return false;

        if (index >= series.Numbers.Length)
            return sum == series.Target;

        var n = series.Numbers[index];

        var result = IsPossible(series, index + 1, sum + n, allowConcat) ||
                     IsPossible(series, index + 1, sum * n, allowConcat);

        if (!allowConcat) return result;

        return result | IsPossible(
            series,
            index + 1,
            sum * (long)Math.Pow(10, Math.Ceiling(Math.Log10(n + 1))) + n,
            true
        );
    }

    private static NumberSeries[] Parse(string input) => input
        .SplitLines()
        .Select(line => NumberSeries.FromArray(Parser.GetValues<long>(line)))
        .ToArray();

    private record NumberSeries(long Target, long[] Numbers)
    {
        public static NumberSeries FromArray(long[] numbers) => new(
            numbers[0],
            numbers.Skip(1).ToArray()
        );
    }
}