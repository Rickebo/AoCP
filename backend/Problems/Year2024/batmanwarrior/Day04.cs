using Common;
using Common.Updates;
using Lib.Grid;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day04 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 04, 0, 0, 0);

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
            CharGrid grid = new(input);

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = XmasOccurences(grid).ToString()
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
                    Solution = MasOccurences(grid).ToString()
                }
            );
            return Task.CompletedTask;
        }
    }

    public static int XmasOccurences(CharGrid grid)
    {
        int occurences = 0;

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                if (grid[x, y] == 'X')
                {
                    occurences += TileXmasCount(grid, x, y);
                }
            }
        }

        return occurences;
    }

    public static int MasOccurences(CharGrid grid)
    {
        int occurences = 0;

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                if (grid[x, y] == 'A')
                {
                    occurences += TileMasCross(grid, x, y) ? 1 : 0;
                }
            }
        }

        return occurences;
    }

    public static bool TileMasCross(CharGrid grid, int x, int y)
    {
        bool present1 = false;
        bool present2 = false;

        // Check \
        int X1 = x - 1;
        int Y1 = y + 1;
        int X2 = x + 1;
        int Y2 = y - 1;
        if (X1 >= 0 && X1 < grid.Width && Y1 >= 0 && Y1 < grid.Height && X2 >= 0 && X2 < grid.Width && Y2 >= 0 && Y2 < grid.Height)
        {
            if (grid[X1, Y1] == 'M' && grid[X2, Y2] == 'S' || grid[X1, Y1] == 'S' && grid[X2, Y2] == 'M')
            {
                present1 = true;
            }
        }

        // Check /
        X1 = x + 1;
        Y1 = y + 1;
        X2 = x - 1;
        Y2 = y - 1;
        if (X1 >= 0 && X1 < grid.Width && Y1 >= 0 && Y1 < grid.Height && X2 >= 0 && X2 < grid.Width && Y2 >= 0 && Y2 < grid.Height)
        {
            if (grid[X1, Y1] == 'M' && grid[X2, Y2] == 'S' || grid[X1, Y1] == 'S' && grid[X2, Y2] == 'M')
            {
                present2 = true;
            }
        }

        return present1 && present2;
    }

    public static int TileXmasCount(CharGrid grid, int x, int y)
    {
        // Check all directions for xmas
        int count = 0;
        int i;
        string xmas = "XMAS";

        // Check right
        i = 1;
        for (int xx = x + 1; xx < grid.Width && i < xmas.Length; xx++, i++)
        {
            if (grid[xx, y] != xmas[i])
            {
                break;
            }
            else if (i == 3)
            {
                count++;
            }
        }

        // Check left
        i = 1;
        for (int xx = x - 1; xx >= 0 && i < xmas.Length; xx--, i++)
        {
            if (grid[xx, y] != xmas[i])
            {
                break;
            }
            else if (i == 3)
            {
                count++;
            }
        }

        // Check north
        i = 1;
        for (int yy = y + 1; yy < grid.Height && i < xmas.Length; yy++, i++)
        {
            if (grid[x, yy] != xmas[i])
            {
                break;
            }
            else if (i == 3)
            {
                count++;
            }
        }

        // Check south
        i = 1;
        for (int yy = y - 1; yy >= 0 && i < xmas.Length; yy--, i++)
        {
            if (grid[x, yy] != xmas[i])
            {
                break;
            }
            else if (i == 3)
            {
                count++;
            }
        }

        // Check diag top right
        i = 1;
        for (int yy = y + 1, xx = x + 1; yy < grid.Height && xx < grid.Width && i < xmas.Length; yy++, xx++, i++)
        {
            if (grid[xx, yy] != xmas[i])
            {
                break;
            }
            else if (i == 3)
            {
                count++;
            }
        }

        // Check diag top left
        i = 1;
        for (int yy = y + 1, xx = x - 1; yy < grid.Height && xx >= 0 && i < xmas.Length; yy++, xx--, i++)
        {
            if (grid[xx, yy] != xmas[i])
            {
                break;
            }
            else if (i == 3)
            {
                count++;
            }
        }

        // Check diag bot right
        i = 1;
        for (int yy = y - 1, xx = x + 1; yy >= 0 && xx < grid.Width && i < xmas.Length; yy--, xx++, i++)
        {
            if (grid[xx, yy] != xmas[i])
            {
                break;
            }
            else if (i == 3)
            {
                count++;
            }
        }

        // Check diag bot left
        i = 1;
        for (int yy = y - 1, xx = x - 1; yy >= 0 && xx >= 0 && i < xmas.Length; yy--, xx--, i++)
        {
            if (grid[xx, yy] != xmas[i])
            {
                break;
            }
            else if (i == 3)
            {
                count++;
            }
        }

        return count;
    }
}