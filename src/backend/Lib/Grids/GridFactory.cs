namespace Lib.Grids;

public static class GridFactory
{
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

