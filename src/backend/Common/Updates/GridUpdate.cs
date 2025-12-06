using Lib.Grids;

namespace Common.Updates;

/// <summary>
/// Base type for grid-based updates emitted while a problem is running.
/// </summary>
/// <typeparam name="T">Type of the payload stored at each grid cell.</typeparam>
public abstract class GridUpdate<T> : OngoingProblemUpdate where T : notnull
{
    /// <inheritdoc />
    public override string Type => "grid";

    /// <summary>
    /// Gets or sets the grid width to apply to the client, if specified.
    /// </summary>
    public int? Width { get; set; } = null;

    /// <summary>
    /// Gets or sets the grid height to apply to the client, if specified.
    /// </summary>
    public int? Height { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether the client should clear the grid before applying updates.
    /// </summary>
    public bool? Clear { get; set; } = null;

    /// <summary>
    /// Gets or sets the rows of the grid keyed by Y then X values.
    /// </summary>
    public Dictionary<string, Dictionary<string, T>> Rows { get; set; } = [];

    /// <summary>
    /// Constructs a grid update from an <see cref="ArrayGrid{T}"/> using the provided converter and constructor.
    /// </summary>
    /// <typeparam name="TGridUpdate">Type of the resulting grid update.</typeparam>
    /// <typeparam name="TGridCell">Type of the source grid cells.</typeparam>
    /// <param name="grid">Source grid.</param>
    /// <param name="cellConverter">Converts each cell value to the target payload type.</param>
    /// <param name="constructor">
    /// Factory that creates the target update instance from the grid dimensions and row data.
    /// </param>
    /// <returns>A populated grid update.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="grid"/>, <paramref name="cellConverter"/>, or <paramref name="constructor"/> is null.
    /// </exception>
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

