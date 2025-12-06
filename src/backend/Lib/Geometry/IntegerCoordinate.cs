using System.Numerics;
using Lib.Math;

namespace Lib.Geometry;

/// <summary>
/// Integer-based 2D coordinate with common grid helpers.
/// </summary>
/// <typeparam name="T">Integer numeric type.</typeparam>
public readonly struct IntegerCoordinate<T>(T x, T y)
    : ICoordinate<IntegerCoordinate<T>, T>,
        IStringCoordinate,
        IEquatable<IntegerCoordinate<T>>
    where T : INumber<T>, IBinaryInteger<T>
{
    /// <summary>
    /// Gets the X component.
    /// </summary>
    public T X { get; } = x;

    /// <summary>
    /// Gets the Y component.
    /// </summary>
    public T Y { get; } = y;

    /// <summary>
    /// Coordinate at (0, 0).
    /// </summary>
    public static IntegerCoordinate<T> Zero { get; } = new(T.Zero, T.Zero);

    /// <summary>
    /// Coordinate at (1, 1).
    /// </summary>
    public static IntegerCoordinate<T> One { get; } = new(T.One, T.One);

    /// <summary>
    /// Unit vector along X.
    /// </summary>
    public static IntegerCoordinate<T> UnitX { get; } = new(T.One, T.Zero);

    /// <summary>
    /// Unit vector along Y.
    /// </summary>
    public static IntegerCoordinate<T> UnitY { get; } = new(T.Zero, T.One);

    /// <summary>
    /// Enumerates neighbouring coordinates according to the selected neighbourhood, excluding any flagged directions.
    /// </summary>
    /// <param name="neighbourhood">Neighbourhood shape to use.</param>
    /// <param name="excluded">Directions to skip.</param>
    public IEnumerable<IntegerCoordinate<T>> Neighbours(
        Neighbourhood neighbourhood = Neighbourhood.Cardinal,
        Direction excluded = Direction.None) =>
        Neighbourhoods.Neighbours(this, neighbourhood, excluded);

    /// <summary>
    /// Computes the Manhattan length.
    /// </summary>
    public T ManhattanLength() => T.Abs(X) + T.Abs(Y);

    /// <summary>
    /// Returns the distance vector to another coordinate.
    /// </summary>
    public Distance<T> Distance(IntegerCoordinate<T> other)
        => new(other.X - X, other.Y - Y);

    /// <summary>
    /// Moves by the provided distance vector.
    /// </summary>
    public IntegerCoordinate<T> Move(Distance<T> distance) =>
        new(X + distance.X, Y + distance.Y);

    /// <summary>
    /// Component-wise minimum.
    /// </summary>
    public IntegerCoordinate<T> Min(IntegerCoordinate<T> other) =>
        new(T.Min(X, other.X), T.Min(Y, other.Y));

    /// <summary>
    /// Component-wise maximum.
    /// </summary>
    public IntegerCoordinate<T> Max(IntegerCoordinate<T> other) =>
        new(T.Max(X, other.X), T.Max(Y, other.Y));

    /// <summary>
    /// Clamps the coordinate between the provided bounds.
    /// </summary>
    public IntegerCoordinate<T> Clamp(
        IntegerCoordinate<T> min,
        IntegerCoordinate<T> max
    ) => new(T.Clamp(X, min.X, max.X), T.Clamp(Y, min.Y, max.Y));

    /// <summary>
    /// Applies the sign of another coordinate.
    /// </summary>
    public IntegerCoordinate<T> CopySign(IntegerCoordinate<T> sign) =>
        new(T.CopySign(X, sign.X), T.CopySign(Y, sign.Y));

    /// <summary>
    /// Returns the absolute value of each component.
    /// </summary>
    public IntegerCoordinate<T> Abs() =>
        new(T.Abs(X), T.Abs(Y));

    /// <summary>
    /// Moves one step in the given direction.
    /// </summary>
    public IntegerCoordinate<T> Move(Direction direction) =>
        this + direction.ToCoordinate<T>();

    /// <summary>
    /// Computes a clamped step vector pointing from this coordinate toward <paramref name="other"/>.
    /// </summary>
    /// <param name="other">Destination coordinate.</param>
    /// <returns>A vector containing -1, 0, or 1 in each component.</returns>
    public IntegerCoordinate<T> DirectionOf(IntegerCoordinate<T> other)
    {
        var dx = other.X - X;
        var dy = other.Y - Y;

        return new IntegerCoordinate<T>(dx, dy).Clamp(-One, One);
    }

    /// <summary>
    /// Enumerates coordinates when moving one step at a time toward another coordinate (excluding the destination).
    /// </summary>
    public IEnumerable<IntegerCoordinate<T>> MoveTo(IntegerCoordinate<T> other)
    {
        for (var src = this; src != other; src += src.DirectionOf(other))
            yield return src;
    }

    /// <summary>
    /// Applies modulo to each component with the corresponding component in <paramref name="other"/>.
    /// </summary>
    public IntegerCoordinate<T> Modulo(IntegerCoordinate<T> other) => new(
        MathExtensions.Modulo(X, other.X),
        MathExtensions.Modulo(Y, other.Y)
    );

    /// <summary>
    /// Applies modulo to each component.
    /// </summary>
    public static IntegerCoordinate<T> operator %(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) => new(left.X % right.X, left.Y % right.Y);

    /// <summary>
    /// Negates both components.
    /// </summary>
    public static IntegerCoordinate<T> operator -(IntegerCoordinate<T> coordinate) =>
        new(-coordinate.X, -coordinate.Y);

    /// <summary>
    /// Performs bitwise AND on each component.
    /// </summary>
    public static IntegerCoordinate<T> operator &(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X & right.X, left.Y & right.Y);

    /// <summary>
    /// Performs bitwise OR on each component.
    /// </summary>
    public static IntegerCoordinate<T> operator |(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X | right.X, left.Y | right.Y);

    /// <summary>
    /// Performs bitwise XOR on each component.
    /// </summary>
    public static IntegerCoordinate<T> operator ^(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X ^ right.X, left.Y ^ right.Y);

    /// <summary>
    /// Performs bitwise NOT on each component.
    /// </summary>
    public static IntegerCoordinate<T> operator ~(IntegerCoordinate<T> coordinate) =>
        new(~coordinate.X, ~coordinate.Y);


    /// <summary>
    /// Adds coordinates component-wise.
    /// </summary>
    public static IntegerCoordinate<T> operator +(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// Subtracts coordinates component-wise.
    /// </summary>
    public static IntegerCoordinate<T> operator -(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// Determines equality by comparing components.
    /// </summary>
    public static bool operator ==(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        left.X == right.X && left.Y == right.Y;

    /// <summary>
    /// Determines inequality by comparing components.
    /// </summary>
    public static bool operator !=(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        !(left == right);

    /// <summary>
    /// Multiplies both components by a scalar.
    /// </summary>
    public static IntegerCoordinate<T> operator *(
        IntegerCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X * factor, coordinate.Y * factor);

    /// <summary>
    /// Divides both components by a scalar.
    /// </summary>
    public static IntegerCoordinate<T> operator /(
        IntegerCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    /// <summary>
    /// Multiplies components element-wise.
    /// </summary>
    public static IntegerCoordinate<T> operator *(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X * right.X, left.Y * right.Y);

    /// <summary>
    /// Divides components element-wise.
    /// </summary>
    public static IntegerCoordinate<T> operator /(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X / right.X, left.Y / right.Y);

    /// <summary>
    /// Returns the X component as a string.
    /// </summary>
    public string? GetStringX() => X.ToString();

    /// <summary>
    /// Returns the Y component as a string.
    /// </summary>
    public string? GetStringY() => Y.ToString();

    /// <inheritdoc />
    public bool Equals(IntegerCoordinate<T> other) => X == other.X && Y == other.Y;

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is IntegerCoordinate<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <summary>
    /// Returns a string representation of the coordinate.
    /// </summary>
    public override string ToString() => $"<{X} {Y}>";
}
