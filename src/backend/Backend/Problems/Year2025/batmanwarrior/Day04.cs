using Common;
using Common.Updates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lib.Color;
using Lib.Geometry;
using Lib.Grids;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day04 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 04);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Printing Department";

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

        public int PartOne()
        {
            int accessibleRolls = 0;

            // Count accessible paper rolls
            foreach (var coord in _grid.Coordinates)
            {
                if (_grid[coord] == '@' && CountNeighbouringRolls(coord) <= 3)
                {
                    _reporter.ReportGlyphGridUpdate(builder => builder.WithEntry(b => b
                        .WithCoordinate(coord)
                        .WithChar('x')
                        .WithForeground(ColorHex.Red)
                        .WithBackground(ColorHex.Black)
                    ));

                    accessibleRolls++;
                }
            }

            return accessibleRolls;
        }

        public int PartTwo()
        {
            int removedRolls = 0;

            // Add visited paper rolls to collection
            HashSet<IntegerCoordinate<int>> paperRolls = [];
            foreach (var coord in _grid.Coordinates)
            {
                if (_grid[coord] == '@')
                {
                    // Paper roll can be removed
                    if (CountNeighbouringRolls(coord) <= 3)
                    {
                        _grid[coord] = '.';

                        _reporter.ReportGlyphGridUpdate(builder => builder.WithEntry(b => b
                            .WithCoordinate(coord)
                            .WithChar('.')
                            .WithForeground(ColorHex.Red)
                            .WithBackground(ColorHex.Black)
                        ));

                        removedRolls++;
                        continue;
                    }

                    paperRolls.Add(coord);
                }
            }
            
            // Remove as many paper rolls as possible
            bool hasBeenRemoved = true;
            while (hasBeenRemoved)
            {
                hasBeenRemoved = false;
                foreach (var coord in paperRolls)
                {
                    if (CountNeighbouringRolls(coord) <= 3)
                    {
                        _grid[coord] = '.';
                        paperRolls.Remove(coord);
                        hasBeenRemoved = true;

                        _reporter.ReportGlyphGridUpdate(builder => builder.WithEntry(b => b
                            .WithCoordinate(coord)
                            .WithChar('.')
                            .WithForeground(ColorHex.Red)
                            .WithBackground(ColorHex.Black)
                        ));

                        removedRolls++;
                    }
                }
            } 
            
            return removedRolls;
        }

        private int CountNeighbouringRolls(IntegerCoordinate<int> coord)
        {
            var neighbouringRolls = 0;

            foreach (var neighbour in Neighbourhoods.All2DNeighbours(coord))
            {
                if (!_grid.Contains(neighbour))
                    continue;

                if (_grid[neighbour] == '@')
                    neighbouringRolls++;

                if (neighbouringRolls > 3)
                    break;
            }

            return neighbouringRolls;
        }
    }
}
