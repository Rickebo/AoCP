namespace Lib.Grid;

public class IntGrid : ArrayGrid<int>
{
    public IntGrid(
        string input,
        OriginPosition originPosition = OriginPosition.BottomLeft
    ) : base(Init(input, originPosition))
    {
    }

    public IntGrid(int[,] values) : base(values)
    {
    }

    private static int[,] Init(string str, OriginPosition originPosition = OriginPosition.BottomLeft)
    {
        // Parse string input
        var rows = Parser.SplitBy(str, ["\r\n", "\r", "\n"]);
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
                arr[x, y] = rows[mappedY][x] - '0';
            }
        }

        return arr;
    }

    public override IntGrid FlipX() => FlipX(values => new IntGrid(values));
    public override IntGrid FlipY() => FlipY(values => new IntGrid(values));
}