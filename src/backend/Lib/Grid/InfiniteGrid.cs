using System.Numerics;
using Lib.Coordinate;

namespace Lib.Grid;

public class InfiniteGrid<TValue, TCoordinateNumber>
    : IGrid<TValue, IntegerCoordinate<TCoordinateNumber>, TCoordinateNumber>
    where TCoordinateNumber : INumber<TCoordinateNumber>, IBinaryInteger<TCoordinateNumber>
{
    public IntegerCoordinate<TCoordinateNumber> Min { get; private set; } = new();
    public IntegerCoordinate<TCoordinateNumber> Max { get; private set; } = new();

    private readonly Dictionary<TCoordinateNumber, Dictionary<TCoordinateNumber, TValue>> _values = [];

    public IEnumerable<IntegerCoordinate<TCoordinateNumber>> Coordinates =>
        _values.SelectMany(
            pair => pair.Value.Keys.Select(
                x => new IntegerCoordinate<TCoordinateNumber>(x, pair.Key)
            )
        );

    public bool Contains(IntegerCoordinate<TCoordinateNumber> coordinate) =>
        _values.TryGetValue(coordinate.Y, out var row) && row.ContainsKey(coordinate.Y);

    public bool TryGetValue(IntegerCoordinate<TCoordinateNumber> coordinate, out TValue? value)
    {
        if (_values.TryGetValue(coordinate.Y, out var row))
            return row.TryGetValue(coordinate.X, out value);

        value = default;
        return false;
    }

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