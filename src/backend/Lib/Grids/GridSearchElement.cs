using Lib.Geometry;
using Lib.Search;

namespace Lib.Grids;

/// <summary>
/// Search element representing a single coordinate/value pair in an <see cref="ArrayGrid{TValue}"/>.
/// Equality is based on the coordinate only so the same cell is treated as one node even if its value changes.
/// </summary>
public readonly record struct GridSearchElement<TValue, TCost>(
    /// <summary>
    /// Gets the coordinate represented by this search element.
    /// </summary>
    IntegerCoordinate<int> Coordinate,
    /// <summary>
    /// Gets the value stored at the coordinate.
    /// </summary>
    TValue Value
) : ISearchElement<TCost>
{
    /// <summary>
    /// Checks equality against another <see cref="GridSearchElement{TValue, TCost}"/> using only the coordinate.
    /// </summary>
    /// <param name="other">Element to compare with.</param>
    /// <returns><c>true</c> when the coordinates match; otherwise <c>false</c>.</returns>
    public bool Equals(GridSearchElement<TValue, TCost> other) =>
        Coordinate.Equals(other.Coordinate);

    /// <summary>
    /// Gets a hash code based on the coordinate.
    /// </summary>
    /// <returns>Hash code for the coordinate.</returns>
    public override int GetHashCode() => Coordinate.GetHashCode();

    /// <summary>
    /// Returns a string representation of the coordinate.
    /// </summary>
    /// <returns>Coordinate string.</returns>
    public override string ToString() => $"{Coordinate}";
}
