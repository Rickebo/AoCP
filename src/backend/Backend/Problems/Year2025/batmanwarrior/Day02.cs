using Common;
using Lib.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day02 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 02);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Gift Shop";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Send solution to frontend (Part 1)
            reporter.ReportSolution(new Solver(input, reporter, 1).PartOne());
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Send solution to frontend (Part 2)
            reporter.ReportSolution(new Solver(input, reporter, 2).PartTwo());
            return Task.CompletedTask;
        }
    }

    public class Solver
    {
        private readonly Reporter _reporter;
        private readonly List<(ulong, ulong)> _ranges = [];

        public Solver(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input ranges
            var values = Parser.GetValues<ulong>(input);
            for (int i = 0; i < values.Length; i += 2)
                _ranges.Add((values[i], values[i + 1]));
        }

        public ulong PartOne()
        {
            // Sum invalid IDs (two equal halves)
            ulong sum = 0;
            foreach (var (start, end) in _ranges)
            {
                for (ulong i = start; i <= end; i++)
                {
                    string str = i.ToString();
                    int len = str.Length;

                    // Must be even length
                    if (len % 2 != 0)
                        continue;

                    // Check if halves are equal
                    int half = len / 2;
                    bool equal = true;
                    for (int j = 0; j < half; j++)
                    {
                        if (str[j] != str[j + half])
                        {
                            equal = false;
                            break;
                        }
                    }

                    if (equal)
                        sum += i;
                }
            }

            return sum;
        }

        public ulong PartTwo()
        {
            // Sum invalid IDs (all repeated chunks equal)
            ulong sum = 0;
            foreach (var range in _ranges)
            {
                for (ulong i = range.Item1; i <= range.Item2; i++)
                {
                    string str = i.ToString();
                    int len = str.Length;

                    // Skip single digit numbers
                    if (len <= 1)
                        continue;

                    if (RepeatingSequence(str))
                        sum += i;
                }
            }

            return sum;
        }

        private static bool RepeatingSequence(string s)
        {
            int len = s.Length;

            // Check pattern up to len / 2
            for (int patternLen = 1; patternLen <= len / 2; patternLen++)
            {
                // Skip not evenly divided sequences
                if (len % patternLen != 0)
                    continue;

                // Compare each character in sequence with pattern
                bool allMatch = true;
                for (int i = patternLen; i < len; i++)
                {
                    if (s[i] != s[i % patternLen])
                    {
                        allMatch = false;
                        break;
                    }
                }

                if (allMatch)
                    return true;
            }

            return false;
        }
    }
}

