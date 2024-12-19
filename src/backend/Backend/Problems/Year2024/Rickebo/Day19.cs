using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.Rickebo;

public class Day19 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 19);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Linen Layout";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(Data.Parse(input).CountSolvable());

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(Data.Parse(input).CountSolvableOptions());

            return Task.CompletedTask;
        }
    }

    public record Data(string[] Available, string[] Desired)
    {
        public static Data Parse(string input)
        {
            var lines = input.SplitLines();
            return new Data(
                lines[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                lines[1..]
            );
        }

        public int CountSolvable()
        {
            var cache = new Dictionary<string, bool>();
            return Desired.Count(d => IsOption(d, cache));
        }

        public long CountSolvableOptions()
        {
            var cache = new Dictionary<string, long>();
            return Desired.Sum(d => CountOptions(d, cache));
        }

        public bool IsOption(string text, Dictionary<string, bool> cache)
        {
            if (text.Length == 0)
                return true;

            if (cache.TryGetValue(text, out var value))
                return value;

            return cache[text] = Available
                .Any(a => text.StartsWith(a) && IsOption(text[a.Length..], cache));
        }

        public long CountOptions(string text, Dictionary<string, long> cache)
        {
            if (text.Length == 0)
                return 1;

            if (cache.TryGetValue(text, out var value))
                return value;

            return cache[text] = Available
                .Where(text.StartsWith)
                .Sum(available => CountOptions(text[available.Length..], cache));
        }
    }
}