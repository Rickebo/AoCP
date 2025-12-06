using System.Numerics;
using Lib.Geometry;
using Lib.Grids;
using Lib.Search;

namespace Lib.Grids;

/// <summary>
/// Adapts an <see cref="ArrayGrid{TValue}"/> for use with generic search algorithms.
/// </summary>
/// <typeparam name="TValue">Grid cell type.</typeparam>
/// <typeparam name="TCost">Cost numeric type.</typeparam>
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
    /// Underlying grid.
    /// </summary>
    public ArrayGrid<TValue> Grid { get; } = grid;

    /// <summary>
    /// Indicates whether diagonal movement is allowed.
    /// </summary>
    public bool IncludeDiagonals { get; } = includeDiagonals;

    /// <summary>
    /// Wraps a coordinate into a search element.
    /// </summary>
    public GridSearchElement<TValue, TCost> ToElement(IntegerCoordinate<int> coordinate) =>
        new(coordinate, Grid[coordinate]);

    /// <inheritdoc />
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

