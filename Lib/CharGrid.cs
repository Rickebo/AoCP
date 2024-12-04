namespace Lib;

public class CharGrid(string input) : ArrayGrid<char>(Init(input))
{
    private static char[,] Init(string str)
    {
        // Parse string input
        string[] rows = Parser.SplitBy(str, ["\r\n", "\r", "\n"]);
        int height = rows.Length;
        int width = rows[0].Length;

        // Init array of chars
        char[,] arr = new char[width, height];

        // Fill 2D-array with chars
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                arr[x, y] = rows[height - 1 - y][x];
            }
        }

        return arr;
    }
}
