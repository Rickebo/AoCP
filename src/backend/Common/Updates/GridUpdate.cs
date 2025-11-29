using Lib.Coordinate;
using Lib.Grid;
using Lib.Printing;

namespace Common.Updates;

public abstract class GridUpdate<T> : OngoingProblemUpdate
{
    public override string Type => "grid";

    public int? Width { get; set; } = null;
    public int? Height { get; set; } = null;
    public bool? Clear { get; set; } = null;
    public Dictionary<string, Dictionary<string, T>> Rows { get; set; } = [];

    public static TGridUpdate FromGrid<TGridUpdate, TCell, TGridCell>(
        ArrayGrid<TGridCell> grid,
        Func<TGridCell, TCell> cellConverter,
        Func<int, int, Dictionary<string, Dictionary<string, TCell>>, TGridUpdate> constructor
    ) where TCell : notnull where TGridUpdate : GridUpdate<TCell>
    {
        var rows = new Dictionary<string, Dictionary<string, TCell>>();
        for (var y = 0; y < grid.Height; y++)
        {
            var row = new Dictionary<string, TCell>();

            for (var x = 0; x < grid.Width; x++)
                row[x.ToString()] = cellConverter(grid[x, y]);

            rows[y.ToString()] = row;
        }

        return constructor(grid.Width, grid.Height, rows);
    }
}