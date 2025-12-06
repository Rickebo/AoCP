using Lib.Geometry;
using Lib.Search;

namespace Lib.Grids;

/// <summary>
/// Search element wrapper for a grid coordinate and its associated value.
/// </summary>
public readonly record struct GridSearchElement<TValue, TCost>(
    IntegerCoordinate<int> Coordinate,
    TValue Value
) : ISearchElement<TCost>
{
    /// <inheritdoc />
    public bool Equals(GridSearchElement<TValue, TCost> other) =>
        Coordinate.Equals(other.Coordinate);

    /// <inheritdoc />
    public override int GetHashCode() => Coordinate.GetHashCode();

    /// <summary>
    /// Returns the coordinate as the textual representation.
    /// </summary>
    public override string ToString() => $"{Coordinate}";
}
