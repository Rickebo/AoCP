using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib.Grid;
using Lib.Coordinate;
using Lib;
using Lib.Extensions;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day18 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 18);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "RAM Run";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create memorySpace
            MemorySpace memorySpace = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(memorySpace.Run(fallenBytes: 1024)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create memorySpace
            MemorySpace memorySpace = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(memorySpace.RunUntilTrapped(fallenBytes: 1024)));
            return Task.CompletedTask;
        }
    }

    public class MemorySpace
    {
        private readonly Reporter _reporter;
        private readonly CharGrid _grid;
        private readonly List<IntegerCoordinate<int>> _fallingBytes = [];

        public MemorySpace(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Save falling bytes
            int width = 0;
            int height = 0;
            foreach (string row in input.SplitLines())
            {
                // Get X & Y
                int[] coords = Parser.GetValues<int>(row);

                // Add to list of falling bytes
                _fallingBytes.Add(new(coords[0], coords[1]));

                // Store size of space
                width = Math.Max(width, coords[0] + 1);
                height = Math.Max(height, coords[1] + 1);
            }

            // Create empty space
            _grid = new('.', width, height);

            // Print grid
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_grid, "#FFFFFF", "#000000"));
        }

        public int Run(int fallenBytes)
        {
            // Drop bytes on space
            for (int i = 0; i < fallenBytes; i++)
                _grid[_fallingBytes[i]] = '#';

            // Print grid
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_grid, "#FFFFFF", "#000000"));

            // Create queue
            PriorityQueue<IntegerCoordinate<int>, int> queue = new();

            // Enqueue start pos
            queue.Enqueue(new(0, 0), 0);

            // Store visited tiles
            HashSet<IntegerCoordinate<int>> visited = [];

            // Traverse
            while (queue.TryDequeue(out IntegerCoordinate<int> pos, out int cost))
            {
                // If tile has been visited before
                if (!visited.Add(pos))
                    continue;

                // Check if end reached
                if (pos.X == _grid.Width - 1 && pos.Y == _grid.Height - 1)
                    return cost;

                // Check neighbours
                foreach (var neighbour in pos.Neighbours)
                {
                    // If not a corrupted byte
                    if (_grid.Contains(neighbour) && _grid[neighbour] != '#')
                    {
                        // Queue next
                        queue.Enqueue(neighbour, cost + 1);
                    }
                }
            }

            // If end not found (should not happen)
            return 0;
        }

        public string RunUntilTrapped(int fallenBytes)
        {
            // Drop initial bytes on space
            for (int i = 0; i < fallenBytes; i++)
                _grid[_fallingBytes[i]] = '#';

            // Create queue
            PriorityQueue<IntegerCoordinate<int>, int> queue = new();

            // Store visited tiles
            HashSet<IntegerCoordinate<int>> visited = [];

            for (int i = fallenBytes; i < _fallingBytes.Count; i++)
            {
                // Drop one byte at the time
                _grid[_fallingBytes[i]] = '#';

                // Clear queue
                queue.Clear();

                // Enqueue start pos
                queue.Enqueue(new(0, 0), 0);

                // Clear visited tiles
                visited.Clear();

                // Traverse
                bool exitFound = false;
                while (queue.TryDequeue(out IntegerCoordinate<int> pos, out int cost))
                {
                    // If tile has been visited before
                    if (!visited.Add(pos))
                        continue;

                    // Check if end reached
                    if (pos.X == _grid.Width - 1 && pos.Y == _grid.Height - 1)
                    {
                        exitFound = true;
                        break;
                    }

                    // Check neighbours
                    foreach (var neighbour in pos.Neighbours)
                    {
                        // If not a corrupted byte
                        if (_grid.Contains(neighbour) && _grid[neighbour] != '#')
                        {
                            // Queue next
                            queue.Enqueue(neighbour, cost + 1);
                        }
                    }
                }

                // If exit not found anymore
                if (!exitFound)
                    return $"{_fallingBytes[i].X},{_fallingBytes[i].Y}";
            }

            // All the bytes has fallen but exit still found (should not happen)
            return "0,0";
        }
    }
}