using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day06 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 06, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Guard Gallivant";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            CharGrid grid = new(input);

            MarkWalk(grid, reporter);

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = WalkedTiles(grid, reporter).ToString()
                }
            );
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            CharGrid grid = new(input);

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = LoopCount(grid, reporter).ToString()
                }
            );
            return Task.CompletedTask;
        }
    }

    public static Guard GetGuard(CharGrid grid, Reporter reporter)
    {
        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                foreach (Direction dir in DirectionExtensions.Cardinals)
                {
                    if (grid[x, y] == dir.Arrow())
                    {
                        return new Guard(new Coordinate<int>(x, y), dir);
                    }
                }
            }
        }
        throw new NotImplementedException();
    }

    public static Guard Walk(Guard guard, CharGrid grid, Reporter reporter)
    {
        // Look ahead
        Coordinate<int> next = guard.pos + Coordinate<int>.UnitY;
        switch (guard.dir)
        {
            case Direction.East:
                next = guard.pos + Coordinate<int>.UnitX;
                break;
            case Direction.South:
                next = guard.pos - Coordinate<int>.UnitY;
                break;
            case Direction.West:
                next = guard.pos - Coordinate<int>.UnitX;
                break;
        }

        // Check if next step is an obstacle
        if (next.X >= 0 && next.X < grid.Width && next.Y >= 0 && next.Y < grid.Height && grid[next.X, next.Y] == '#')
        {
            // Rotate clockwise
            return new Guard(guard.pos, guard.dir.RotateClockwise());
        }
        else
        {
            // Keep walking
            return new Guard(next, guard.dir);
        }
    }

    public static void MarkWalk(CharGrid grid, Reporter reporter)
    {
        Guard guard = GetGuard(grid, reporter);
        Coordinate<int> currPos = guard.pos;

        Direction dir = Direction.North;
        switch (grid[guard.pos.X, guard.pos.Y])
        {
            case '>':
                dir = Direction.East;
                break;
            case 'v':
                dir = Direction.South;
                break;
            case '<':
                dir = Direction.West;
                break;
        }

        grid[guard.pos.X, guard.pos.Y] = 'X';

        for (; ; )
        {
            // Look ahead
            Coordinate<int> next = currPos + Coordinate<int>.UnitY;
            switch (dir)
            {
                case Direction.East:
                    next = currPos + Coordinate<int>.UnitX;
                    break;
                case Direction.South:
                    next = currPos - Coordinate<int>.UnitY;
                    break;
                case Direction.West:
                    next = currPos - Coordinate<int>.UnitX;
                    break;
            }

            // Check if next step is outside grid
            if (next.X < 0 || next.X >= grid.Width || next.Y < 0 || next.Y >= grid.Height)
            {
                return;
            }

            // Check if next step is an obstacle
            if (grid[next.X, next.Y] == '#')
            {
                dir = dir.RotateClockwise();
            }
            else
            {
                // Mark next tile as walked
                grid[next.X, next.Y] = 'X';

                // Update current position
                currPos = next;
            }
        }
    }

    public static int WalkedTiles(CharGrid grid, Reporter reporter)
    {
        int count = 0;
        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                if (grid[x, y] == 'X')
                {
                    count++;
                }
            }
        }
        return count;
    }

    public static int LoopCount(CharGrid grid, Reporter reporter)
    {
        int loops = 0;
        Guard guard = GetGuard(grid, reporter);
        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                if (grid[x, y] == '.')
                {
                    grid[x, y] = '#';
                    if (LoopCheck(grid, guard, reporter))
                    {
                        loops++;
                    }
                    grid[x, y] = '.';
                }
            }
        }
        return loops;
    }

    public static bool LoopCheck(CharGrid grid, Guard guard, Reporter reporter)
    {
        HashSet<Guard> visited = [];

        for (; ; )
        {
            if (guard.pos.X < 0 || guard.pos.X >= grid.Width || guard.pos.Y < 0 || guard.pos.Y >= grid.Height)
            {
                return false;
            }
            else if (!visited.Add(guard))
            {
                return true;
            }

            guard = Walk(guard, grid, reporter);
        }
    }

    public record Guard(Coordinate<int> pos, Direction dir);
}