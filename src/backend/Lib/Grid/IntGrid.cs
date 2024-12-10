namespace Lib.Grid;

public class IntGrid : ArrayGrid<int>
{
    public IntGrid(
        string input,
        int? defaultValue = null,
        OriginPosition originPosition = OriginPosition.BottomLeft
    ) : base(ParseFromString(input, defaultValue, originPosition))
    {
    }

    public IntGrid(int num, int width, int height) : base(Fill(num, width, height))
    {
    }

    public IntGrid(int[,] values) : base(values)
    {
    }

    private static int[,] ParseFromString(
        string str,
        int? defaultValue = null,
        OriginPosition originPosition = OriginPosition.BottomLeft
    )
    {
        // Parse string input
        var rows = str.SplitLines();
        var height = rows.Length;
        var width = rows[0].Length;

        // Init array of ints
        var arr = new int[width, height];

        // Fill 2D-array with ints
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var mappedY = originPosition == OriginPosition.BottomLeft ? height - 1 - y : y;
                arr[x, y] = char.IsDigit(rows[mappedY][x])
                    ? rows[mappedY][x] - '0'
                    : defaultValue ??
                      throw new ArgumentException(
                          "Cannot parse grid containing non-numeric values without a default value specified."
                      );
            }
        }

        return arr;
    }

    private static int[,] Fill(int num, int width, int height)
    {
        // Init array of ints
        var arr = new int[width, height];

        // Fill 2D-array with ints
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                arr[x, y] = num;
            }
        }

        return arr;
    }

    public override IntGrid FlipX() => FlipX(values => new IntGrid(values));
    public override IntGrid FlipY() => FlipY(values => new IntGrid(values));
}