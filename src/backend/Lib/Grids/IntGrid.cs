using Lib.Geometry;
using Lib.Text;

namespace Lib.Grids;

/// <summary>
/// <see cref="ArrayGrid{TValue}"/> specialized for integer values.
/// </summary>
public class IntGrid : ArrayGrid<int>
{
    /// <summary>
    /// Creates a grid from a multiline string of digits, optionally providing a default value for non-digits.
    /// </summary>
    /// <param name="input">Text to parse.</param>
    /// <param name="defaultValue">Default value for any non-digit characters.</param>
    public IntGrid(string input, int? defaultValue = null) : base(ParseFromString(input, defaultValue)) {}

    /// <summary>
    /// Initializes a grid with the specified dimensions and fills it with the provided number.
    /// </summary>
    /// <param name="width">Grid width.</param>
    /// <param name="height">Grid height.</param>
    /// <param name="num">Value used to populate each cell.</param>
    public IntGrid(int width, int height, int num) : base(width, height, num) {}

    /// <summary>
    /// Wraps an existing integer array as a grid.
    /// </summary>
    /// <param name="values">Backing array.</param>
    public IntGrid(int[,] values) : base(values) {}

    /// <summary>
    /// Parses a multiline string of digits into an integer array.
    /// </summary>
    /// <param name="str">Input text.</param>
    /// <param name="defaultValue">Optional default value for non-digit characters.</param>
    /// <returns>Parsed grid values.</returns>
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

    /// <summary>
    /// Counts consecutive repeating integers starting at a coordinate in the specified direction.
    /// </summary>
    /// <param name="pos">Starting coordinate.</param>
    /// <param name="dir">Direction to move.</param>
    /// <returns>Number of repeating values including the starting cell.</returns>
    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir) =>
        CountRepeating(pos, dir, this[pos]);

    /// <summary>
    /// Counts consecutive cells equal to <paramref name="source"/> starting at a coordinate in the specified direction.
    /// </summary>
    /// <param name="pos">Starting coordinate.</param>
    /// <param name="dir">Direction to move.</param>
    /// <param name="source">Value that must repeat.</param>
    /// <returns>Number of matching values including the starting cell.</returns>
    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir, int source) =>
        RetrieveDirection(pos, dir).TakeWhile(val => val == source).Count();

    /// <summary>
    /// Returns a flipped copy of the grid along the specified axis.
    /// </summary>
    /// <param name="axis">Axis or axes to flip around.</param>
    /// <returns>A new <see cref="IntGrid"/> containing mirrored values.</returns>
    public override IntGrid Flip(Axis axis) => Flip(values => new IntGrid(values), axis);
}
