using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Geometry;
using Lib.Grids;
using Lib.Text;

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
            var cache = new Dictionary<StringSpan, bool>();
            return Desired.Count(d => IsOption(new StringSpan(d), FilterAvailable(d, Available), cache));
        }

        public long CountSolvableOptions()
        {
            var cache = new Dictionary<StringSpan, long>();
            return Desired.Sum(d => CountOptions(new StringSpan(d), FilterAvailable(d, Available), cache));
        }

        public List<AvailableEntry> FilterAvailable(string text, string[] available)
        {
            var result = new List<AvailableEntry>();
            foreach (var item in available)
            {
                var positions = new HashSet<int>();
                for (var i = text.IndexOf(item, StringComparison.Ordinal);
                     i != -1;
                     i = text.IndexOf(item, i + 1, StringComparison.Ordinal))
                    positions.Add(i);

                if (positions.Count > 0)
                    result.Add(new AvailableEntry(item, positions.Count, positions));
            }

            return result;
        }

        private static bool IsOption(
            StringSpan text,
            List<AvailableEntry> available,
            Dictionary<StringSpan, bool> cache
        )
        {
            if (text.Length == 0)
                return true;

            if (cache.TryGetValue(text, out var value))
                return value;

            for (var i = 0; i < available.Count; i++)
            {
                var entry = available[i];
                var (ava, uses) = (entry.Text, entry.Uses);
                if (uses < 1 || !entry.Positions.Contains(text.Start))
                    continue;

                entry.Uses--;
                available[i] = entry;

                if (ava.Length == text.Length || IsOption(text.Substring(ava.Length), available, cache))
                    return cache[text] = true;

                entry.Uses = uses;
                available[i] = entry;
            }

            return cache[text] = false;
        }

        private static long CountOptions(
            StringSpan text,
            List<AvailableEntry> available,
            Dictionary<StringSpan, long> cache
        )
        {
            if (text.Length == 0)
                return 1;

            if (cache.TryGetValue(text, out var value))
                return value;

            var sum = 0L;
            for (var i = 0; i < available.Count; i++)
            {
                var entry = available[i];
                if (entry.Uses < 1 || !entry.Positions.Contains(text.Start))
                    continue;

                if (entry.Text.Length == text.Length)
                {
                    sum += 1;
                    continue;
                }

                entry.Uses--;
                available[i] = entry;

                sum += CountOptions(text.Substring(entry.Text.Length), available, cache);

                entry.Uses++;
                available[i] = entry;
            }

            return cache[text] = sum;
        }
    }

    public struct AvailableEntry(string text, int uses, HashSet<int> positions)
    {
        public string Text { get; } = text;
        public int Uses { get; set; } = uses;
        public HashSet<int> Positions { get; } = positions;
    }
}

