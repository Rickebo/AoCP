using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Coordinate;
using Lib.Enums;
using Lib.Extensions;
using Lib.Grid;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day06 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 06);

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

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(lab.GuardPositions()));
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

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(lab.LoopPositions()));
            return Task.CompletedTask;
        }
    }

    public class Lab
    {
        private record Guard(IntegerCoordinate<int> Pos, Direction Dir);
        private readonly Reporter _reporter;
        private readonly CharGrid _grid;
        private readonly HashSet<Guard> _guardStates = [];

        public Lab (string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Create grid
            _grid = new(input);

            // Color grid
            _reporter.ReportStringGridUpdate(
                _grid,
                (builder, coordinate, val) => builder
                    .WithCoordinate(coordinate)
                    .WithText(ColorCell(coordinate))
            );
        }

        private Guard FindGuard()
        {
            // Guard characters
            const string guardChars = "<>v^";

            // Search grid for guard position
            IntegerCoordinate<int> pos = _grid.Find(ch => guardChars.Contains(ch.ToString()));

            // Parse direction
            Direction dir = DirectionExtensions.Parse(_grid[pos]);

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
                return new Guard(guard.Pos, guard.Dir.Rotate(Rotation.Clockwise));
            }
            else
            {
                // Keep walking
                return new Guard(next, guard.Dir);
            }
        }

        private int GuardWalk(IntegerCoordinate<int> start, Direction dir)
        {
            // Create guard
            Guard guard = new(start, dir);

            // Visited tiles
            HashSet<IntegerCoordinate<int>> visited = [];

            // Walk until leaving map
            while (_grid.Contains(guard.Pos))
            {
                // Save guard state
                _guardStates.Add(guard);

                // Check if new tile
                if (visited.Add(guard.Pos))
                {
                    // Send to frontend grid (skip guard start pos)
                    if (!guard.Pos.Equals(start))
                        _reporter.ReportStringGridUpdate(guard.Pos, "#66FF66");
                }

                // Walk forward or rotate
                guard = Walk(guard);
            }

            return visited.Count;
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

        public int GuardPositions()
        {
            // Find guard in lab
            Guard guard = FindGuard();

            // Walk and return walked tiles
            return GuardWalk(guard.Pos, guard.Dir);
        }

        public int LoopPositions()
        {
            // Find guard on lab grid
            Guard prevState = FindGuard();

            // Walk with guard to obtain states
            GuardWalk(prevState.Pos, prevState.Dir);

            // Placed obstructions
            HashSet<IntegerCoordinate<int>> obstructionTiles = [];

            // Place obstacles and check for guard loops
            int loops = 0;
            foreach (Guard currState in _guardStates.Skip(1))
            {
                // If guard is turning around or tile already tried
                if (currState.Pos == prevState.Pos || !obstructionTiles.Add(currState.Pos))
                {
                    // Ignore this state
                    prevState = currState;
                    continue;
                } 

                // Place obstacle at current state
                _grid[currState.Pos] = '#';

                // Keep track of previous positions and directions
                HashSet<Guard> visited = [];

                // Create guard from previous state to avoid unnecessary walking
                Guard guard = Walk(prevState);
                while (_grid.Contains(guard.Pos))
                {
                    // Check if guard has looped
                    if (!visited.Add(guard))
                    {
                        // Color loop obstacle red
                        _reporter.ReportStringGridUpdate(currState.Pos, "#FF0000");

                        // Increment loop count
                        loops++;
                        break;
                    }

                    // Walk forward or rotate
                    guard = Walk(guard);
                }

                // Remove obstacle for next state
                _grid[currState.Pos] = '.';

                // Save state for next loop
                prevState = currState;
            }

            return loops;
        }
    }
}