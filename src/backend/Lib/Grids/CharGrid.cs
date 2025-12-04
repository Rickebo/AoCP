using Lib.Geometry;
using Lib.Text;

namespace Lib.Grids;

/// <summary>
/// Specialized <see cref="ArrayGrid{TValue}"/> for character data.
/// </summary>
public class CharGrid : ArrayGrid<char>
{
    /// <summary>
    /// Creates a grid from a multiline string, treating the first line as the top row.
    /// </summary>
    /// <param name="input">Multiline text to parse.</param>
    public CharGrid(string input) : base(ParseFromString(input)) {}

    /// <summary>
    /// Initializes a grid with the specified dimensions, filled with a character.
    /// </summary>
    /// <param name="width">Grid width.</param>
    /// <param name="height">Grid height.</param>
    /// <param name="c">Character used to fill the grid.</param>
    public CharGrid(int width, int height, char c) : base(width, height, c) {}

    /// <summary>
    /// Initializes a grid backed by an existing 2D character array.
    /// </summary>
    /// <param name="values">Values to wrap.</param>
    public CharGrid(char[,] values) : base(values) {}

    /// <summary>
    /// Parses a multiline string into a 2D character array.
    /// </summary>
    /// <param name="str">Text to parse.</param>
    /// <returns>Parsed array with origin at bottom-left.</returns>
    private static char[,] ParseFromString(string str)
    {
        // Parse string input
        var rows = str.SplitLines();
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
    /// Counts consecutive repeating characters from the starting position in a given direction.
    /// </summary>
    /// <param name="pos">Starting coordinate.</param>
    /// <param name="dir">Direction to move.</param>
    /// <returns>Number of repeating characters including the starting cell.</returns>
    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir) =>
        CountRepeating(pos, dir, this[pos]);

    /// <summary>
    /// Counts consecutive cells matching the supplied source character from the starting position in a given direction.
    /// </summary>
    /// <param name="pos">Starting coordinate.</param>
    /// <param name="dir">Direction to move.</param>
    /// <param name="source">Character to compare against.</param>
    /// <returns>Number of matching characters including the starting cell.</returns>
    public int CountRepeating(IntegerCoordinate<int> pos, Direction dir, char source) =>
        RetrieveDirection(pos, dir).TakeWhile(val => val == source).Count();

    /// <summary>
    /// Returns a flipped copy of the grid along the specified axis.
    /// </summary>
    /// <param name="axis">Axis or axes to flip around.</param>
    /// <returns>A new <see cref="CharGrid"/> containing mirrored values.</returns>
    public override CharGrid Flip(Axis axis) => Flip(values => new CharGrid(values), axis);
}
