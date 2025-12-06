using Lib.Geometry;
using Lib.Search;

namespace Lib.Grids;

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
