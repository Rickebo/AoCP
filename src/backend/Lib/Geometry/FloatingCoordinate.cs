using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Two-dimensional coordinate backed by floating point numbers.
/// </summary>
/// <typeparam name="T">Floating point numeric type.</typeparam>
public readonly struct FloatingCoordinate<T>(T x, T y)
    : ICoordinate<FloatingCoordinate<T>, T>, IStringCoordinate
    where T : IFloatingPoint<T>, IRootFunctions<T>
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
    /// Gets the Euclidean length of the coordinate vector.
    /// </summary>
    public T Length => T.Sqrt(X * X + Y * Y);

    /// <summary>
    /// Coordinate at (0, 0).
    /// </summary>
    public static FloatingCoordinate<T> Zero { get; } = new(T.Zero, T.Zero);

    /// <summary>
    /// Coordinate at (1, 1).
    /// </summary>
    public static FloatingCoordinate<T> One { get; } = new(T.One, T.One);

    /// <summary>
    /// Unit vector along X.
    /// </summary>
    public static FloatingCoordinate<T> UnitX { get; } = new(T.One, T.Zero);

    /// <summary>
    /// Unit vector along Y.
    /// </summary>
    public static FloatingCoordinate<T> UnitY { get; } = new(T.Zero, T.One);

    /// <summary>
    /// Component-wise minimum.
    /// </summary>
    public FloatingCoordinate<T> Min(FloatingCoordinate<T> other) =>
        new(T.Min(X, other.X), T.Min(Y, other.Y));

    /// <summary>
    /// Component-wise maximum.
    /// </summary>
    public FloatingCoordinate<T> Max(FloatingCoordinate<T> other) =>
        new(T.Max(X, other.X), T.Max(Y, other.Y));

    /// <summary>
    /// Clamps each component between the provided bounds.
    /// </summary>
    public FloatingCoordinate<T> Clamp(
        FloatingCoordinate<T> min,
        FloatingCoordinate<T> max
    ) => new(
        T.Clamp(X, min.X, max.X),
        T.Clamp(Y, min.Y, max.Y)
    );

    /// <summary>
    /// Copies the sign of another coordinate.
    /// </summary>
    public FloatingCoordinate<T> CopySign(FloatingCoordinate<T> sign) => new(
        T.CopySign(X, sign.X),
        T.CopySign(Y, sign.Y)
    );

    /// <summary>
    /// Returns the absolute value of each component.
    /// </summary>
    public FloatingCoordinate<T> Abs() => new(T.Abs(X), T.Abs(Y));

    /// <summary>
    /// Negates both components.
    /// </summary>
    public static FloatingCoordinate<T> operator -(FloatingCoordinate<T> coordinate) =>
        new(-coordinate.X, -coordinate.Y);

    /// <summary>
    /// Adds coordinates component-wise.
    /// </summary>
    public static FloatingCoordinate<T> operator +(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// Subtracts coordinates component-wise.
    /// </summary>
    public static FloatingCoordinate<T> operator -(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// Determines equality by comparing components.
    /// </summary>
    public static bool operator ==(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        left.X == right.X && left.Y == right.Y;

    /// <summary>
    /// Determines inequality by comparing components.
    /// </summary>
    public static bool operator !=(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        !(left == right);

    /// <summary>
    /// Multiplies both components by a scalar.
    /// </summary>
    public static FloatingCoordinate<T> operator *(
        FloatingCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X * factor, coordinate.Y * factor);

    /// <summary>
    /// Divides both components by a scalar.
    /// </summary>
    public static FloatingCoordinate<T> operator /(
        FloatingCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    /// <summary>
    /// Multiplies components element-wise.
    /// </summary>
    public static FloatingCoordinate<T> operator *(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X * right.X, left.Y * right.Y);

    /// <summary>
    /// Divides components element-wise.
    /// </summary>
    public static FloatingCoordinate<T> operator /(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X / right.X, left.Y / right.Y);

    /// <inheritdoc />
    public bool Equals(FloatingCoordinate<T> other) => X == other.X && Y == other.Y;

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is FloatingCoordinate<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <summary>
    /// Returns the X component as a string.
    /// </summary>
    public string? GetStringX() => X.ToString();

    /// <summary>
    /// Returns the Y component as a string.
    /// </summary>
    public string? GetStringY() => Y.ToString();
}
