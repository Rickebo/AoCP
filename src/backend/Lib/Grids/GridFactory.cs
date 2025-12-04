namespace Lib.Grids;

public static class GridFactory
{
    /// <summary>
    /// Parses a rectangular collection of strings into a character grid.
    /// </summary>
    /// <param name="lines">Input lines of uniform length.</param>
    /// <returns>A new grid containing the parsed characters.</returns>
    /// <exception cref="ArgumentException">Thrown when input is empty or rows differ in length.</exception>
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
    /// Parses a rectangular collection of numeric strings into an integer grid.
    /// </summary>
    /// <param name="lines">Input lines consisting of digits.</param>
    /// <returns>A new grid populated with parsed digits.</returns>
    /// <exception cref="ArgumentException">Thrown when input is empty, rows are uneven, or a non-digit is encountered.</exception>
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

