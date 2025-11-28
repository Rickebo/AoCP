namespace Lib.Grid;

public class CharGrid : ArrayGrid<char>
{
    public CharGrid(string input) : base(ParseFromString(input)) { }

    public CharGrid(char c, int width, int height) : base(Fill(c, width, height)) { }

    public CharGrid(char[,] values) : base(values) { }

    private static char[,] ParseFromString(string str)
    {
        // Parse string input
        var rows = str.SplitLines();
        var height = rows.Length;
        var width = rows[0].Length;

        // Fill 2D-array with chars (Y mapped to bottom left origin)
        var arr = new char[width, height];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                arr[x, y] = rows[^(y + 1)][x];

        return arr;
    }

    private static char[,] Fill(char c, int width, int height)
    {
        // Fill 2D-array with chars
        var arr = new char[width, height];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                arr[x, y] = c;

        return arr;
    }

    public override CharGrid Flip(Axis axis) => Flip(values => new CharGrid(values), axis);
}