using Lib.Geometry;
using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Generic 3D coordinate backed by numeric components.
/// </summary>
/// <typeparam name="T">Numeric type for the coordinate components.</typeparam>
public readonly struct Coordinate3D<T>(T x, T y, T z)
    : ICoordinate3D<Coordinate3D<T>, T>
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
    /// Gets the Z component.
    /// </summary>
    public T Z { get; } = z;

    /// <summary>
    /// Coordinate at (0, 0, 0).
    /// </summary>
    public static Coordinate3D<T> Zero => new(T.Zero, T.Zero, T.Zero);

    /// <summary>
    /// Coordinate at (1, 1, 1).
    /// </summary>
    public static Coordinate3D<T> One => new(T.One, T.One, T.One);

    /// <summary>
    /// Unit vector along the X axis.
    /// </summary>
    public static Coordinate3D<T> UnitX => new(T.One, T.Zero, T.Zero);

    /// <summary>
    /// Unit vector along the Y axis.
    /// </summary>
    public static Coordinate3D<T> UnitY => new(T.Zero, T.One, T.Zero);

    /// <summary>
    /// Unit vector along the Z axis.
    /// </summary>
    public static Coordinate3D<T> UnitZ => new(T.Zero, T.Zero, T.One);

    /// <summary>
    /// Returns the X component as a string.
    /// </summary>
    public string? GetStringX() => X.ToString();

    /// <summary>
    /// Returns the Y component as a string.
    /// </summary>
    public string? GetStringY() => Y.ToString();

    /// <summary>
    /// Returns the Z component as a string.
    /// </summary>
    public string? GetStringZ() => Z.ToString();

    /// <summary>
    /// Copies the sign of another coordinate onto each component.
    /// </summary>
    public Coordinate3D<T> CopySign(Coordinate3D<T> sign) => new(
        T.CopySign(X, sign.X),
        T.CopySign(Y, sign.Y),
        T.CopySign(Z, sign.Z)
    );

    /// <summary>
    /// Returns the absolute value of each component.
    /// </summary>
    public Coordinate3D<T> Abs() => new(T.Abs(X), T.Abs(Y), T.Abs(Z));

    /// <summary>
    /// Computes the Manhattan length of the coordinate.
    /// </summary>
    public T ManhattanLength() => T.Abs(X) + T.Abs(Y) + T.Abs(Z);

    /// <summary>
    /// Computes the component-wise minimum of two coordinates.
    /// </summary>
    public Coordinate3D<T> Min(Coordinate3D<T> other) => new(
        T.Min(X, other.X),
        T.Min(Y, other.Y),
        T.Min(Z, other.Z)
    );

    /// <summary>
    /// Computes the component-wise maximum of two coordinates.
    /// </summary>
    public Coordinate3D<T> Max(Coordinate3D<T> other) => new(
        T.Max(X, other.X),
        T.Max(Y, other.Y),
        T.Max(Z, other.Z)
    );

    /// <summary>
    /// Clamps each component between corresponding components in <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    public Coordinate3D<T> Clamp(Coordinate3D<T> min, Coordinate3D<T> max) => new(
        T.Clamp(X, min.X, max.X),
        T.Clamp(Y, min.Y, max.Y),
        T.Clamp(Z, min.Z, max.Z)
    );

    /// <summary>
    /// Adds two coordinates component-wise.
    /// </summary>
    public static Coordinate3D<T> operator +(Coordinate3D<T> left, Coordinate3D<T> right) =>
        new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    /// <summary>
    /// Subtracts two coordinates component-wise.
    /// </summary>
    public static Coordinate3D<T> operator -(Coordinate3D<T> left, Coordinate3D<T> right) =>
        new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    /// <summary>
    /// Multiplies each component by a scalar.
    /// </summary>
    public static Coordinate3D<T> operator *(Coordinate3D<T> coordinate, T factor) =>
        new(coordinate.X * factor, coordinate.Y * factor, coordinate.Z * factor);

    /// <summary>
    /// Divides each component by a scalar.
    /// </summary>
    public static Coordinate3D<T> operator /(Coordinate3D<T> coordinate, T factor) =>
        new(coordinate.X / factor, coordinate.Y / factor, coordinate.Z / factor);

    /// <inheritdoc />
    public bool Equals(Coordinate3D<T> other) =>
        X == other.X && Y == other.Y && Z == other.Z;

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is Coordinate3D<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);
}
