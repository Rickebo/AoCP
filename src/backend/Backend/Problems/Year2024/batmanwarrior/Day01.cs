using Common;
using Lib.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day01 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2024, 12, 01);

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
        private readonly List<int> _left = [];
        private readonly List<int> _right = [];

        public Solver(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            foreach (string line in input.SplitLines())
            {
                // Retrieve numbers
                int[] numbers = Parser.GetValues<int>(line);
                if (numbers.Length != 2)
                    throw new ProblemException("Invalid input.");

                // Populate lists
                _left.Add(numbers[0]);
                _right.Add(numbers[1]);
            }

            // Print
            _reporter.ReportLine($"Left list created with {_left.Count} entries.");
            _reporter.ReportLine($"Right list created with {_right.Count} entries.\n");
        }

        public int PartOne()
        {
            // Sort
            _left.Sort();
            _right.Sort();

            // Print
            _reporter.ReportLine("Left  | Right | Distance | Total Distance");

            // Retrieve list distances
            int totalDistance = 0;
            for (int i = 0; i < _left.Count; i++)
            {
                // Add distance to total
                int distance = Math.Abs(_left[i] - _right[i]);
                totalDistance += distance;

                // Print
                _reporter.ReportLine($"{_left[i],-8}{_right[i],-8}{distance,-11}{totalDistance}");
            }

            return totalDistance;
        }

        public int PartTwo()
        {
            // Print
            _reporter.ReportLine("Left  | Occurence | Similarity Score | Total Similarity Score");

            // Retrieve similarity scores
            int totalSimilarity = 0;
            for (int i = 0; i < _left.Count; i++)
            {
                // Add similarity score to total
                int occurences = _right.Where(x => x.Equals(_left[i])).Count();
                int similarity = _left[i] * occurences;
                totalSimilarity += similarity;

                // Print
                _reporter.ReportLine($"{_left[i],-8}{occurences,-12}{similarity,-19}{totalSimilarity}");
            }

            return totalSimilarity;
        }
    }
}

