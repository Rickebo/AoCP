using Lib.Geometry;
using Lib.Text;

namespace Lib.Grids;

/// <summary>
/// Integer grid with parsing helpers.
/// </summary>
public class IntGrid : ArrayGrid<int>
{
    /// <summary>
    /// Creates a grid from multiline input of digits.
    /// </summary>
    public IntGrid(string input) : base(ParseFromString(input)) { }

    /// <summary>
    /// Creates a grid from multiline input, replacing non-digits with <paramref name="defaultValue"/>.
    /// </summary>
    public IntGrid(string input, int defaultValue)
        : base(ParseFromString(input, defaultValue)) { }

    /// <summary>
    /// Creates a grid from multiline input using custom split options.
    /// </summary>
    public IntGrid(string input, StringSplitOptions options)
        : base(ParseFromString(input, options: options)) { }

    /// <summary>
    /// Creates a grid from multiline input, replacing non-digits with <paramref name="defaultValue"/> using custom split options.
    /// </summary>
    public IntGrid(string input, int defaultValue, StringSplitOptions options)
        : base(ParseFromString(input, defaultValue, options)) { }

    /// <summary>
    /// Creates a grid of the given size filled with <paramref name="num"/>.
    /// </summary>
    public IntGrid(int width, int height, int num) : base(width, height, num) { }

    /// <summary>
    /// Creates a grid from an existing backing array.
    /// </summary>
    public IntGrid(int[,] values) : base(values) { }

    private static int[,] ParseFromString(
        string str, 
        int? defaultValue = null, 
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        // Parse string input
        var rows = str.SplitLines(options);
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

    /// <summary>
    /// Counts repeating values starting at <paramref name="pos"/> in <paramref name="dir"/>.
    /// </summary>
    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir) =>
        CountRepeating(pos, dir, this[pos]);

    /// <summary>
    /// Counts repeating values matching <paramref name="source"/> starting at <paramref name="pos"/> in <paramref name="dir"/>.
    /// </summary>
    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir, int source) =>
        RetrieveDirection(pos, dir).TakeWhile(val => val == source).Count();

    /// <inheritdoc />
    public override IntGrid Flip(Axis axis) => Flip(values => new IntGrid(values), axis);
}
