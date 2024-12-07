using System.Numerics;

namespace Lib.Coordinate;

public readonly struct FloatingCoordinate<T>(T x, T y)
    : ICoordinate<FloatingCoordinate<T>, T>, IStringCoordinate
    where T : IFloatingPoint<T>, IRootFunctions<T>
{
    public T X { get; } = x;
    public T Y { get; } = y;

    public T Length => T.Sqrt(X * X + Y * Y);

    public static FloatingCoordinate<T> Zero { get; } = new(T.Zero, T.Zero);
    public static FloatingCoordinate<T> One { get; } = new(T.One, T.One);
    public static FloatingCoordinate<T> UnitX { get; } = new(T.One, T.Zero);
    public static FloatingCoordinate<T> UnitY { get; } = new(T.Zero, T.One);

    public FloatingCoordinate<T> Min(FloatingCoordinate<T> other) =>
        new(T.Min(X, other.X), T.Min(Y, other.Y));

    public FloatingCoordinate<T> Max(FloatingCoordinate<T> other) =>
        new(T.Max(X, other.X), T.Max(Y, other.Y));

    public FloatingCoordinate<T> Clamp(
        FloatingCoordinate<T> min,
        FloatingCoordinate<T> max
    ) => new(
        T.Clamp(X, min.X, max.X),
        T.Clamp(Y, min.Y, max.Y)
    );

    public FloatingCoordinate<T> CopySign(Coordinate<T> sign) => new(
        T.CopySign(X, sign.X),
        T.CopySign(Y, sign.Y)
    );

    public FloatingCoordinate<T> Abs() => new(T.Abs(X), T.Abs(Y));

    public static FloatingCoordinate<T> operator -(FloatingCoordinate<T> coordinate) =>
        new(-coordinate.X, -coordinate.Y);

    public static FloatingCoordinate<T> operator +(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X + right.X, left.Y + right.Y);

    public static FloatingCoordinate<T> operator -(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X - right.X, left.Y - right.Y);

    public static bool operator ==(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        left.X == right.Y && left.Y == right.Y;

    public static bool operator !=(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        !(left == right);

    public static FloatingCoordinate<T> operator *(
        FloatingCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X * factor, coordinate.Y * factor);

    public static FloatingCoordinate<T> operator /(
        FloatingCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    public static FloatingCoordinate<T> operator *(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X * right.X, left.Y * right.Y);

    public static FloatingCoordinate<T> operator /(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X / right.X, left.Y / right.Y);

    public bool Equals(FloatingCoordinate<T> other) => X == other.X && Y == other.Y;

    public override bool Equals(object? obj) =>
        obj is FloatingCoordinate<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);
    public string? GetStringX() => X.ToString();

    public string? GetStringY() => Y.ToString();
}