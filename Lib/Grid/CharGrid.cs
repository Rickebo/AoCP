namespace Lib.Grid;

public class CharGrid : ArrayGrid<char>
{
    public CharGrid(
        string input,
        OriginPosition originPosition = OriginPosition.BottomLeft
    ) : base(Init(input, originPosition))
    {
    }

    public CharGrid(char[,] values) : base(values)
    {
    }

    private static char[,] Init(string str, OriginPosition originPosition = OriginPosition.BottomLeft)
    {
        // Parse string input
        var rows = Parser.SplitBy(str, ["\r\n", "\r", "\n"]);
        var height = rows.Length;
        var width = rows[0].Length;

        // Init array of chars
        var arr = new char[width, height];

        // Fill 2D-array with chars
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var mappedY = originPosition == OriginPosition.BottomLeft ? height - 1 - y : y;
                arr[x, y] = rows[mappedY][x];
            }
        }

        return arr;
    }

    public override CharGrid FlipX() => FlipX(values => new CharGrid(values));
    public override CharGrid FlipY() => FlipY(values => new CharGrid(values));
}