using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Parsing;
using Lib.Coordinate;
using Lib.Extensions;
using Lib.Grid;

namespace Backend.Problems.Year2024.Rickebo;

public class Day14 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 14);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Restroom Redoubt";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            var data = Parse(input);

            var result = data.Simulate(100);
            reporter.ReportSolution(result.QuadrantProduct());
            result.Print(reporter);

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            var data = Parse(input);

            var (steps, easterEggData) = data.FindEasterEgg();
            reporter.ReportSolution(steps);
            easterEggData.Print(reporter);


            return Task.CompletedTask;
        }
    }

    public class Data
    {
        public required Robot[] Robots { get; init; }
        public required IntegerCoordinate<int> Dimensions { get; init; }

        public Data Simulate(int steps) => Clone().Step(steps);

        public Data Clone() => new()
        {
            Robots = Robots.ToArray(),
            Dimensions = Dimensions
        };

        public Data Step(int steps = 1)
        {
            foreach (var robot in Robots)
                robot.Step(steps, Dimensions);

            return this;
        }

        public int QuadrantProduct()
        {
            var middle = Dimensions / 2;
            var directions = new int[(int)DirectionExtensions.Max + 1];

            foreach (var robot in Robots)
            {
                var direction = (robot.Position - middle).Direction();
                directions[(int)direction]++;
            }

            return DirectionExtensions.Diagonals.Aggregate(
                1,
                (current, dir) => current * directions[(int)dir]
            );
        }

        public void Print(Reporter reporter)
        {
            var grid = new ArrayGrid<int>(Dimensions.X, Dimensions.Y);
            foreach (var robot in Robots)
                grid[robot.Position.X, robot.Position.Y]++;

            reporter.Report(
                GlyphGridUpdate.FromGrid(
                    grid,
                    value => new Cell(
                        null,
                        value > 9 ? "+" : value.ToString(),
                        "#FFFFFF",
                        "#000000"
                    )
                )
            );
        }

        public (int, Data) FindEasterEgg()
        {
            var data = Clone();
            var hs = new HashSet<IntegerCoordinate<int>>();

            for (var i = 0; i < int.MaxValue; i++)
            {
                hs.Clear();
                if (data.Robots.Select(r => r.Position).All(hs.Add))
                    return (i, data);

                data.Step(1);
            }

            throw new IndexOutOfRangeException();
        }
    }

    public class Robot(IntegerCoordinate<int> position, IntegerCoordinate<int> velocity)
    {
        public IntegerCoordinate<int> Position { get; private set; } = position;
        public IntegerCoordinate<int> Velocity { get; } = velocity;

        public void Step(int steps, IntegerCoordinate<int> dimensions)
        {
            Position = (Position + Velocity * steps).Modulo(dimensions);
        }
    }


    public static Data Parse(string input)
    {
        var lines = input.SplitLines();
        var robots = new Robot[lines.Length];
        var index = 0;

        var dim = lines.Length < 100
            ? new IntegerCoordinate<int>(11, 7)
            : new IntegerCoordinate<int>(101, 103);

        foreach (var line in lines)
        {
            var values = Parser.GetValues<int>(line);
            robots[index++] = new Robot(
                new IntegerCoordinate<int>(values[0], values[1]),
                new IntegerCoordinate<int>(values[2], values[3])
            );
        }

        return new Data
        {
            Robots = robots,
            Dimensions = dim
        };
    }
}