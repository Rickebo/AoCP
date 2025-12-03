using Lib.Geometry;
using Lib.Text;

namespace Lib.Grids;

public class IntGrid : ArrayGrid<int>
{
    public IntGrid(string input, int? defaultValue = null) : base(ParseFromString(input, defaultValue)) {}

    public IntGrid(int width, int height, int num) : base(width, height, num) {}

    public IntGrid(int[,] values) : base(values) {}

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

    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir) =>
        CountRepeating(pos, dir, this[pos]);

    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir, int source) =>
        RetrieveDirection(pos, dir).TakeWhile(val => val == source).Count();

    public override IntGrid Flip(Axis axis) => Flip(values => new IntGrid(values), axis);
}

