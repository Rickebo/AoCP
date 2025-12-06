using Lib.Geometry;
using Lib.Text;

namespace Lib.Grids;

/// <summary>
/// Character grid with helpers for parsing string input.
/// </summary>
public class CharGrid : ArrayGrid<char>
{
    /// <summary>
    /// Creates a grid from multiline input, splitting on newlines.
    /// </summary>
    /// <param name="input">Raw input text.</param>
    public CharGrid(string input) : base(ParseFromString(input)) {}

    /// <summary>
    /// Creates a grid from multiline input with custom split options.
    /// </summary>
    /// <param name="input">Raw input text.</param>
    /// <param name="options">Line splitting options.</param>
    public CharGrid(string input, StringSplitOptions options) 
        : base(ParseFromString(input, options)) { }

    /// <summary>
    /// Creates a grid of the specified size filled with <paramref name="c"/>.
    /// </summary>
    public CharGrid(int width, int height, char c) : base(width, height, c) {}

    /// <summary>
    /// Creates a grid from an existing backing array.
    /// </summary>
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

    /// <summary>
    /// Counts repeating characters starting at <paramref name="pos"/> moving in <paramref name="dir"/>.
    /// </summary>
    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir) =>
        CountRepeating(pos, dir, this[pos]);

    /// <summary>
    /// Counts repeating characters matching <paramref name="source"/> starting at <paramref name="pos"/> in <paramref name="dir"/>.
    /// </summary>
    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir, char source) =>
        RetrieveDirection(pos, dir).TakeWhile(val => val == source).Count();

    /// <inheritdoc />
    public override CharGrid Flip(Axis axis) => Flip(values => new CharGrid(values), axis);
}
