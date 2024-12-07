using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2023;

public class Day10 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2023, 12, 10, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Pipe Maze";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public async override Task Solve(string input, Reporter reporter)
        {
            var rnd = Random.Shared;
            for (var i = 0; i < 100; i++)
            {
                Thread.Sleep(30);
                var x = rnd.Next(100);
                var y = rnd.Next(100);

                reporter.Report(
                    new StringGridUpdate
                    {
                        Rows = new Dictionary<string, Dictionary<string, string>>()
                        {
                            [y.ToString()] = new()
                            {
                                [x.ToString()] = "#FFFFFF"
                            }
                        }
                    }
                );
            }

            var graph = Graph.Parse(
                input.Split(["\r\n", "\r", "\n"], StringSplitOptions.None)
            );

            reporter.ReportGlyphGridUpdate(
                graph.Grid,
                (builder, coordinate, glyph) => builder
                    .WithCoordinate(coordinate)
                    .WithGlyph(glyph.ToString())
            );

            var result = graph
                .BreadthFirstSearch()
                .ToString();

            reporter.Report(
                FinishedProblemUpdate.FromSolution(result)
            );
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public async override Task Solve(string input, Reporter reporter)
        {
            var rnd = Random.Shared;
            for (var i = 0; i < 100; i++)
            {
                Thread.Sleep(30);
                var x = rnd.Next(100);
                var y = rnd.Next(100);

                reporter.ReportStringGridUpdate(
                    new IntegerCoordinate<int>(x, y),
                    "#FFFFFF"
                );
            }

            var result = Graph
                .Parse(input.Split(["\r\n", "\r", "\n"], StringSplitOptions.None))
                .CountWithinLoop()
                .ToString();

            reporter.Report(FinishedProblemUpdate.FromSolution(result));
        }
    }

    public class Graph
    {
        public CharGrid Grid { get; init; }
        public IntegerCoordinate<int> Source { get; init; }
        public Direction SourceDirection { get; private set; }

        public static Graph Parse(string[] lines)
        {
            var grid = Parser.ParseCharGrid(string.Join("\n", lines));
            var source = grid.Find(c => c == 'S');

            var graph = new Graph()
            {
                Grid = grid,
                Source = source
            };

            graph.SourceDirection = graph.DeduceNodeDirection(graph.Source);
            return graph;
        }

        public Direction DeduceNodeDirection(IntegerCoordinate<int> coordinate)
        {
            var offsets = new[]
            {
                (Direction.West, Direction.East, new IntegerCoordinate<int>(-1, 0)),
                (Direction.North, Direction.South, new IntegerCoordinate<int>(0, -1)),
                (Direction.East, Direction.West, new IntegerCoordinate<int>(1, 0)),
                (Direction.South, Direction.North, new IntegerCoordinate<int>(0, 1)),
            };

            var direction = Direction.None;
            foreach (var offset in offsets)
            {
                var absolute = coordinate + offset.Item3;
                if (!Contains(absolute))
                    continue;

                if ((GetDirection(absolute) & offset.Item2) == 0)
                    continue;

                direction |= offset.Item1;
            }

            return direction;
        }

        public Direction GetDirection(IntegerCoordinate<int> coordinate) =>
            coordinate == Source
                ? SourceDirection
                : DirectionExtensions.Parse(Grid[coordinate]);

        public IEnumerable<IntegerCoordinate<int>> GetNeighbours(
            IntegerCoordinate<int> coordinate,
            Direction? overrideDirection = null
        )
        {
            var x = coordinate.X;
            var y = coordinate.Y;

            var node = Grid[y, x];
            var direction = overrideDirection ?? GetDirection(coordinate);

            if ((direction & Direction.West) != 0 && x > 0)
                yield return new IntegerCoordinate<int>(x - 1, y);

            if ((direction & Direction.North) != 0 && y > 0)
                yield return new IntegerCoordinate<int>(x, y - 1);

            if ((direction & Direction.East) != 0 && x + 1 < Grid.Width)
                yield return new IntegerCoordinate<int>(x + 1, y);

            if ((direction & Direction.South) != 0 && y + 1 < Grid.Height)
                yield return new IntegerCoordinate<int>(x, y + 1);
        }

        public int BreadthFirstSearch(HashSet<IntegerCoordinate<int>>? visited = null) =>
            BreadthFirstSearch(Source, visited);

        public int BreadthFirstSearch(
            IntegerCoordinate<int> source,
            HashSet<IntegerCoordinate<int>>? visited = null
        )
        {
            var frontier = new Queue<(IntegerCoordinate<int>, int)>();
            visited ??= new HashSet<IntegerCoordinate<int>>();

            frontier.Enqueue((source, 0));
            var maxDepth = 0;

            while (frontier.TryDequeue(out var pair))
            {
                var current = pair.Item1;
                var currentDepth = pair.Item2;

                if (maxDepth < currentDepth)
                    maxDepth = currentDepth;

                if (visited.Contains(current))
                    continue;

                visited.Add(current);

                foreach (var neighbour in GetNeighbours(current))
                {
                    if (visited.Contains(neighbour))
                        continue;

                    visited.Add(current);
                    frontier.Enqueue((neighbour, currentDepth + 1));
                }
            }

            return maxDepth;
        }

        public bool[,] MaskAccessible(
            IntegerCoordinate<int> source,
            HashSet<IntegerCoordinate<int>> visited
        )
        {
            var mask = new bool[Grid.Width, Grid.Height];
            foreach (var node in visited)
                mask[node.Y, node.X] = true;

            return mask;
        }

        private bool Contains(IntegerCoordinate<int> coordinate) =>
            coordinate.X >= 0 && coordinate.X < Grid.Width &&
            coordinate.Y >= 0 && coordinate.Y < Grid.Height;

        private bool IsInsidePolygon(
            IntegerCoordinate<int> source,
            HashSet<IntegerCoordinate<int>> visited
        )
        {
            var crossings = 0;
            var direction = new IntegerCoordinate<int>(1, 0);
            var ignoredDirection = Direction.West | Direction.East;
            var wasOn = visited.Contains(source);

            if (wasOn)
                return true;

            var entry = Direction.None;
            var exit = Direction.None;
            var upDown = Direction.North | Direction.South;

            for (var current = source; Contains(current); current += direction)
            {
                var currentDir = GetDirection(current);
                var isOn = visited.Contains(current);

                if (!isOn)
                {
                    entry = Direction.None;
                    exit = Direction.None;
                    continue;
                }

                if (currentDir == ignoredDirection)
                    continue;

                if ((currentDir & Direction.East) != 0)
                    entry = currentDir;
                else
                {
                    exit = currentDir;

                    if (((entry | exit) & upDown) == upDown)
                        crossings++;
                }
            }

            return (crossings & 1) == 1;
        }

        public int CountWithinLoop() => CountWithinLoop(Source);

        public int CountWithinLoop(IntegerCoordinate<int> source)
        {
            var visited = new HashSet<IntegerCoordinate<int>>();
            BreadthFirstSearch(source, visited);
            var count = 0;

            for (var y = 0; y < Grid.Height; y++)
            {
                for (var x = 0; x < Grid.Width; x++)
                {
                    var coordinate = new IntegerCoordinate<int>(x, y);
                    if (visited.Contains(coordinate))
                        continue;

                    if (IsInsidePolygon(coordinate, visited))
                        count++;
                }
            }

            return count;
        }
    }
}