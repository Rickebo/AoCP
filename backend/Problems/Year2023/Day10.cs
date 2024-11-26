using Backend.Problems.Updates;

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

        public async override IAsyncEnumerable<ProblemUpdate> Solve(string input)
        {
            var rnd = Random.Shared;
            for (var i = 0; i < 100; i++)
            {
                Thread.Sleep(30);
                var x = rnd.Next(100);
                var y = rnd.Next(100);

                yield return new GridUpdate
                {
                    Rows = new Dictionary<string, Dictionary<string, string>>()
                    {
                        [y.ToString()] = new()
                        {
                            [x.ToString()] = "#FFFFFF"
                        }
                    }
                };
            }
            
            var result = Graph
                .Parse(input.Split(["\r\n", "\r", "\n"], StringSplitOptions.None))
                .BreadthFirstSearch()
                .ToString();

            yield return FinishedProblemUpdate.FromSolution(result);
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public async override IAsyncEnumerable<ProblemUpdate> Solve(string input)
        {
            var rnd = Random.Shared;
            for (var i = 0; i < 100; i++)
            {
                Thread.Sleep(30);
                var x = rnd.Next(100);
                var y = rnd.Next(100);

                yield return new GridUpdate
                {
                    Rows = new Dictionary<string, Dictionary<string, string>>()
                    {
                        [y.ToString()] = new()
                        {
                            [x.ToString()] = "#FFFFFF"
                        }
                    }
                };
            }
            
            var result = Graph
                .Parse(input.Split(["\r\n", "\r", "\n"], StringSplitOptions.None))
                .CountWithinLoop()
                .ToString();

            yield return FinishedProblemUpdate.FromSolution(result);
        }
    }

    public class Graph
    {
        public char[,] Grid { get; init; }
        public Coordinate Source { get; init; }
        public NodeDirection SourceDirection { get; private set; }

        public static Graph Parse(string[] lines)
        {
            var grid = new char[lines.Length, lines[0].Length];
            var source = default(Coordinate?);

            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (var x = 0; x < line.Length; x++)
                {
                    var ch = line[x];
                    grid[y, x] = ch;

                    if (ch == 'S')
                        source = new Coordinate(x, y);
                }
            }

            var graph = new Graph()
            {
                Grid = grid,
                Source = source ??
                         throw new InvalidOperationException(
                             "Grid contains no source node."
                         ),
            };

            graph.SourceDirection = graph.DeduceNodeDirection(graph.Source);
            return graph;
        }

        public NodeDirection DeduceNodeDirection(Coordinate coordinate)
        {
            var offsets = new[]
            {
                (NodeDirection.Left, NodeDirection.Right, new Coordinate(-1, 0)),
                (NodeDirection.Top, NodeDirection.Bottom, new Coordinate(0, -1)),
                (NodeDirection.Right, NodeDirection.Left, new Coordinate(1, 0)),
                (NodeDirection.Bottom, NodeDirection.Top, new Coordinate(0, 1)),
            };

            var direction = NodeDirection.None;
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

        public NodeDirection GetDirection(Coordinate coordinate)
        {
            if (coordinate == Source)
                return SourceDirection;

            return Grid[coordinate.Y, coordinate.X] switch
            {
                '|' => NodeDirection.Bottom | NodeDirection.Top,
                '-' => NodeDirection.Left | NodeDirection.Right,
                'L' => NodeDirection.Top | NodeDirection.Right,
                'J' => NodeDirection.Left | NodeDirection.Top,
                '7' => NodeDirection.Left | NodeDirection.Bottom,
                'F' => NodeDirection.Bottom | NodeDirection.Right,
                'S' => NodeDirection.All,
                _ => NodeDirection.None
            };
        }

        public IEnumerable<Coordinate> GetNeighbours(
            Coordinate coordinate,
            NodeDirection? overrideDirection = null
        )
        {
            var x = coordinate.X;
            var y = coordinate.Y;

            var node = Grid[y, x];
            var direction = overrideDirection ?? GetDirection(coordinate);

            if ((direction & NodeDirection.Left) != 0 && x > 0)
                yield return new Coordinate(x - 1, y);

            if ((direction & NodeDirection.Top) != 0 && y > 0)
                yield return new Coordinate(x, y - 1);

            if ((direction & NodeDirection.Right) != 0 && x + 1 < Grid.GetLength(1))
                yield return new Coordinate(x + 1, y);

            if ((direction & NodeDirection.Bottom) != 0 && y + 1 < Grid.GetLength(0))
                yield return new Coordinate(x, y + 1);
        }

        public int BreadthFirstSearch(HashSet<Coordinate>? visited = null) =>
            BreadthFirstSearch(Source, visited);

        public int BreadthFirstSearch(
            Coordinate source,
            HashSet<Coordinate>? visited = null
        )
        {
            var frontier = new Queue<(Coordinate, int)>();
            visited ??= new HashSet<Coordinate>();

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

        public bool[,] MaskAccessible(Coordinate source, HashSet<Coordinate> visited)
        {
            var mask = new bool[Grid.GetLength(0), Grid.GetLength(1)];
            foreach (var node in visited)
                mask[node.Y, node.X] = true;

            return mask;
        }

        private bool Contains(Coordinate coordinate) =>
            coordinate.X >= 0 && coordinate.X < Grid.GetLength(1) &&
            coordinate.Y >= 0 && coordinate.Y < Grid.GetLength(0);

        private bool IsInsidePolygon(Coordinate source, HashSet<Coordinate> visited)
        {
            var crossings = 0;
            var direction = new Coordinate(1, 0);
            var ignoredDirection = NodeDirection.Left | NodeDirection.Right;
            var wasOn = visited.Contains(source);

            if (wasOn)
                return true;

            var entry = NodeDirection.None;
            var exit = NodeDirection.None;
            var upDown = NodeDirection.Top | NodeDirection.Bottom;

            for (var current = source; Contains(current); current += direction)
            {
                var currentDir = GetDirection(current);
                var isOn = visited.Contains(current);

                if (!isOn)
                {
                    entry = NodeDirection.None;
                    exit = NodeDirection.None;
                    continue;
                }

                if (currentDir == ignoredDirection)
                    continue;

                if ((currentDir & NodeDirection.Right) != 0)
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

        public int CountWithinLoop(Coordinate source)
        {
            var visited = new HashSet<Coordinate>();
            BreadthFirstSearch(source, visited);
            var count = 0;

            for (var y = 0; y < Grid.GetLength(0); y++)
            {
                for (var x = 0; x < Grid.GetLength(1); x++)
                {
                    var coordinate = new Coordinate(x, y);
                    if (visited.Contains(coordinate))
                        continue;

                    if (IsInsidePolygon(coordinate, visited))
                        count++;
                }
            }

            return count;
        }
    }

    [Flags]
    public enum NodeDirection
    {
        None = 0,
        Left = 1,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3,
        All = 0b1111
    }

    public readonly struct Coordinate
    {
        public int X { get; }
        public int Y { get; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj) =>
            obj is Coordinate other && X == other.X && Y == other.Y;

        public override int GetHashCode() =>
            HashCode.Combine(X, Y);

        public static bool operator ==(Coordinate a, Coordinate b) => a.Equals(b);
        public static bool operator !=(Coordinate a, Coordinate b) => !(a == b);

        public static Coordinate operator +(Coordinate a, Coordinate b) =>
            new(a.X + b.X, a.Y + b.Y);
    }
}