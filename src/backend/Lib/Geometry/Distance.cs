using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Represents a 2D displacement backed by numeric components.
/// </summary>
/// <typeparam name="T">Numeric type for the displacement.</typeparam>
public readonly struct Distance<T>(T x, T y) where T : INumber<T>
{
    /// <summary>
    /// Gets the X component of the distance.
    /// </summary>
    public T X { get; } = x;

    /// <summary>
    /// Gets the Y component of the distance.
    /// </summary>
    public T Y { get; } = y;

    /// <summary>
    /// Zero distance.
    /// </summary>
    public static Distance<T> Zero => new(T.Zero, T.Zero);

    /// <summary>
    /// Computes the Manhattan length of the distance.
    /// </summary>
    public T Manhattan() => T.Abs(X) + T.Abs(Y);

    /// <summary>
    /// Returns the absolute value for each component.
    /// </summary>
    public Distance<T> Abs() => new(T.Abs(X), T.Abs(Y));

    /// <summary>
    /// Adds two distances component-wise.
    /// </summary>
    public static Distance<T> operator +(Distance<T> left, Distance<T> right) =>
        new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// Subtracts two distances component-wise.
    /// </summary>
    public static Distance<T> operator -(Distance<T> left, Distance<T> right) =>
        new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// Determines equality by comparing components.
    /// </summary>
    public static bool operator ==(Distance<T> left, Distance<T> right) =>
        left.X == right.X && left.Y == right.Y;

    /// <summary>
    /// Determines inequality by comparing components.
    /// </summary>
    public static bool operator !=(Distance<T> left, Distance<T> right) =>
        !(left == right);

    /// <summary>
    /// Multiplies both components by a scalar.
    /// </summary>
    public static Distance<T> operator *(Distance<T> coordinate, T factor) =>
    new(coordinate.X * factor, coordinate.Y * factor);

    /// <summary>
    /// Divides both components by a scalar.
    /// </summary>
    public static Distance<T> operator /(Distance<T> coordinate, T factor) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    /// <summary>
    /// Multiplies the components element-wise.
    /// </summary>
    public static Distance<T> operator *(Distance<T> left, Distance<T> right) =>
        new(left.X * right.X, left.Y * right.Y);

    /// <summary>
    /// Divides the components element-wise.
    /// </summary>
    public static Distance<T> operator /(Distance<T> left, Distance<T> right) =>
        new(left.X / right.X, left.Y / right.Y);

    /// <inheritdoc />
    public bool Equals(Distance<T> other) =>
        X == other.X && Y == other.Y;

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is Distance<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y);
}


