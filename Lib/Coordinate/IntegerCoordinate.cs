using System.Numerics;

namespace Lib.Coordinate;

public readonly struct IntegerCoordinate<T>(T x, T y)
    : ICoordinate<IntegerCoordinate<T>, T>, IStringCoordinate
    where T : INumber<T>, IBinaryInteger<T>
{
    public T X { get; } = x;
    public T Y { get; } = y;
    public static IntegerCoordinate<T> Zero { get; } = new(T.Zero, T.Zero);
    public static IntegerCoordinate<T> One { get; } = new(T.One, T.One);
    public static IntegerCoordinate<T> UnitX { get; } = new(T.One, T.Zero);
    public static IntegerCoordinate<T> UnitY { get; } = new(T.Zero, T.One);

    public IEnumerable<IntegerCoordinate<T>> Neighbours
    {
        get
        {
            var self = this;
            return DirectionExtensions.Cardinals.Select(
                direction => self + direction.ToCoordinate<T>()
            );
        }
    }

    public Distance<T> Distance(IntegerCoordinate<T> other)
        => new(other.X - X, other.Y - Y);

    public IntegerCoordinate<T> Move(Distance<T> distance) =>
        new(X + distance.X, Y + distance.Y);

    public IntegerCoordinate<T> Min(IntegerCoordinate<T> other) =>
        new(T.Min(X, other.X), T.Min(Y, other.Y));

    public IntegerCoordinate<T> Max(IntegerCoordinate<T> other) =>
        new(T.Max(X, other.X), T.Max(Y, other.Y));

    public IntegerCoordinate<T> Clamp(
        IntegerCoordinate<T> min,
        IntegerCoordinate<T> max
    ) => new(T.Clamp(X, min.X, max.X), T.Clamp(Y, min.Y, max.Y));

    public IntegerCoordinate<T> CopySign(Coordinate<T> sign) =>
        new(T.CopySign(X, sign.X), T.CopySign(Y, sign.Y));

    public IntegerCoordinate<T> Abs() =>
        new(T.Abs(X), T.Abs(Y));

    public IntegerCoordinate<T> Move(Direction direction) =>
        this + direction.ToCoordinate<T>();

    public IntegerCoordinate<T> DirectionOf(IntegerCoordinate<T> other)
    {
        var dx = other.X - X;
        var dy = other.Y - Y;

        return new IntegerCoordinate<T>(dx, dy).Clamp(-One, One);
    }

    public IEnumerable<IntegerCoordinate<T>> MoveTo(IntegerCoordinate<T> other)
    {
        for (var src = this; src != other; src += src.DirectionOf(other))
            yield return src;
    }

    public static IntegerCoordinate<T> operator -(IntegerCoordinate<T> coordinate) =>
        new(-coordinate.X, -coordinate.Y);

    public static IntegerCoordinate<T> operator &(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X & right.X, left.Y & right.Y);

    public static IntegerCoordinate<T> operator |(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X | right.X, left.Y | right.Y);

    public static IntegerCoordinate<T> operator ^(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X ^ right.X, left.Y ^ right.Y);

    public static IntegerCoordinate<T> operator ~(IntegerCoordinate<T> coordinate) =>
        new(~coordinate.X, ~coordinate.Y);


    public static IntegerCoordinate<T> operator +(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X + right.X, left.Y + right.Y);

    public static IntegerCoordinate<T> operator -(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X - right.X, left.Y - right.Y);

    public static bool operator ==(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        left.X == right.Y && left.Y == right.Y;

    public static bool operator !=(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        !(left == right);

    public static IntegerCoordinate<T> operator *(
        IntegerCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X * factor, coordinate.Y * factor);

    public static IntegerCoordinate<T> operator /(
        IntegerCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    public static IntegerCoordinate<T> operator *(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X * right.X, left.Y * right.Y);

    public static IntegerCoordinate<T> operator /(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X / right.X, left.Y / right.Y);

    public string? GetStringX() => X.ToString();

    public string? GetStringY() => Y.ToString();

    public bool Equals(IntegerCoordinate<T> other) => X == other.X && Y == other.Y;

    public override bool Equals(object? obj) =>
        obj is IntegerCoordinate<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);
}