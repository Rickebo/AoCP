using System.Numerics;

namespace Lib.Coordinate;

public interface ICoordinate<TCoordinate, TNumber>
    where TCoordinate : IStringCoordinate
    where TNumber : INumber<TNumber>
{
    public TNumber X { get; }
    public TNumber Y { get; }

    public string? GetStringX() => X.ToString();
    public string? GetStringY() => Y.ToString();

    public static abstract TCoordinate Zero { get; }
    public static abstract TCoordinate One { get; }
    public static abstract TCoordinate UnitX { get; }
    public static abstract TCoordinate UnitY { get; }

    public TCoordinate Min(TCoordinate other);
    public TCoordinate Max(TCoordinate other);

    public TCoordinate Clamp(TCoordinate min, TCoordinate max);

    public TCoordinate CopySign(Coordinate<TNumber> sign);

    public TCoordinate Abs();

    public TNumber ManhattanLength() => X + Y;
}