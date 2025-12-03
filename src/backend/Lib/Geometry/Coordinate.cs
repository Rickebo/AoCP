using System.Numerics;

namespace Lib.Geometry;

public readonly struct Coordinate<T>(T x, T y)
    : ICoordinate<Coordinate<T>, T>, IStringCoordinate
    where T : INumber<T>
{
    public T X { get; } = x;
    public T Y { get; } = y;
    public static Coordinate<T> Zero => new(T.Zero, T.Zero);
    public static Coordinate<T> One { get; } = new(T.One, T.One);
    public static Coordinate<T> UnitX { get; } = new(T.One, T.Zero);
    public static Coordinate<T> UnitY { get; } = new(T.Zero, T.One);

    public string? GetStringX() => X.ToString();

    public string? GetStringY() => Y.ToString();

    public Coordinate<T> Min(Coordinate<T> other) =>
        new(T.Min(X, other.X), T.Min(Y, other.Y));

    public Coordinate<T> Max(Coordinate<T> other) =>
        new(T.Max(X, other.X), T.Max(Y, other.Y));

    public Coordinate<T> Clamp(Coordinate<T> min, Coordinate<T> max) => new(
        T.Clamp(X, min.X, max.X),
        T.Clamp(Y, min.Y, max.Y)
    );

    public Coordinate<T> CopySign(Coordinate<T> sign) => new(
        T.CopySign(X, sign.X),
        T.CopySign(Y, sign.Y)
    );

    public Coordinate<T> Abs() => new(T.Abs(X), T.Abs(Y));

    public static Coordinate<T> operator +(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static Coordinate<T> operator -(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static bool operator ==(Coordinate<T> left, Coordinate<T> right) =>
        left.X == right.X && left.Y == right.Y;

    public static bool operator !=(Coordinate<T> left, Coordinate<T> right) =>
        !(left == right);

    public static Coordinate<T> operator *(Coordinate<T> coordinate, T factor) =>
        new(coordinate.X * factor, coordinate.Y * factor);

    public static Coordinate<T> operator /(Coordinate<T> coordinate, T factor) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    public static Coordinate<T> operator *(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X * right.X, left.Y * right.Y);

    public static Coordinate<T> operator /(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X / right.X, left.Y / right.Y);

    public bool Equals(Coordinate<T> other) => X == other.X && Y == other.Y;

    public override bool Equals(object? obj) =>
        obj is Coordinate<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);
}


