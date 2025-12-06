namespace Lib.Grids;

/// <summary>
/// Factory helpers for constructing typed grids from text.
/// </summary>
public static class GridFactory
{
    /// <summary>
    /// Parses a character grid from equally-sized lines.
    /// </summary>
    /// <param name="lines">Input lines.</param>
    /// <returns>An <see cref="ArrayGrid{TValue}"/> of characters.</returns>
    /// <exception cref="ArgumentException">Thrown when input is empty or jagged.</exception>
    public static ArrayGrid<char> ParseCharGrid(IEnumerable<string> lines)
    {
        var rows = lines.ToList();
        if (rows.Count == 0)
            throw new ArgumentException("Input is empty", nameof(lines));

        var width = rows[0].Length;
        if (rows.Any(row => row.Length != width))
            throw new ArgumentException("All rows must have the same length");

        var height = rows.Count;
        var data = new char[width, height];
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            data[x, y] = rows[y][x];

        return new ArrayGrid<char>(data);
    }

    /// <summary>
    /// Parses an integer grid from equally-sized digit-only lines.
    /// </summary>
    /// <param name="lines">Input lines.</param>
    /// <returns>An <see cref="ArrayGrid{TValue}"/> of integers.</returns>
    /// <exception cref="ArgumentException">Thrown when input is empty, jagged, or contains non-digits.</exception>
    public static ArrayGrid<int> ParseIntGrid(IEnumerable<string> lines)
    {
        var rows = lines.ToList();
        if (rows.Count == 0)
            throw new ArgumentException("Input is empty", nameof(lines));

        var width = rows[0].Length;
        if (rows.Any(row => row.Length != width))
            throw new ArgumentException("All rows must have the same length");

        // Preserve the same orientation as ParseCharGrid: width first, height second.
        var data = new int[width, rows.Count];
        for (var y = 0; y < rows.Count; y++)
        for (var x = 0; x < width; x++)
        {
            if (!char.IsDigit(rows[y][x]))
                throw new ArgumentException("Non-digit character encountered when parsing int grid");
            data[x, y] = rows[y][x] - '0';
        }

        return new ArrayGrid<int>(data);
    }
}

