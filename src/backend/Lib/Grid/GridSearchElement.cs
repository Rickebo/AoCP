using Lib.Coordinate;
using System.Numerics;

namespace Lib.Grid;

public class GridSearchElement<TValue, TCoordinate, TCoordinateNumber>
    where TCoordinate : ICoordinate<TCoordinate, TCoordinateNumber>, IStringCoordinate
    where TCoordinateNumber : INumber<TCoordinateNumber>
{
    TValue? Value { get; init; }
    TCoordinate? Coordinate { get; init; }
}
