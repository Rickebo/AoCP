using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lib.Geometry;
using Lib.Math;
using Lib.Text;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day01 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 01);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Secret Entrance";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Send solution to frontend (Part 1)
            reporter.ReportSolution(new DialUp(input, reporter, 1).PartOne());
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
            reporter.ReportSolution(new DialUp(input, reporter, 2).PartTwo());
            return Task.CompletedTask;
        }
    }

    public class DialUp
    {
        private readonly Reporter _reporter;
        private readonly List<(Rotation, int)> _rotations = [];

        public DialUp(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            foreach (var line in input.SplitLines())
            {
                // Direction of rotation
                var dir = line[0] switch
                {
                    'L' => Rotation.CounterClockwise,
                    'R' => Rotation.Clockwise,
                    _ => throw new ProblemException("Invalid input."),
                };

                // Amount
                if (!int.TryParse(line[1..], out var amount))
                    throw new ProblemException("Invalid input.");

                // Add to list
                _rotations.Add((dir, amount));
            }
        }

        public int PartOne()
        {
            // Initial position
            int pos = 50;

            // Count how many times the dial up points at zero after rotation
            int password = 0;
            foreach (var (dir, amount) in _rotations)
            {
                // Step full amount and modulo any wrap arounds
                pos += dir == Rotation.Clockwise ? amount : -amount;
                pos = MathExtensions.Modulo(pos, 100);

                // Pointing at zero after rotation
                if (pos == 0)
                    password++;
            }

            return password;
        }

        public int PartTwo()
        {
            // Initial position
            int pos = 50;

            // Count how many times the dial up points at zero during rotation
            int password = 0;
            foreach (var (dir, amount) in _rotations)
            {
                // Step like crazy man
                for (int i = 0; i < amount; i++)
                {
                    // Take one step in direction and modulo any wrap arounds
                    int step = dir == Rotation.Clockwise ? 1 : -1;
                    pos = MathExtensions.Modulo(pos + step, 100);

                    // Pointing at zero during rotation
                    if (pos == 0)
                        password++;
                }
            }

            return password;
        }
    }
}

