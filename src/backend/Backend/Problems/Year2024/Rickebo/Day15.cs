using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Lib.Geometry;
using Lib.Grids;
using Lib.Color;

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
            var data = Data.Parse(input, false);
            data.Step(reporter: reporter);

            reporter.ReportSolution(data.CoordinateSum());

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            var data = Data.Parse(input, true);
            data.Step(reporter: reporter);

            reporter.ReportSolution(data.CoordinateSum());

            return Task.CompletedTask;
        }
    }

    private class Data
    {
        private CharGrid Grid { get; }
        private string Movement { get; }
        private IntegerCoordinate<int> Robot { get; set; }

        private Data(CharGrid grid, string movement, IntegerCoordinate<int> robot)
        {
            Grid = grid;
            Movement = movement;
            Robot = robot;
        }

        public static Data Parse(string input, bool part2)
        {
            var lines = input.Split(
                ["\r\n", "\r", "\n"],
                StringSplitOptions.TrimEntries
            );

            var empty = Array.IndexOf(lines, "");
            var gridLines = lines.Take(empty).ToArray();

            if (part2)
            {
                var sb = new StringBuilder();
                for (var y = 0; y < gridLines.Length; y++)
                {
                    var line = gridLines[y];
                    for (var x = 0; x < gridLines[y].Length; x++)
                    {
                        sb.Append(
                            line[x] switch
                            {
                                '@' => "@.",
                                'O' => "[]",
                                _ => new string(line[x], 2)
                            }
                        );
                    }

                    sb.AppendLine();
                }

                gridLines = sb.ToString().SplitLines();
            }

            var grid = new CharGrid(string.Join("\n", gridLines));
            var movement = string.Join("", lines.Skip(empty));
            var robot = grid.Find(c => c == '@');

            return new Data(grid, movement, robot);
        }

        public void Step(int steps = -1, int start = 0, Reporter? reporter = null)
        {
            if (steps < 0)
                steps = Movement.Length;

            for (var i = start; i < steps; i++)
                TryPush(Robot, DirectionExtensions.Parse(Movement[i]));

            reporter?.ReportGlyphGridUpdate(
                Grid,
                (builder, pos, ch) => builder
                    .WithCoordinate(pos)
                    .WithChar(ch)
                    .WithForeground(Color.White)
                    .WithBackground(Color.Black)
            );
        }

        private bool CanPush(
            IntegerCoordinate<int> pos,
            Direction direction,
            int depth,
            Lib.Collections.PriorityQueue<IntegerCoordinate<int>, int> steps,
            HashSet<IntegerCoordinate<int>>? visited = null
        )
        {
            visited ??= [];
            if (!visited.Add(pos))
                return true;
            
            if (Grid[pos] == '.')
                return true;

            var next = pos.Move(direction);
            if (!Grid.Contains(next) || Grid[next] == '#')
                return false;

            if (!CanPush(next, direction, depth + 1, steps, visited))
                return false;

            if ((direction & (Direction.North | Direction.South)) != 0)
                switch (Grid[next])
                {
                    case '[':
                        if (!CanPush(next.Move(Direction.East), direction, depth + 1, steps, visited))
                            return false;

                        break;
                    case ']':
                        if (!CanPush(next.Move(Direction.West), direction, depth + 1, steps, visited))
                            return false;

                        break;
                }

            steps.Enqueue(pos, -depth);
            return true;
        }

        private void TryPush(IntegerCoordinate<int> pos, Direction direction)
        {
            var pushQueue = new Lib.Collections.PriorityQueue<IntegerCoordinate<int>, int>();
            if (!CanPush(pos, direction, 0, pushQueue))
                return;

            while (pushQueue.TryDequeue(out var push, out _))
            {
                var next = push.Move(direction);

                Grid[next] = Grid[push];
                Grid[push] = '.';

                if (Grid[next] == '@')
                    Robot = next;
            }
        }

        public int CoordinateSum() => Grid
            .Coordinates
            .Where(coordinate => "O[".Contains(Grid[coordinate]))
            .Sum(coordinate => 100 * (Grid.Height - coordinate.Y - 1) + coordinate.X);
    }
}

