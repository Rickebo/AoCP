using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Generic 2D coordinate backed by numeric components.
/// </summary>
/// <typeparam name="T">Numeric type for the coordinate components.</typeparam>
public readonly struct Coordinate<T>(T x, T y)
    : ICoordinate<Coordinate<T>, T>, IStringCoordinate
    where T : INumber<T>
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
    public static Coordinate<T> Zero => new(T.Zero, T.Zero);

    /// <summary>
    /// Coordinate at (1, 1).
    /// </summary>
    public static Coordinate<T> One { get; } = new(T.One, T.One);

    /// <summary>
    /// Unit vector along the X axis.
    /// </summary>
    public static Coordinate<T> UnitX { get; } = new(T.One, T.Zero);

    /// <summary>
    /// Unit vector along the Y axis.
    /// </summary>
    public static Coordinate<T> UnitY { get; } = new(T.Zero, T.One);

    /// <summary>
    /// Returns the X component as a string.
    /// </summary>
    public string? GetStringX() => X.ToString();

    /// <summary>
    /// Returns the Y component as a string.
    /// </summary>
    public string? GetStringY() => Y.ToString();

    /// <summary>
    /// Computes the component-wise minimum of two coordinates.
    /// </summary>
    public Coordinate<T> Min(Coordinate<T> other) =>
        new(T.Min(X, other.X), T.Min(Y, other.Y));

    /// <summary>
    /// Computes the component-wise maximum of two coordinates.
    /// </summary>
    public Coordinate<T> Max(Coordinate<T> other) =>
        new(T.Max(X, other.X), T.Max(Y, other.Y));

    /// <summary>
    /// Clamps each component between the corresponding components of <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    public Coordinate<T> Clamp(Coordinate<T> min, Coordinate<T> max) => new(
        T.Clamp(X, min.X, max.X),
        T.Clamp(Y, min.Y, max.Y)
    );

    /// <summary>
    /// Applies the sign of <paramref name="sign"/> to each component.
    /// </summary>
    public Coordinate<T> CopySign(Coordinate<T> sign) => new(
        T.CopySign(X, sign.X),
        T.CopySign(Y, sign.Y)
    );

    /// <summary>
    /// Returns the absolute value of each component.
    /// </summary>
    public Coordinate<T> Abs() => new(T.Abs(X), T.Abs(Y));

    /// <summary>
    /// Adds two coordinates component-wise.
    /// </summary>
    public static Coordinate<T> operator +(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// Subtracts two coordinates component-wise.
    /// </summary>
    public static Coordinate<T> operator -(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// Determines equality by comparing components.
    /// </summary>
    public static bool operator ==(Coordinate<T> left, Coordinate<T> right) =>
        left.X == right.X && left.Y == right.Y;

    /// <summary>
    /// Determines inequality by comparing components.
    /// </summary>
    public static bool operator !=(Coordinate<T> left, Coordinate<T> right) =>
        !(left == right);

    /// <summary>
    /// Multiplies both components by a scalar.
    /// </summary>
    public static Coordinate<T> operator *(Coordinate<T> coordinate, T factor) =>
        new(coordinate.X * factor, coordinate.Y * factor);

    /// <summary>
    /// Divides both components by a scalar.
    /// </summary>
    public static Coordinate<T> operator /(Coordinate<T> coordinate, T factor) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    /// <summary>
    /// Multiplies components element-wise.
    /// </summary>
    public static Coordinate<T> operator *(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X * right.X, left.Y * right.Y);

    /// <summary>
    /// Divides components element-wise.
    /// </summary>
    public static Coordinate<T> operator /(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X / right.X, left.Y / right.Y);

    /// <inheritdoc />
    public bool Equals(Coordinate<T> other) => X == other.X && Y == other.Y;

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is Coordinate<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y);
}


