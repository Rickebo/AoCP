using System.Numerics;
using Lib.Geometry;

namespace Lib.Grids;

public interface IGrid<TValue, in TCoordinate, TCoordinateNumber>
    where TCoordinate : ICoordinate<TCoordinate, TCoordinateNumber>, IStringCoordinate
    where TCoordinateNumber : INumber<TCoordinateNumber>
{
    public TValue this[TCoordinate coordinate] { get; set; }
}
