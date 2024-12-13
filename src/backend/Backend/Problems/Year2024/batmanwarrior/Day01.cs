using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day01 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 01);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Historian Hysteria";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create list manager
            ListManager listManager = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(listManager.Distance()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create list manager
            ListManager listManager = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(listManager.Similarity()));
            return Task.CompletedTask;
        }
    }

    public class ListManager
    {
        private readonly Reporter _reporter;
        private readonly List<int> _left = [];
        private readonly List<int> _right = [];

        public ListManager(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Check every row of input
            foreach (string row in input.SplitLines())
            {
                // Parse numbers and populate lists
                int[] numbers = Parser.GetValues<int>(row);
                _left.Add(numbers[0]);
                _right.Add(numbers[1]);
            }

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"List pair created with {_left.Count} entries.\n"));
        }

        public int Distance()
        {
            // Sort lists
            _left.Sort();
            _right.Sort();

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine("Left  | Right | Distance | Total Distance"));

            // Loop through lists
            int totalDistance = 0;
            for (int i = 0; i < _left.Count; i++)
            {
                // Get distance between numbers
                int distance = Math.Abs(_left[i] - _right[i]);

                // Accumulate distance
                totalDistance += distance;

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"{_left[i], -8}{_right[i], -8}{distance, -11}{totalDistance}"));
            }

            return totalDistance;
        }

        public int Similarity()
        {
            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine("Left  | Occurence | Similarity Score | Total Similarity Score"));

            // Loop through lists
            int totalSimilarity = 0;
            for (int i = 0; i < _left.Count; i++)
            {
                // Count occurences of left in right
                int occurences = _right.Where(x => x.Equals(_left[i])).Count();

                // Calculate similarity score
                int similarity = _left[i] * occurences;

                // Accumulate similarity
                totalSimilarity += similarity;

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"{_left[i], -8}{occurences, -12}{similarity, -19}{totalSimilarity}"));
            }

            return totalSimilarity;
        }
    }
}