using System.Numerics;
using Lib.Geometry;

namespace Lib.Grids;

/// <summary>
/// Sparse grid that grows as values are written, tracked by integer coordinates.
/// </summary>
public class InfiniteGrid<TValue, TCoordinateNumber>
    : IGrid<TValue, IntegerCoordinate<TCoordinateNumber>, TCoordinateNumber>
    where TCoordinateNumber : INumber<TCoordinateNumber>, IBinaryInteger<TCoordinateNumber>
{
    /// <summary>
    /// Gets the minimum coordinate that has been written so far.
    /// </summary>
    public IntegerCoordinate<TCoordinateNumber> Min { get; private set; } = new();

    /// <summary>
    /// Gets the maximum coordinate that has been written so far.
    /// </summary>
    public IntegerCoordinate<TCoordinateNumber> Max { get; private set; } = new();

    private readonly Dictionary<TCoordinateNumber, Dictionary<TCoordinateNumber, TValue>> _values = [];

    /// <summary>
    /// Enumerates all coordinates that currently have stored values.
    /// </summary>
    public IEnumerable<IntegerCoordinate<TCoordinateNumber>> Coordinates =>
        _values.SelectMany(
            pair => pair.Value.Keys.Select(
                x => new IntegerCoordinate<TCoordinateNumber>(x, pair.Key)
            )
        );

    /// <summary>
    /// Determines whether a value exists at the given coordinate.
    /// </summary>
    /// <param name="coordinate">Coordinate to check.</param>
    /// <returns><c>true</c> when a value is present; otherwise <c>false</c>.</returns>
    public bool Contains(IntegerCoordinate<TCoordinateNumber> coordinate) =>
        _values.TryGetValue(coordinate.Y, out var row) && row.ContainsKey(coordinate.X);

    /// <summary>
    /// Attempts to retrieve the value at the given coordinate.
    /// </summary>
    /// <param name="coordinate">Coordinate to read.</param>
    /// <param name="value">Value found at the coordinate, if present.</param>
    /// <returns><c>true</c> when a value is present; otherwise <c>false</c>.</returns>
    public bool TryGetValue(IntegerCoordinate<TCoordinateNumber> coordinate, out TValue? value)
    {
        if (_values.TryGetValue(coordinate.Y, out var row))
            return row.TryGetValue(coordinate.X, out value);

        value = default;
        return false;
    }

    /// <summary>
    /// Gets or sets the value stored at the specified coordinate.
    /// </summary>
    /// <param name="coordinate">Coordinate to address.</param>
    /// <returns>The stored value.</returns>
    public TValue this[IntegerCoordinate<TCoordinateNumber> coordinate]
    {
        get => _values[coordinate.Y][coordinate.X];
        set
        {
            if (!_values.TryGetValue(coordinate.Y, out var row))
                row = _values[coordinate.Y] = [];

            row[coordinate.X] = value;
            Min = Min.Min(coordinate);
            Max = Max.Max(coordinate);
        }
    }
}

