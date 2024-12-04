using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024;

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
            Grid grid = new(input);

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = grid.XmasOccurences().ToString()
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
            Grid grid = new(input);

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = grid.MasOccurences().ToString()
                }
            );
            return Task.CompletedTask;
        }
    }

    public class Grid
    {
        public int width;
        public int height;
        public Tile[,] tiles;

        public Grid(string input)
        {
            string[] rows = Parser.SplitBy(input, ["\r\n", "\r", "\n"]);
            height = rows.Length;
            width = rows[0].Length;
            tiles = new Tile[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y] = new Tile(x, y, rows[(height - 1) - y][x]);
                }
            }
        }

        public int XmasOccurences()
        {
            int occurences = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (tiles[x, y].c == 'X')
                    {
                        occurences += TileXmasCount(tiles[x, y]);
                    }
                }
            }

            return occurences;
        }

        public int MasOccurences()
        {
            int occurences = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (tiles[x, y].c == 'A')
                    {
                        occurences += TileMasCross(tiles[x, y]) ? 1 : 0;
                    }
                }
            }

            return occurences;
        }

        public bool TileMasCross(Tile tile)
        {
            bool present1 = false;
            bool present2 = false;

            // Check \
            int X1 = tile.x - 1;
            int Y1 = tile.y + 1;
            int X2 = tile.x + 1;
            int Y2 = tile.y - 1;
            if (X1 >= 0 && X1 < width && Y1 >= 0 && Y1 < height && X2 >= 0 && X2 < width && Y2 >= 0 && Y2 < height)
            {
                if ((tiles[X1, Y1].c == 'M' && tiles[X2, Y2].c == 'S') || (tiles[X1, Y1].c == 'S' && tiles[X2, Y2].c == 'M'))
                {
                    present1 = true;
                }
            }

            // Check /
            X1 = tile.x + 1;
            Y1 = tile.y + 1;
            X2 = tile.x - 1;
            Y2 = tile.y - 1;
            if (X1 >= 0 && X1 < width && Y1 >= 0 && Y1 < height && X2 >= 0 && X2 < width && Y2 >= 0 && Y2 < height)
            {
                if ((tiles[X1, Y1].c == 'M' && tiles[X2, Y2].c == 'S') || (tiles[X1, Y1].c == 'S' && tiles[X2, Y2].c == 'M'))
                {
                    present2 = true;
                }
            }

            return present1 && present2;
        }

        public int TileXmasCount(Tile tile)
        {
            // Check all directions for xmas
            int count = 0;
            int i;
            string xmas = "XMAS";

            // Check right
            i = 1;
            for (int x = tile.x + 1; x < width && i < xmas.Length; x++, i++)
            {
                if (tiles[x, tile.y].c != xmas[i])
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
            for (int x = tile.x - 1; x >= 0 && i < xmas.Length; x--, i++)
            {
                if (tiles[x, tile.y].c != xmas[i])
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
            for (int y = tile.y + 1; y < height && i < xmas.Length; y++, i++)
            {
                if (tiles[tile.x, y].c != xmas[i])
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
            for (int y = tile.y - 1; y >= 0 && i < xmas.Length; y--, i++)
            {
                if (tiles[tile.x, y].c != xmas[i])
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
            for (int y = tile.y + 1, x = tile.x + 1; y < height && x < width && i < xmas.Length; y++, x++, i++)
            {
                if (tiles[x, y].c != xmas[i])
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
            for (int y = tile.y + 1, x = tile.x - 1; y < height && x >= 0 && i < xmas.Length; y++, x--, i++)
            {
                if (tiles[x, y].c != xmas[i])
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
            for (int y = tile.y - 1, x = tile.x + 1; y >= 0 && x < width && i < xmas.Length; y--, x++, i++)
            {
                if (tiles[x, y].c != xmas[i])
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
            for (int y = tile.y - 1, x = tile.x - 1; y >= 0 && x >= 0 && i < xmas.Length; y--, x--, i++)
            {
                if (tiles[x, y].c != xmas[i])
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

    public struct Tile(int _x, int _y, char _c)
    {
        public int x = _x;
        public int y = _y;
        public char c = _c;
    }
}