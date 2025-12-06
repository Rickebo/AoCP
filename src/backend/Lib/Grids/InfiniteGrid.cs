using System.Numerics;
using Lib.Geometry;

namespace Lib.Grids;

/// <summary>
/// Sparse grid that grows as values are assigned.
/// </summary>
/// <typeparam name="TValue">Cell value type.</typeparam>
/// <typeparam name="TCoordinateNumber">Numeric type for coordinates.</typeparam>
public class InfiniteGrid<TValue, TCoordinateNumber>
    : IGrid<TValue, IntegerCoordinate<TCoordinateNumber>, TCoordinateNumber>
    where TCoordinateNumber : INumber<TCoordinateNumber>, IBinaryInteger<TCoordinateNumber>
{
    /// <summary>
    /// Minimum coordinate that has been assigned.
    /// </summary>
    public IntegerCoordinate<TCoordinateNumber> Min { get; private set; } = new();

    /// <summary>
    /// Maximum coordinate that has been assigned.
    /// </summary>
    public IntegerCoordinate<TCoordinateNumber> Max { get; private set; } = new();

    private readonly Dictionary<TCoordinateNumber, Dictionary<TCoordinateNumber, TValue>> _values = [];
    private bool _hasValues;

    /// <summary>
    /// All coordinates that currently have stored values.
    /// </summary>
    public IEnumerable<IntegerCoordinate<TCoordinateNumber>> Coordinates =>
        _values.SelectMany(
            pair => pair.Value.Keys.Select(
                x => new IntegerCoordinate<TCoordinateNumber>(x, pair.Key)
            )
        );

    /// <summary>
    /// Determines whether a coordinate has an assigned value.
    /// </summary>
    public bool Contains(IntegerCoordinate<TCoordinateNumber> coordinate) =>
        _values.TryGetValue(coordinate.Y, out var row) && row.ContainsKey(coordinate.X);

    /// <summary>
    /// Attempts to retrieve the value at a coordinate.
    /// </summary>
    public bool TryGetValue(IntegerCoordinate<TCoordinateNumber> coordinate, out TValue? value)
    {
        if (_values.TryGetValue(coordinate.Y, out var row))
            return row.TryGetValue(coordinate.X, out value);

        value = default;
        return false;
    }

    /// <summary>
    /// Gets or sets the value at the specified coordinate, tracking the observed bounds.
    /// </summary>
    public TValue this[IntegerCoordinate<TCoordinateNumber> coordinate]
    {
        get => _values[coordinate.Y][coordinate.X];
        set
        {
            if (!_values.TryGetValue(coordinate.Y, out var row))
                row = _values[coordinate.Y] = [];

            row[coordinate.X] = value;
            if (!_hasValues)
            {
                Min = coordinate;
                Max = coordinate;
                _hasValues = true;
                return;
            }

            Min = Min.Min(coordinate);
            Max = Max.Max(coordinate);
        }
    }
}

