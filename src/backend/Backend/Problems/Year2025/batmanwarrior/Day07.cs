using Common;
using Common.Updates;
using Lib.Color;
using Lib.Geometry;
using Lib.Grids;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day07 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 07);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Laboratories";

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
        private readonly CharGrid _grid;

        public Solver(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            _grid = new(input);

            // Print grid
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_grid, ColorHex.White, ColorHex.Black));
        }

        public long PartOne()
        {
            long splits = 0;

            // Queue start
            Queue<IntegerCoordinate<int>> queue = new();
            queue.Enqueue(_grid.Find(c => c == 'S'));

            // Fall until end of grid
            HashSet<IntegerCoordinate<int>> visited = [];
            while (queue.TryDequeue(out var pos))
            {
                // Skip visited
                if (!visited.Add(pos))
                    continue;

                // Print
                if (_grid[pos] != 'S')
                {
                    _reporter.ReportGlyphGridUpdate(builder => builder.WithEntry(b => b
                        .WithCoordinate(pos)
                        .WithChar('|')
                        .WithForeground(ColorHex.Red)
                        .WithBackground(ColorHex.Black)
                    ));
                }

                // Exiting grid
                IntegerCoordinate<int> next = pos.Move(Direction.South);
                if (!_grid.Contains(next))
                    continue;

                // Fall
                if (_grid[next] == '.')
                    queue.Enqueue(next);

                // Split
                else if (_grid[next] == '^')
                {
                    IntegerCoordinate<int> left = pos.Move(Direction.West);
                    if (_grid.Contains(left))
                        queue.Enqueue(left);

                    IntegerCoordinate<int> right = pos.Move(Direction.East);
                    if (_grid.Contains(right))
                        queue.Enqueue(right);

                    splits++;
                }
            }

            return splits;
        }

        public long PartTwo() => 
            Timelines(_grid.Find(s => s == 'S'), []);

        private long Timelines(IntegerCoordinate<int> pos, Dictionary<IntegerCoordinate<int>, long> cache)
        {
            // Outside of grid
            if (!_grid.Contains(pos))
                return 0;

            // Cached
            if (cache.TryGetValue(pos, out long timelines))
                return timelines;

            // Print
            _reporter.ReportGlyphGridUpdate(builder => builder.WithEntry(b => b
                .WithCoordinate(pos)
                .WithChar('|')
                .WithForeground(ColorHex.Red)
                .WithBackground(ColorHex.Black)
            ));

            // Exiting grid
            IntegerCoordinate<int> next = pos.Move(Direction.South);
            if (!_grid.Contains(next))
            {
                cache.Add(pos, 1);
                return 1;
            }

            // Fall
            if (_grid[next] == '.')
            {
                long nextResult = Timelines(next, cache);
                cache.Add(pos, nextResult);
                return nextResult;
            }

            // Split
            if (_grid[next] == '^')
            {
                IntegerCoordinate<int> left = next.Move(Direction.West);
                IntegerCoordinate<int> right = next.Move(Direction.East);
                long timeLines = Timelines(left, cache) + Timelines(right, cache);
                cache.Add(pos, timeLines);
                return timeLines;
            }

            throw new ProblemException("Error.");
        }
    }
}

