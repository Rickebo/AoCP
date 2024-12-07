using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;
using System;
using System.Reflection.Metadata.Ecma335;
using static Backend.Problems.Year2023.Rickebo.Day10;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day06 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 06, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Guard Gallivant";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create lab
            Lab lab = new(input, reporter);

            // Walk with guard
            lab.GuardWalk();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(lab.WalkedTiles()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create lab
            Lab lab = new(input, reporter);

            // Walk with guard
            lab.GuardWalk();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(lab.LoopCount()));
            return Task.CompletedTask;
        }
    }

    public class Lab(string input, Reporter reporter)
    {
        private readonly CharGrid _grid = new(input);
        private readonly Reporter _reporter = reporter;
        private readonly HashSet<IntegerCoordinate<int>> _walked = [];
        private record Guard(IntegerCoordinate<int> Pos, Direction Dir);

        private Guard FindGuard()
        {
            // Guard characters
            const string guardChars = "<>v^";

            // Search grid for guard position
            var pos = _grid.Find(ch => guardChars.Contains(ch.ToString()));

            // Parse direction
            var dir = DirectionExtensions.Parse(_grid[pos]);

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"Guard ({dir.Arrow()}) found at [{pos.X},{pos.Y}]"));
            return new Guard(pos, dir);
        }

        private Guard Walk(Guard guard)
        {
            // Look ahead
            IntegerCoordinate<int> next = guard.Pos.Move(guard.Dir);

            // Check if next step is an obstacle
            if (_grid.Contains(next) && _grid[next] == '#')
            {
                // Rotate clockwise
                return new Guard(guard.Pos, guard.Dir.RotateClockwise());
            }
            else
            {
                // Keep walking
                return new Guard(next, guard.Dir);
            }
        }

        public void GuardWalk()
        {
            // Find guard on lab grid
            Guard guard = FindGuard();
            IntegerCoordinate<int> start = guard.Pos;

            // Color grid
            _reporter.ReportStringGridUpdate(
                _grid,
                (builder, coordinate, val) => builder
                    .WithCoordinate(coordinate)
                    .WithText(ColorCell(coordinate))
            );

            // Walk until leaving map
            while (_grid.Contains(guard.Pos))
            {
                // Save walked tiles for later
                if (_walked.Add(guard.Pos))
                {
                    // Send to frontend grid (skip guard start pos)
                    if (!guard.Pos.Equals(start)) _reporter.ReportStringGridUpdate(guard.Pos, "#66FF66");
                }

                // Walk forward or rotate
                guard = Walk(guard);
            }
        }

        private string ColorCell(IntegerCoordinate<int> coordinate)
        {
            return _grid[coordinate] switch
            {
                '#' => "#FFFFFF", // Obstacle
                '.' => "#B3B3B3", // Floor
                _ => "#0066FF" // Guard
            };
        }

        public int WalkedTiles() => _walked.Count;

        public int LoopCount()
        {
            // Find guard on lab grid
            Guard guardStart = FindGuard();

            // Place obstacles and check for guard loops
            int loops = 0;
            foreach (IntegerCoordinate<int> pos in _walked.Skip(1))
            {
                // Place obstacle at walked tile
                _grid[pos] = '#';

                // Keep track of previous positions and directions
                HashSet<Guard> visited = [];

                // Create guard and start walking
                Guard guard = Walk(guardStart);
                while (_grid.Contains(guard.Pos))
                {
                    // Check if guard has been here before
                    if (!visited.Add(guard))
                    {
                        // Color obstacle red
                        _reporter.ReportStringGridUpdate(pos, "#FF0000");

                        // Increment loop count
                        loops++;
                        break;
                    }

                    // Walk forward or rotate
                    guard = Walk(guard);
                }

                // Remove obstacle for next iteration
                _grid[pos] = '.';
            }

            return loops;
        }
    }
}