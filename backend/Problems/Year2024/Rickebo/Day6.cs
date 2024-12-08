using System.Drawing;
using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.Rickebo;

public class Day6 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 06, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Guard Gallivant";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            var grid = Parse(input);
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution =
                        SimulateGuard(grid, FindGuard(grid), reporter)?.Item1.Count
                            .ToString() ?? throw new Exception()
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
            var grid = Parse(input);

            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = CountObstacles(grid, reporter).ToString()
                }
            );

            return Task.CompletedTask;
        }
    }

    private static string ColorCell(
        CharGrid grid,
        IntegerCoordinate<int> coordinate,
        bool isGuard
    )
    {
        if (isGuard) return "#FF0000";
        return grid[coordinate] switch
        {
            '#' => "#444444",
            _ => "#000000"
        };
    }

    private static int CountObstacles(CharGrid grid, Reporter reporter)
    {
        var guard = FindGuard(grid);
        var (visited, path) =
            SimulateGuard(grid, guard, reporter) ?? throw new Exception();

        var obstructions = new HashSet<IntegerCoordinate<int>>();

        for (var i = 1; i < path.Count; i++)
        {
            var pos = path[i].Position;

            var before = grid[pos];
            grid[pos] = '#';

            if (SimulateGuard(grid, guard, null) == null)
            {
                obstructions.Add(pos);
                reporter?.Report(
                    new StringGridUpdate()
                    {
                        Rows = new Dictionary<string, Dictionary<string, string>>()
                        {
                            [(grid.Height - 1 - pos.Y).ToString()] = new()
                            {
                                [pos.X.ToString()] = "#00FF00"
                            }
                        }
                    }
                );
            }

            grid[pos] = before;
        }

        return obstructions.Count;
    }

    private static Tuple<HashSet<IntegerCoordinate<int>>, List<GuardPosition>>?
        SimulateGuard(
            CharGrid grid,
            GuardPosition guardPosition,
            Reporter? reporter
        )
    {
        var pos = guardPosition.Position;
        var dir = guardPosition.Direction;
        grid[pos] = '.';

        var visited = new HashSet<IntegerCoordinate<int>>([pos]);
        var visitedGuards = new HashSet<GuardPosition>();
        var path = new List<GuardPosition>();

        reporter?.ReportStringGridUpdate(
            grid,
            (builder, coordinate, val) => builder
                .WithCoordinate(coordinate)
                .WithText(ColorCell(grid, coordinate, val != '#' && val != '.'))
        );
        
        // reporter?.Report(
        //     new StringGridUpdate()
        //     {
        //         Width = grid.Width,
        //         Height = grid.Height,
        //         Rows = Enumerable.Range(0, grid.Height)
        //             .ToDictionary(
        //                 y => (grid.Height - 1 - y).ToString(),
        //                 y => Enumerable.Range(0, grid.Width)
        //                     .ToDictionary(
        //                         x => x.ToString(),
        //                         x =>
        //                         {
        //                             var cellPos = new IntegerCoordinate<int>(x, y);
        //                             var val = grid[cellPos];
        //                             return ColorCell(
        //                                 grid,
        //                                 cellPos,
        //                                 val != '#' && val != '.'
        //                             );
        //                         }
        //                     )
        //             )
        //     }
        // );

        while (grid.Contains(pos))
        {
            var gp = new GuardPosition(pos, dir);
            visited.Add(pos);
            if (!visitedGuards.Add(gp))
                return null;

            path.Add(gp);
            reporter?.Report(
                new StringGridUpdate()
                {
                    Rows = new Dictionary<string, Dictionary<string, string>>()
                    {
                        [(grid.Height - 1 - pos.Y).ToString()] = new()
                        {
                            [pos.X.ToString()] = ColorCell(grid, pos, true)
                        }
                    }
                }
            );

            var next = pos.Move(dir);

            while (grid.Contains(next) && grid[next] == '#')
            {
                dir = dir.RotateClockwise();
                next = pos.Move(dir);
            }

            pos = next;
        }

        return new Tuple<HashSet<IntegerCoordinate<int>>, List<GuardPosition>>(
            visited,
            path
        );
    }

    private static GuardPosition FindGuard(CharGrid grid)
    {
        const string guardChars = "<>v^";

        var pos = grid.Find(ch => guardChars.Contains(ch.ToString()));
        var dir = DirectionExtensions.Parse(grid[pos]);

        return new GuardPosition(pos, dir);
    }

    private record GuardPosition(IntegerCoordinate<int> Position, Direction Direction);

    private static CharGrid Parse(string input) =>
        Parser.ParseCharGrid(input, OriginPosition.TopLeft);
}