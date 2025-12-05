using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib.Grids;
using Lib.Geometry;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day16 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 16);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Reindeer Maze";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create temp
            Maze maze = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(maze.LowestScore(canSit: false)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create temp
            Maze maze = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(maze.LowestScore(canSit: true)));
            return Task.CompletedTask;
        }
    }

    public class Maze
    {
        private record Reindeer(IntegerCoordinate<int> Pos, Direction Dir);
        private record Tracker(Reindeer Reindeer, HashSet<IntegerCoordinate<int>> Path);
        private readonly Reporter _reporter;
        private readonly CharGrid _grid;

        public Maze(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Create grid
            _grid = new(input);

            // Print grid
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_grid, "#FFFFFF", "#000000"));
        }

        public long LowestScore(bool canSit)
        {
            // Create priority queue for traversing
            PriorityQueue<Tracker, long> queue = new();

            // Create start reindeer
            Reindeer startReindeer = new(_grid.Find(ch => ch == 'S'), Direction.East);

            // Queue reindeer tracker
            queue.Enqueue(new(startReindeer, [startReindeer.Pos]), 0);

            // List of lowest cost paths
            List<(HashSet<IntegerCoordinate<int>>, long)> lowcostPaths = [];

            // Dict with minimum costs
            Dictionary<IntegerCoordinate<int>, long> minimumCosts = [];

            // Traverse lowest cost path until goal
            long lowestCost = Int64.MaxValue;
            while (queue.TryDequeue(out Tracker? tracker, out long cost))
            {
                // If this tile has been visited with lower cost, be gone (FIX LATER)
                if (minimumCosts.TryGetValue(tracker.Reindeer.Pos, out long value) && cost - 1001 > value)
                    continue;

                // Store cost for this tile
                minimumCosts[tracker.Reindeer.Pos] = cost;

                // If goal has been reached
                if (_grid[tracker.Reindeer.Pos] == 'E')
                {
                    if (cost < lowestCost)
                    {
                        lowestCost = cost;
                        lowcostPaths = [(tracker.Path, cost)];
                    }
                    else if (cost == lowestCost)
                    {
                        lowcostPaths.Add((tracker.Path, cost));
                    }
                    else
                    {
                        break;
                    }

                    //continue;
                }

                // A reindeer can only move forward or to the sides, otherwise it is backtracking
                Direction rightDir = tracker.Reindeer.Dir.Rotate(Rotation.Clockwise);
                Direction leftDir = tracker.Reindeer.Dir.Rotate(Rotation.CounterClockwise);
                IntegerCoordinate<int> forwardPos = tracker.Reindeer.Pos.Move(tracker.Reindeer.Dir);
                IntegerCoordinate<int> rightPos = tracker.Reindeer.Pos.Move(rightDir);
                IntegerCoordinate<int> leftPos = tracker.Reindeer.Pos.Move(leftDir);

                // Options
                bool forward = _grid[forwardPos] != '#';
                bool right = _grid[rightPos] != '#';
                bool left = _grid[leftPos] != '#';

                if (forward)
                {
                    // Add forward tile in path
                    HashSet<IntegerCoordinate<int>> newPath = [.. tracker.Path,forwardPos];

                    // Keep going with +1 cost
                    queue.Enqueue(new(new(forwardPos, tracker.Reindeer.Dir), newPath), cost + 1);
                }

                if (right)
                {
                    // Add right tile in path
                    HashSet<IntegerCoordinate<int>> newPath = [.. tracker.Path, rightPos];

                    // Keep going with +1001 cost
                    queue.Enqueue(new(new(rightPos, rightDir), newPath), cost + 1001);
                }

                if (left)
                {
                    // Add left tile in path
                    HashSet<IntegerCoordinate<int>> newPath = [.. tracker.Path, leftPos];

                    // Keep going with +1001 cost
                    queue.Enqueue(new(new(leftPos, leftDir), newPath), cost + 1001);
                }
            }

            HashSet<IntegerCoordinate<int>> unique = [];

            foreach (var path in lowcostPaths)
            {
                foreach (var pos in path.Item1)
                {
                    if (unique.Add(pos))
                    {
                        // Print pos in red
                        _reporter.ReportGlyphGridUpdate(
                            builder => builder.WithEntry(
                            b => b
                                    .WithCoordinate(pos)
                                    .WithChar(_grid[pos])
                                    .WithForeground("#FF0000")
                                    .WithBackground("#000000")
                            )
                        );
                    }
                }
            }

            if (!canSit)
                return lowcostPaths[0].Item2;

            return unique.Count;
        }
    }
}

