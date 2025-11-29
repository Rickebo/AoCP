using System.Numerics;

namespace Lib.Coordinate;

public readonly struct Distance<T>(T x, T y) where T : INumber<T>
{
    public T X { get; } = x;
    public T Y { get; } = y;

    public static Distance<T> Zero => new(T.Zero, T.Zero);

    public T Manhattan() => T.Abs(X) + T.Abs(Y);

    public Distance<T> Abs() => new(T.Abs(X), T.Abs(Y));

    public static Distance<T> operator +(Distance<T> left, Distance<T> right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static Distance<T> operator -(Distance<T> left, Distance<T> right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static bool operator ==(Distance<T> left, Distance<T> right) =>
        left.X == right.Y && left.Y == right.Y;

    public static bool operator !=(Distance<T> left, Distance<T> right) =>
        !(left == right);

    public static Distance<T> operator *(Distance<T> coordinate, T factor) =>
    new(coordinate.X * factor, coordinate.Y * factor);

    public static Distance<T> operator /(Distance<T> coordinate, T factor) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    public static Distance<T> operator *(Distance<T> left, Distance<T> right) =>
        new(left.X * right.X, left.Y * right.Y);

    public static Distance<T> operator /(Distance<T> left, Distance<T> right) =>
        new(left.X / right.X, left.Y / right.Y);

    public bool Equals(Distance<T> other) =>
        X == other.X && Y == other.Y;

    public override bool Equals(object? obj) =>
        obj is Distance<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);
}
