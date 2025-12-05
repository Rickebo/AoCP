using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day05 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 05);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Cafeteria";

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
        private readonly List<NumberRange<long>> _ranges = [];
        private readonly List<long> _ingredients = [];

        public Solver(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            foreach (var line in input.SplitLines())
            {
                var vals = Parser.GetValues<long>(line.Replace('-', ' '));
                if (vals.Length == 2)
                    _ranges.Add(new(Math.Min(vals[0], vals[1]), Math.Max(vals[0], vals[1])));
                else if (vals.Length == 1)
                    _ingredients.Add(vals[0]);
            }
        }

        public int PartOne()
        {
            // Count fresh ingredients
            int freshIngredients = 0;
            foreach (var ingredient in _ingredients)
            {
                foreach (var range in _ranges)
                {
                    // Fresh
                    if (range.Contains(ingredient))
                    {
                        freshIngredients++;
                        break;
                    }
                }
            }

            return freshIngredients;
        }

        public long PartTwo()
        {
            // Merge all ranges
            for (int i = 0; i < _ranges.Count - 1; i++)
            {
                for (int j = i + 1; j < _ranges.Count; j++)
                {
                    // Intersection
                    if (_ranges[i].Intersects(_ranges[j]))
                    {
                        // Merge
                        _ranges[i] = _ranges[i].Union(_ranges[j]);
                        _ranges.RemoveAt(j);
                        i--; // Re-check with merged
                        break;
                    }
                }
            }

            // Sum
            long freshIDs = 0;
            foreach (var range in _ranges)
                freshIDs += range.Length;

            return freshIDs;
        }
    }
}
