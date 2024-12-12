using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day04 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 04);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Ceres Search";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create WordSearch
            WordSearch wordSearch = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(wordSearch.XmasCount()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create WordSearch
            WordSearch wordSearch = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(wordSearch.MasCount()));
            return Task.CompletedTask;
        }
    }

    public class WordSearch
    {
        private readonly Reporter _reporter;
        private readonly CharGrid _grid;
        private const string _find = "MAS";

        public WordSearch(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Create grid from input
            _grid = new(input);

            // Print grid
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_grid, "#FFFFFF", "#000000"));
        }

        public int XmasCount()
        {
            // Loop through all 'X' characters
            int count = 0;
            foreach (IntegerCoordinate<int> pos in _grid.FindAll(c => c == 'X'))
            {
                // Print X in red
                _reporter.ReportGlyphGridUpdate(
                    builder => builder.WithEntry(
                        b => b
                            .WithCoordinate(pos)
                            .WithChar('X')
                            .WithForeground("#FF0000")
                    )
                );

                // Look for "MAS" in every direction
                foreach (Direction dir in DirectionExtensions.All())
                {
                    // Check values in direction
                    int matches = 0;
                    foreach (char c in _grid.RetrieveDirection(pos.Move(dir), dir, _find.Length))
                    {
                        // Keep track of matches
                        if (c == _find[matches])
                        {
                            matches++;
                        }
                            
                        else break;
                    }

                    // Check if all characters were matched
                    if (matches == _find.Length)
                        count++;
                }
            }

            return count;
        }

        public int MasCount()
        {
            // Loop through all 'A' characters
            int count = 0;
            foreach (IntegerCoordinate<int> pos in _grid.FindAll(c => c == 'A'))
            {
                // Skip edge positions
                if (pos.X == 0 || pos.X == (_grid.Width - 1) || pos.Y == 0 || pos.Y == (_grid.Height - 1))
                    continue;

                // Print A in red
                _reporter.ReportGlyphGridUpdate(
                    builder => builder.WithEntry(
                        b => b
                            .WithCoordinate(pos)
                            .WithChar('A')
                            .WithForeground("#FF0000")
                    )
                );

                // Positions to check
                List<IntegerCoordinate<int>[]> posDiagonals = [
                    [pos.Move(Direction.NorthWest), pos.Move(Direction.SouthEast)], // '\'
                    [pos.Move(Direction.NorthEast), pos.Move(Direction.SouthWest)]  // '/'
                ];

                // Check if diagonals contain the right characters
                int validDiagonals = 0;
                foreach (IntegerCoordinate<int>[] posArr in posDiagonals)
                {
                    // Check characters
                    if ((_grid[posArr[0]] == 'M' && _grid[posArr[1]] == 'S') || (_grid[posArr[0]] == 'S' && _grid[posArr[1]] == 'M'))
                        validDiagonals++;
                    else
                        break;
                }

                // Check if both diagonals were valid
                if (validDiagonals == 2)
                    count++;
            }

            return count;
        }
    }
}