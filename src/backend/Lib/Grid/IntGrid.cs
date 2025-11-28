namespace Lib.Grid;

public class IntGrid : ArrayGrid<int>
{
    public IntGrid(string input, int? defaultValue = null)
        : base(ParseFromString(input, defaultValue)) {}

    public IntGrid(int num, int width, int height)
        : base(Fill(num, width, height)) {}

    public IntGrid(int[,] values)
        : base(values) {}

    private static int[,] ParseFromString(string str, int? defaultValue)
    {
        // Parse string input
        var rows = str.SplitLines();
        var height = rows.Length;
        var width = rows[0].Length;

        // Fill 2D-array with ints (Y mapped to bottom left origin)
        var arr = new int[width, height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var c = rows[^(y + 1)][x];
                arr[x, y] = char.IsDigit(c) ? c - '0' : defaultValue ??
                    throw new ArgumentException(
                        "Cannot parse grid containing non-numeric values without a default value specified."
                    );
            }
        }

        return arr;
    }

    private static int[,] Fill(int num, int width, int height)
    {
        // Fill 2D-array with ints
        var arr = new int[width, height];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                arr[x, y] = num;

        return arr;
    }

    public override IntGrid Flip(Axis axis) => Flip(values => new IntGrid(values), axis);
}