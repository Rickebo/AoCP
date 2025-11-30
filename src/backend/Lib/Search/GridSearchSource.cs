namespace Lib.Search;

// public class GridSearchSource<TElement, TSearchElement, TCoordinate, TCoordinateNumber, TCost> : ISearchSource<TSearchElement, TCost>
//     where TElement : ISearchElement<TCost>
//     where TCoordinate : ICoordinate<TCoordinate, TCoordinateNumber>, IStringCoordinate
//     where TCoordinateNumber : INumber<TCoordinateNumber>
//     where TCost : INumber<TCost>
//     where TSearchElement : ISearchElement<TCost>
// {
//     public IGrid<TElement, TCoordinate, TCoordinateNumber> Grid { get; }
//     public Func<TElement, TSearchElement> ElementAdapter { get; }
//     public Func<TElement, TElement> Cost { get; }
//     public TCoordinateNumber NeighbourOffset { get; }
//
//     public GridSearchSource(
//         IGrid<TElement, TCoordinate, TCoordinateNumber> grid,
//         Func<TElement, TSearchElement> elementAdapter,
//         Func<TElement, TElement> cost,
//         TCoordinateNumber neighbourOffset
//         )
//     {
//         Grid = grid;
//         ElementAdapter = elementAdapter;
//         Cost = cost;
//         NeighbourOffset = neighbourOffset;
//     }
//
//     public IEnumerable<SearchNeighbour<TSearchElement, TCost>> GetNeighbours(TSearchElement element)
//     {
//         var offsets = new []
//         {
//             TCoordinate.UnitX,
//             TCoordinate.UnitY,
//             -TCoordinate.UnitX,
//             -TCoordinate.UnitY,
//         }:
//         var neighbours = Grid
//     }
// }