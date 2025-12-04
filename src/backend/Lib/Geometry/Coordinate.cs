using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Generic two-dimensional coordinate with arithmetic helpers.
/// </summary>
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
    /// Gets a coordinate with both components set to zero.
    /// </summary>
    public static Coordinate<T> Zero => new(T.Zero, T.Zero);

    /// <summary>
    /// Gets a coordinate with both components set to one.
    /// </summary>
    public static Coordinate<T> One { get; } = new(T.One, T.One);

    /// <summary>
    /// Gets the unit vector pointing along the X axis.
    /// </summary>
    public static Coordinate<T> UnitX { get; } = new(T.One, T.Zero);

    /// <summary>
    /// Gets the unit vector pointing along the Y axis.
    /// </summary>
    public static Coordinate<T> UnitY { get; } = new(T.Zero, T.One);

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
    /// Computes the component-wise minimum of this and another coordinate.
    /// </summary>
    /// <param name="other">Coordinate to compare with.</param>
    /// <returns>Coordinate containing the minimum components.</returns>
    public Coordinate<T> Min(Coordinate<T> other) =>
        new(T.Min(X, other.X), T.Min(Y, other.Y));

    /// <summary>
    /// Computes the component-wise maximum of this and another coordinate.
    /// </summary>
    /// <param name="other">Coordinate to compare with.</param>
    /// <returns>Coordinate containing the maximum components.</returns>
    public Coordinate<T> Max(Coordinate<T> other) =>
        new(T.Max(X, other.X), T.Max(Y, other.Y));

    /// <summary>
    /// Clamps each component between the provided minimum and maximum coordinates.
    /// </summary>
    /// <param name="min">Minimum allowed values.</param>
    /// <param name="max">Maximum allowed values.</param>
    /// <returns>The clamped coordinate.</returns>
    public Coordinate<T> Clamp(Coordinate<T> min, Coordinate<T> max) => new(
        T.Clamp(X, min.X, max.X),
        T.Clamp(Y, min.Y, max.Y)
    );

    /// <summary>
    /// Copies the sign of each component from another coordinate.
    /// </summary>
    /// <param name="sign">Coordinate providing sign for each axis.</param>
    /// <returns>A coordinate whose magnitudes match this coordinate and signs match <paramref name="sign"/>.</returns>
    public Coordinate<T> CopySign(Coordinate<T> sign) => new(
        T.CopySign(X, sign.X),
        T.CopySign(Y, sign.Y)
    );

    /// <summary>
    /// Takes the absolute value of each component.
    /// </summary>
    /// <returns>Coordinate with absolute components.</returns>
    public Coordinate<T> Abs() => new(T.Abs(X), T.Abs(Y));

    /// <summary>
    /// Adds two coordinates component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Sum of the coordinates.</returns>
    public static Coordinate<T> operator +(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// Subtracts one coordinate from another component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Difference of the coordinates.</returns>
    public static Coordinate<T> operator -(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// Determines whether two coordinates are equal.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><c>true</c> if the components match; otherwise <c>false</c>.</returns>
    public static bool operator ==(Coordinate<T> left, Coordinate<T> right) =>
        left.X == right.X && left.Y == right.Y;

    /// <summary>
    /// Determines whether two coordinates are not equal.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><c>true</c> if any component differs; otherwise <c>false</c>.</returns>
    public static bool operator !=(Coordinate<T> left, Coordinate<T> right) =>
        !(left == right);

    /// <summary>
    /// Multiplies each component by a scalar factor.
    /// </summary>
    /// <param name="coordinate">Coordinate to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled coordinate.</returns>
    public static Coordinate<T> operator *(Coordinate<T> coordinate, T factor) =>
        new(coordinate.X * factor, coordinate.Y * factor);

    /// <summary>
    /// Divides each component by a scalar factor.
    /// </summary>
    /// <param name="coordinate">Coordinate to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled coordinate.</returns>
    public static Coordinate<T> operator /(Coordinate<T> coordinate, T factor) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    /// <summary>
    /// Multiplies coordinates component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Component-wise product.</returns>
    public static Coordinate<T> operator *(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X * right.X, left.Y * right.Y);

    /// <summary>
    /// Divides coordinates component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Component-wise quotient.</returns>
    public static Coordinate<T> operator /(Coordinate<T> left, Coordinate<T> right) =>
        new(left.X / right.X, left.Y / right.Y);

    /// <summary>
    /// Determines equality with another coordinate.
    /// </summary>
    /// <param name="other">Coordinate to compare.</param>
    /// <returns><c>true</c> when components match; otherwise <c>false</c>.</returns>
    public bool Equals(Coordinate<T> other) => X == other.X && Y == other.Y;

    /// <summary>
    /// Determines equality with another object.
    /// </summary>
    /// <param name="obj">Object to compare.</param>
    /// <returns><c>true</c> when the object is a coordinate with matching components.</returns>
    public override bool Equals(object? obj) =>
        obj is Coordinate<T> other && Equals(other);

    /// <summary>
    /// Generates a hash code from the component values.
    /// </summary>
    /// <returns>Hash code for the coordinate.</returns>
    public override int GetHashCode() => HashCode.Combine(X, Y);
}


