using System.Numerics;
using Lib.Geometry;

namespace Lib.Grids;

/// <summary>
/// Defines the minimal contract for grid types accessed via coordinates.
/// </summary>
public interface IGrid<TValue, in TCoordinate, TCoordinateNumber>
    where TCoordinate : ICoordinate<TCoordinate, TCoordinateNumber>, IStringCoordinate
    where TCoordinateNumber : INumber<TCoordinateNumber>
{
    /// <summary>
    /// Gets or sets the value at the given coordinate.
    /// </summary>
    public TValue this[TCoordinate coordinate] { get; set; }
}
