using Lib.Grids;

namespace Common.Updates;

public abstract class GridUpdate<T> : OngoingProblemUpdate where T : notnull
{
    public override string Type => "grid";

    public int? Width { get; set; } = null;

    public int? Height { get; set; } = null;

    public bool? Clear { get; set; } = null;

    public Dictionary<string, Dictionary<string, T>> Rows { get; set; } = [];

    public static TGridUpdate FromGrid<TGridUpdate, TGridCell>(
        ArrayGrid<TGridCell> grid,
        Func<TGridCell, T> cellConverter,
        Func<int, int, Dictionary<string, Dictionary<string, T>>, TGridUpdate> constructor
    ) where TGridUpdate : GridUpdate<T>
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(cellConverter);
        ArgumentNullException.ThrowIfNull(constructor);

        var rows = new Dictionary<string, Dictionary<string, T>>();
        for (var y = 0; y < grid.Height; y++)
        {
            var row = new Dictionary<string, T>();

            for (var x = 0; x < grid.Width; x++)
                row[x.ToString()] = cellConverter(grid[x, y]);

            rows[y.ToString()] = row;
        }

        return constructor(grid.Width, grid.Height, rows);
    }
}

