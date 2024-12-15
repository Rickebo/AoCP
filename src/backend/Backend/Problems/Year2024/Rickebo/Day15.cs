using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.Rickebo;

public class Day15 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 15);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Warehouse Woes";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            var data = Data.Parse(input);

            reporter.ReportSolution("?");

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            var data = Data.Parse(input);

            reporter.ReportSolution("?");
            
            return Task.CompletedTask;
        }
    }

    private class Data(CharGrid grid, string movement, IntegerCoordinate<int> robot)
    {
        public static Data Parse(string input)
        {
            var lines = input.Split(
                ["\r\n", "\r", "\n"],
                StringSplitOptions.TrimEntries
            );

            var empty = Array.IndexOf(lines, "");
            var grid = Parser.ParseCharGrid(string.Join("\n", lines.Take(empty)));
            var movement = string.Join("", lines.Skip(empty));
            var robot = grid.Find(c => c == '@');

            return new Data(grid, movement, robot);
        }

        public void Step(int steps, int start = 0)
        {
            for (var i = start; i < steps; i++)
            {
                var step = movement[i];
                var direction = DirectionExtensions.Parse(movement[step]);
            }
        }
    }
}