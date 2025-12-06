using Lib.Geometry;
using Lib.Text;

namespace Lib.Grids;

public class CharGrid : ArrayGrid<char>
{
    public CharGrid(string input) : base(ParseFromString(input)) {}

    public CharGrid(string input, StringSplitOptions options) 
        : base(ParseFromString(input, options)) { }

    public CharGrid(int width, int height, char c) : base(width, height, c) {}

    public CharGrid(char[,] values) : base(values) {}

    private static char[,] ParseFromString(
        string str, 
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        // Parse string input
        var rows = str.SplitLines(options);
        var height = rows.Length;
        var width = rows[0].Length;

        // Fill 2D-array with chars (Y mapped to bottom left origin)
        var arr = new char[width, height];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                arr[x, y] = rows[^(y + 1)][x];

        return arr;
    }

    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir) =>
        CountRepeating(pos, dir, this[pos]);

    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir, char source) =>
        RetrieveDirection(pos, dir).TakeWhile(val => val == source).Count();

    public override CharGrid Flip(Axis axis) => Flip(values => new CharGrid(values), axis);
}
