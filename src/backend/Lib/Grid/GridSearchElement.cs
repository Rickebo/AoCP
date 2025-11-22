using Lib.Coordinate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Grid
{
    public class GridSearchElement<TValue, TCoordinate, TCoordinateNumber>
        where TCoordinate : ICoordinate<TCoordinate, TCoordinateNumber>, IStringCoordinate
        where TCoordinateNumber : INumber<TCoordinateNumber>
    {
        TValue Value { get; init; }
        TCoordinate Coordinate { get; init; }
    }
}
