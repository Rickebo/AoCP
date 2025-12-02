using Lib.Coordinate;
using Lib.Search;

namespace Lib.Grid;

/// <summary>
/// Search element representing a single coordinate/value pair in an <see cref="ArrayGrid{TValue}"/>.
/// Equality is based on the coordinate only so the same cell is treated as one node even if its value changes.
/// </summary>
public readonly record struct GridSearchElement<TValue, TCost>(
    IntegerCoordinate<int> Coordinate,
    TValue Value
) : ISearchElement<TCost>
{
    public bool Equals(GridSearchElement<TValue, TCost> other) =>
        Coordinate.Equals(other.Coordinate);

    public override int GetHashCode() => Coordinate.GetHashCode();
    public override string ToString() => $"{Coordinate}";
}
