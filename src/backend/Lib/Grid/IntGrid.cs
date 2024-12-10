namespace Lib.Grid;

public class IntGrid : ArrayGrid<int>
{
    public IntGrid(string input, int defaultValue, OriginPosition originPosition = OriginPosition.BottomLeft) : base(ArrFromStr(input, defaultValue, originPosition)) { }

    public IntGrid(int num, int width, int height) : base(ArrFromValSize(num, width, height)) { }

    public IntGrid(int[,] values) : base(values) { }

    private static int[,] ArrFromStr(string str, int defaultValue, OriginPosition originPosition = OriginPosition.BottomLeft)
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
                arr[x, y] = char.IsDigit(rows[mappedY][x]) ? rows[mappedY][x] - '0' : defaultValue;
            }
        }

        return arr;
    }

    private static int[,] ArrFromValSize(int num, int width, int height)
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