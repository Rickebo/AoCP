using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Geometry;
using Lib.Geometry;
using Lib.Grids;

namespace Backend.Problems.Year2024.Rickebo;

public class Day04 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 04);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Ceres Search";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = CountOccurrences(Parse(input), "XMAS").ToString()
                }
            );

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = CountMasOccurrences(Parse(input)).ToString()
                }
            );

            return Task.CompletedTask;
        }
    }

    private static CharGrid Parse(string input) => new(input);

    public static int CountOccurrences(ArrayGrid<char> grid, string characters)
    {
        var first = characters[0];
        var directions = new[]
        {
            Direction.North,
            Direction.NorthEast,
            Direction.East,
            Direction.SouthEast,
            Direction.South,
            Direction.SouthWest,
            Direction.West,
            Direction.NorthWest
        };

        var count = 0;
        foreach (var coordinate in grid.Coordinates)
        {
            if (grid[coordinate] != first)
                continue;

            count += directions.Count(direction => IsMatch(grid, characters, coordinate, direction));
        }

        return count;
    }

    public static int CountMasOccurrences(ArrayGrid<char> grid)
    {
        var directions = new[]
        {
            (Direction.NorthEast, Direction.SouthWest),
            (Direction.NorthWest, Direction.SouthEast),
            (Direction.SouthEast, Direction.NorthWest),
            (Direction.SouthWest, Direction.NorthEast),
        };

        var a = "AS";
        var b = "AM";
        var count = 0;
        foreach (var coordinate in grid.Coordinates)
        {
            if (grid[coordinate] != 'A')
                continue;

            var match = true;
            foreach (var (src, dst) in directions)
            {
                if (IsMatch(grid, a, coordinate, src) && IsMatch(grid, b, coordinate, dst) ||
                    IsMatch(grid, b, coordinate, src) && IsMatch(grid, a, coordinate, dst))
                    continue;

                match = false;
            }

            if (match) count++;
        }

        return count;
    }

    private static bool IsMatch(ArrayGrid<char> grid, string characters, IntegerCoordinate<int> source, Direction direction)
    {
        var pos = source;
        foreach (var t in characters)
        {
            if (!grid.Contains(pos) || grid[pos] != t) return false;
            pos = pos.Move(direction);
        }

        return true;
    }
}

