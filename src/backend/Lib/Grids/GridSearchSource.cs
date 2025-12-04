using System.Numerics;
using Lib.Geometry;
using Lib.Grids;
using Lib.Search;

namespace Lib.Grids;

/// <summary>
/// Adapter that exposes an <see cref="ArrayGrid{TValue}"/> as an <see cref="ISearchSource{TElement,TCost}"/>.
/// </summary>
public sealed class GridSearchSource<TValue, TCost>(
    ArrayGrid<TValue> grid,
    Func<IntegerCoordinate<int>, TValue, bool> isWalkable,
    Func<IntegerCoordinate<int>, IntegerCoordinate<int>, TValue, TValue, TCost> costSelector,
    bool includeDiagonals = false
) : ISearchSource<GridSearchElement<TValue, TCost>, TCost>
    where TCost : INumber<TCost>
{
    private readonly Func<IntegerCoordinate<int>, IntegerCoordinate<int>, TValue, TValue, TCost> _costSelector = costSelector;
    private readonly Func<IntegerCoordinate<int>, TValue, bool> _isWalkable = isWalkable;

    /// <summary>
    /// Gets the grid being searched.
    /// </summary>
    public ArrayGrid<TValue> Grid { get; } = grid;

    /// <summary>
    /// Gets a value indicating whether diagonal neighbours are considered.
    /// </summary>
    public bool IncludeDiagonals { get; } = includeDiagonals;

    /// <summary>
    /// Converts a coordinate into a search element containing its value.
    /// </summary>
    /// <param name="coordinate">Coordinate to wrap.</param>
    /// <returns>A search element representing the coordinate.</returns>
    public GridSearchElement<TValue, TCost> ToElement(IntegerCoordinate<int> coordinate) =>
        new(coordinate, Grid[coordinate]);

    /// <summary>
    /// Returns walkable neighbours for a given element along with their traversal cost.
    /// </summary>
    /// <param name="element">Element whose neighbours to explore.</param>
    /// <returns>Neighbour elements and their associated costs.</returns>
    public IEnumerable<SearchNeighbour<GridSearchElement<TValue, TCost>, TCost>> GetNeighbours(
        GridSearchElement<TValue, TCost> element
    )
    {
        var neighbours = IncludeDiagonals
            ? Neighbourhoods.All2DNeighbours(element.Coordinate)
            : Neighbourhoods.Orthogonal(element.Coordinate);

        foreach (var neighbour in neighbours)
        {
            if (!Grid.Contains(neighbour))
                continue;

            var neighbourValue = Grid[neighbour];

            if (!_isWalkable(neighbour, neighbourValue))
                continue;

            yield return new SearchNeighbour<GridSearchElement<TValue, TCost>, TCost>
            {
                Element = new GridSearchElement<TValue, TCost>(neighbour, neighbourValue),
                Cost = _costSelector(
                    element.Coordinate,
                    neighbour,
                    element.Value,
                    neighbourValue
                )
            };
        }
    }
}

