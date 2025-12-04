using System.Numerics;
using Lib.Geometry;

namespace Lib.Grids;

public interface IGrid<TValue, in TCoordinate, TCoordinateNumber>
    where TCoordinate : ICoordinate<TCoordinate, TCoordinateNumber>, IStringCoordinate
    where TCoordinateNumber : INumber<TCoordinateNumber>
{
    /// <summary>
    /// Gets or sets the value stored at the specified coordinate.
    /// </summary>
    /// <param name="coordinate">Coordinate to address.</param>
    /// <returns>The grid value at the coordinate.</returns>
    public TValue this[TCoordinate coordinate] { get; set; }
}
