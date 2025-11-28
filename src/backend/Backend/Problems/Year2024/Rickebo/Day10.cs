using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.Rickebo;

public class Day10 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 10);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    private static string[] _colors =
    {
        "#440000",
        "#222222",
        "#333333",
        "#444444",
        "#555555",
        "#666666",
        "#777777",
        "#888888",
        "#999999",
        "#AADDAA"
    };

    public override string Name => "Hoof It";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            var grid = new CharGrid(input);
            ReportGrid(grid, reporter);

            reporter.ReportSolution(
                grid
                    .FindAll(x => x == '0')
                    .Sum(src => Score(grid, src, []))
            );

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            var grid = new CharGrid(input);
            ReportGrid(grid, reporter);

            reporter.ReportSolution(
                grid
                    .FindAll(x => x == '0')
                    .Sum(src => Score(grid, src))
            );

            return Task.CompletedTask;
        }
    }

    private static void ReportGrid(CharGrid grid, Reporter reporter) =>
        reporter.ReportStringGridUpdate(
            grid,
            (builder, coordinate, v) => builder
                .WithCoordinate(coordinate)
                .WithText(v == '.' ? "#000000" : _colors[v - '0'])
        );

    private static int Score(
        CharGrid grid,
        IntegerCoordinate<int> source,
        HashSet<IntegerCoordinate<int>>? visited = null,
        Reporter? reporter = null
    )
    {
        var levelChar = grid[source];

        if (levelChar == '.')
            throw new Exception("Score of invalid level.");

        var level = levelChar - '0';
        if (visited != null && !visited.Add(source))
            return 0;

        reporter?.ReportStringGridUpdate(source, _colors[level]);

        if (level == 9)
            return 1;

        var score = 0;
        foreach (var neighbour in source.Neighbours)
        {
            if (!grid.Contains(neighbour))
                continue;

            var neighbourLevel = grid[neighbour] == '.' ? -1 : grid[neighbour] - '0';
            if (neighbourLevel != level + 1)
                continue;

            score += Score(grid, neighbour, visited);
        }

        return score;
    }
}