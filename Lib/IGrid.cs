using System.Numerics;

namespace Lib;

public interface IGrid<TValue, TCoordinate, TCoordinateNumber>
    where TCoordinate : ICoordinate<TCoordinate, TCoordinateNumber>
    where TCoordinateNumber : INumber<TCoordinateNumber>
{
    public TValue this[TCoordinate coordinate] { get; set; }
}