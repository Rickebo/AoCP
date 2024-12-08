using System.Numerics;
using Lib.Coordinate;

namespace Lib.Grid;

public interface IGrid<TValue, in TCoordinate, TCoordinateNumber>
    where TCoordinate : ICoordinate<TCoordinate, TCoordinateNumber>, IStringCoordinate
    where TCoordinateNumber : INumber<TCoordinateNumber>
{
    public TValue this[TCoordinate coordinate] { get; set; }
}