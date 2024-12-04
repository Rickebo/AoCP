namespace Lib;

public class IntGrid(string input) : ArrayGrid<int>(Init(input))
{
    private static int[,] Init(string str)
    {
        // Parse string input
        string[] rows = Parser.SplitBy(str, ["\r\n", "\r", "\n"]);
        int height = rows.Length;
        int width = rows[0].Length;

        // Init array of ints
        int[,] arr = new int[width, height];

        // Fill 2D-array with ints
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                arr[x, y] = rows[height - 1 - y][x] - '0';
            }
        }

        return arr;
    }
}
