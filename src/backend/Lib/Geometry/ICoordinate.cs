using System.Numerics;

namespace Lib.Coordinate;

public interface ICoordinate<TCoordinate, TNumber> : IStringCoordinate
    where TCoordinate : ICoordinate<TCoordinate, TNumber>
    where TNumber : INumber<TNumber>
{
    TNumber X { get; }
    TNumber Y { get; }

    static abstract TCoordinate Zero { get; }
    static abstract TCoordinate One { get; }
    static abstract TCoordinate UnitX { get; }
    static abstract TCoordinate UnitY { get; }

    TCoordinate Min(TCoordinate other);
    TCoordinate Max(TCoordinate other);

    TCoordinate Clamp(TCoordinate min, TCoordinate max);

    TCoordinate CopySign(TCoordinate sign);

    TCoordinate Abs();

    TNumber ManhattanLength() => X + Y;
}

public interface ICoordinate3D<TCoordinate, TNumber> : ICoordinate<TCoordinate, TNumber>
    where TCoordinate : ICoordinate<TCoordinate, TNumber>
    where TNumber : INumber<TNumber>
{
    TNumber Z { get; }
    string? GetStringZ() => Z.ToString();
}
