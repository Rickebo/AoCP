using Lib.Geometry;
using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Generic three-dimensional coordinate with arithmetic helpers.
/// </summary>
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
    /// Gets a coordinate with all components set to zero.
    /// </summary>
    public static Coordinate3D<T> Zero => new(T.Zero, T.Zero, T.Zero);

    /// <summary>
    /// Gets a coordinate with all components set to one.
    /// </summary>
    public static Coordinate3D<T> One => new(T.One, T.One, T.One);

    /// <summary>
    /// Gets the unit vector along the X axis.
    /// </summary>
    public static Coordinate3D<T> UnitX => new(T.One, T.Zero, T.Zero);

    /// <summary>
    /// Gets the unit vector along the Y axis.
    /// </summary>
    public static Coordinate3D<T> UnitY => new(T.Zero, T.One, T.Zero);

    /// <summary>
    /// Gets the unit vector along the Z axis.
    /// </summary>
    public static Coordinate3D<T> UnitZ => new(T.Zero, T.Zero, T.One);

    /// <summary>
    /// Gets the string representation of <see cref="X"/>.
    /// </summary>
    /// <returns>String form of the X component.</returns>
    public string? GetStringX() => X.ToString();

    /// <summary>
    /// Gets the string representation of <see cref="Y"/>.
    /// </summary>
    /// <returns>String form of the Y component.</returns>
    public string? GetStringY() => Y.ToString();

    /// <summary>
    /// Gets the string representation of <see cref="Z"/>.
    /// </summary>
    /// <returns>String form of the Z component.</returns>
    public string? GetStringZ() => Z.ToString();

    /// <summary>
    /// Copies the sign of each component from another coordinate.
    /// </summary>
    /// <param name="sign">Coordinate providing sign information.</param>
    /// <returns>A coordinate whose signs match <paramref name="sign"/>.</returns>
    public Coordinate3D<T> CopySign(Coordinate3D<T> sign) => new(
        T.CopySign(X, sign.X),
        T.CopySign(Y, sign.Y),
        T.CopySign(Z, sign.Z)
    );

    /// <summary>
    /// Returns the absolute value of each component.
    /// </summary>
    /// <returns>Coordinate with absolute components.</returns>
    public Coordinate3D<T> Abs() => new(T.Abs(X), T.Abs(Y), T.Abs(Z));

    /// <summary>
    /// Computes the Manhattan length (sum of absolute component values).
    /// </summary>
    /// <returns>Manhattan length.</returns>
    public T ManhattanLength() => T.Abs(X) + T.Abs(Y) + T.Abs(Z);

    /// <summary>
    /// Returns the component-wise minimum between this coordinate and another.
    /// </summary>
    /// <param name="other">Coordinate to compare with.</param>
    /// <returns>Minimum coordinate values.</returns>
    public Coordinate3D<T> Min(Coordinate3D<T> other) => new(
        T.Min(X, other.X),
        T.Min(Y, other.Y),
        T.Min(Z, other.Z)
    );

    /// <summary>
    /// Returns the component-wise maximum between this coordinate and another.
    /// </summary>
    /// <param name="other">Coordinate to compare with.</param>
    /// <returns>Maximum coordinate values.</returns>
    public Coordinate3D<T> Max(Coordinate3D<T> other) => new(
        T.Max(X, other.X),
        T.Max(Y, other.Y),
        T.Max(Z, other.Z)
    );

    /// <summary>
    /// Clamps each component between the provided minimum and maximum values.
    /// </summary>
    /// <param name="min">Minimum allowed values.</param>
    /// <param name="max">Maximum allowed values.</param>
    /// <returns>The clamped coordinate.</returns>
    public Coordinate3D<T> Clamp(Coordinate3D<T> min, Coordinate3D<T> max) => new(
        T.Clamp(X, min.X, max.X),
        T.Clamp(Y, min.Y, max.Y),
        T.Clamp(Z, min.Z, max.Z)
    );

    /// <summary>
    /// Adds two coordinates component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Sum of the coordinates.</returns>
    public static Coordinate3D<T> operator +(Coordinate3D<T> left, Coordinate3D<T> right) =>
        new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    /// <summary>
    /// Subtracts one coordinate from another component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Difference of the coordinates.</returns>
    public static Coordinate3D<T> operator -(Coordinate3D<T> left, Coordinate3D<T> right) =>
        new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    /// <summary>
    /// Multiplies all components by a scalar factor.
    /// </summary>
    /// <param name="coordinate">Coordinate to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled coordinate.</returns>
    public static Coordinate3D<T> operator *(Coordinate3D<T> coordinate, T factor) =>
        new(coordinate.X * factor, coordinate.Y * factor, coordinate.Z * factor);

    /// <summary>
    /// Divides all components by a scalar factor.
    /// </summary>
    /// <param name="coordinate">Coordinate to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled coordinate.</returns>
    public static Coordinate3D<T> operator /(Coordinate3D<T> coordinate, T factor) =>
        new(coordinate.X / factor, coordinate.Y / factor, coordinate.Z / factor);

    /// <summary>
    /// Determines equality with another coordinate.
    /// </summary>
    /// <param name="other">Coordinate to compare.</param>
    /// <returns><c>true</c> when all components match; otherwise <c>false</c>.</returns>
    public bool Equals(Coordinate3D<T> other) =>
        X == other.X && Y == other.Y && Z == other.Z;

    /// <summary>
    /// Determines equality with another object.
    /// </summary>
    /// <param name="obj">Object to compare.</param>
    /// <returns><c>true</c> when the object is a coordinate with matching components.</returns>
    public override bool Equals(object? obj) =>
        obj is Coordinate3D<T> other && Equals(other);

    /// <summary>
    /// Generates a hash code from component values.
    /// </summary>
    /// <returns>Hash code for the coordinate.</returns>
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);
}
