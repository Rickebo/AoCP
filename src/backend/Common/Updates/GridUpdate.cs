using Lib.Grids;

namespace Common.Updates;

/// <summary>
/// Base type for updates that paint data onto a 2D grid.
/// </summary>
public abstract class GridUpdate<T> : OngoingProblemUpdate
{
    /// <summary>
    /// Gets the update type string used for grid payloads.
    /// </summary>
    public override string Type => "grid";

    /// <summary>
    /// Gets or sets the total grid width when available.
    /// </summary>
    public int? Width { get; set; } = null;

    /// <summary>
    /// Gets or sets the total grid height when available.
    /// </summary>
    public int? Height { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether the client should clear any previously rendered state.
    /// </summary>
    public bool? Clear { get; set; } = null;

    /// <summary>
    /// Gets or sets the row data keyed by Y, then X coordinates.
    /// </summary>
    public Dictionary<string, Dictionary<string, T>> Rows { get; set; } = [];

    /// <summary>
    /// Builds a grid update instance from a concrete <see cref="ArrayGrid{TGridCell}"/>.
    /// </summary>
    /// <typeparam name="TGridUpdate">Target grid update type.</typeparam>
    /// <typeparam name="TCell">Type of cells after conversion.</typeparam>
    /// <typeparam name="TGridCell">Type stored in the source grid.</typeparam>
    /// <param name="grid">Grid to translate into a payload.</param>
    /// <param name="cellConverter">Function converting grid cells into the payload type.</param>
    /// <param name="constructor">Factory that constructs the final update using width, height, and rows.</param>
    /// <returns>An initialized grid update containing the grid contents.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static TGridUpdate FromGrid<TGridUpdate, TCell, TGridCell>(
        ArrayGrid<TGridCell> grid,
        Func<TGridCell, TCell> cellConverter,
        Func<int, int, Dictionary<string, Dictionary<string, TCell>>, TGridUpdate> constructor
    ) where TCell : notnull where TGridUpdate : GridUpdate<TCell>
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(cellConverter);
        ArgumentNullException.ThrowIfNull(constructor);

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

