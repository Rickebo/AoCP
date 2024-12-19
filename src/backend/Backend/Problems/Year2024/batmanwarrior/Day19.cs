using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib;
using System.Linq;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day19 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 19);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Linen Layout";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create towel designs
            TowelDesigns towelDesigns = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(towelDesigns.Possible()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create towel designs
            TowelDesigns towelDesigns = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(towelDesigns.Ways()));
            return Task.CompletedTask;
        }
    }

    public class TowelDesigns
    {
        private readonly Reporter _reporter;
        private readonly string[] _patterns;
        private readonly string[] _designs;
        private readonly Dictionary<string, long> _memory = [];

        public TowelDesigns(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Split input to rows
            string[] rows = input.SplitLines();

            // Retrieve available patterns
            _patterns = Parser.SplitBy(rows[0], [", "]);

            // Retrieve desired designs
            _designs = rows[1..];
        }

        public long Possible() => _designs.Where((x, _) => Spelunker(x).Item1).Count();

        public long Ways() => _designs.Sum(x => Spelunker(x).Item2);

        public (bool, long) Spelunker(string design)
        {
            // Check if remaining design exist in memory
            if (_memory.TryGetValue(design, out long value))
                return (true, value);

            // If end of design reached
            if (design.Length == 0)
            {
                _memory[design] = 1;
                return (true, 1);
            }

            // Check all patterns
            long combinations = 0;
            foreach (string pattern in _patterns)
            {
                // Check if remaining design starts with pattern
                if (design.StartsWith(pattern))
                {
                    // Spelunk further
                    string remaining = design[pattern.Length..];
                    (bool possible, long ways) = Spelunker(remaining);
                    if (possible)
                    {
                        _memory[remaining] = ways;
                        combinations += ways;
                    }
                }
            }

            // If design is possible
            if (combinations > 0)
                return (true, combinations);

            // If design has no possible combination
            return (false, 0);
        }
    }
}