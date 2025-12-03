using Lib.Geometry;
using System.Numerics;

namespace Lib.Geometry;

public readonly struct Coordinate3D<T>(T x, T y, T z)
    : ICoordinate3D<Coordinate3D<T>, T>
    where T : INumber<T>
{
    public T X { get; } = x;
    public T Y { get; } = y;
    public T Z { get; } = z;

    public static Coordinate3D<T> Zero => new(T.Zero, T.Zero, T.Zero);
    public static Coordinate3D<T> One => new(T.One, T.One, T.One);
    public static Coordinate3D<T> UnitX => new(T.One, T.Zero, T.Zero);
    public static Coordinate3D<T> UnitY => new(T.Zero, T.One, T.Zero);
    public static Coordinate3D<T> UnitZ => new(T.Zero, T.Zero, T.One);

    public string? GetStringX() => X.ToString();
    public string? GetStringY() => Y.ToString();
    public string? GetStringZ() => Z.ToString();

    public Coordinate3D<T> CopySign(Coordinate3D<T> sign) => new(
        T.CopySign(X, sign.X),
        T.CopySign(Y, sign.Y),
        T.CopySign(Z, sign.Z)
    );

    public Coordinate3D<T> Abs() => new(T.Abs(X), T.Abs(Y), T.Abs(Z));

    public T ManhattanLength() => T.Abs(X) + T.Abs(Y) + T.Abs(Z);



    public Coordinate3D<T> Min(Coordinate3D<T> other) => new(
        T.Min(X, other.X),
        T.Min(Y, other.Y),
        T.Min(Z, other.Z)
    );

    public Coordinate3D<T> Max(Coordinate3D<T> other) => new(
        T.Max(X, other.X),
        T.Max(Y, other.Y),
        T.Max(Z, other.Z)
    );

    public Coordinate3D<T> Clamp(Coordinate3D<T> min, Coordinate3D<T> max) => new(
        T.Clamp(X, min.X, max.X),
        T.Clamp(Y, min.Y, max.Y),
        T.Clamp(Z, min.Z, max.Z)
    );

    public static Coordinate3D<T> operator +(Coordinate3D<T> left, Coordinate3D<T> right) =>
        new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Coordinate3D<T> operator -(Coordinate3D<T> left, Coordinate3D<T> right) =>
        new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Coordinate3D<T> operator *(Coordinate3D<T> coordinate, T factor) =>
        new(coordinate.X * factor, coordinate.Y * factor, coordinate.Z * factor);

    public static Coordinate3D<T> operator /(Coordinate3D<T> coordinate, T factor) =>
        new(coordinate.X / factor, coordinate.Y / factor, coordinate.Z / factor);

    public bool Equals(Coordinate3D<T> other) =>
        X == other.X && Y == other.Y && Z == other.Z;

    public override bool Equals(object? obj) =>
        obj is Coordinate3D<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);
}

