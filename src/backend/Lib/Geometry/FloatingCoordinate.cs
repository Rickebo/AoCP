using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Two-dimensional coordinate for floating-point types with vector helpers.
/// </summary>
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
    /// Gets the Euclidean length of the vector.
    /// </summary>
    public T Length => T.Sqrt(X * X + Y * Y);

    /// <summary>
    /// Gets a coordinate with all components set to zero.
    /// </summary>
    public static FloatingCoordinate<T> Zero { get; } = new(T.Zero, T.Zero);

    /// <summary>
    /// Gets a coordinate with all components set to one.
    /// </summary>
    public static FloatingCoordinate<T> One { get; } = new(T.One, T.One);

    /// <summary>
    /// Gets the unit vector along the X axis.
    /// </summary>
    public static FloatingCoordinate<T> UnitX { get; } = new(T.One, T.Zero);

    /// <summary>
    /// Gets the unit vector along the Y axis.
    /// </summary>
    public static FloatingCoordinate<T> UnitY { get; } = new(T.Zero, T.One);

    /// <summary>
    /// Returns the component-wise minimum of this and another coordinate.
    /// </summary>
    /// <param name="other">Coordinate to compare with.</param>
    /// <returns>Minimum component values.</returns>
    public FloatingCoordinate<T> Min(FloatingCoordinate<T> other) =>
        new(T.Min(X, other.X), T.Min(Y, other.Y));

    /// <summary>
    /// Returns the component-wise maximum of this and another coordinate.
    /// </summary>
    /// <param name="other">Coordinate to compare with.</param>
    /// <returns>Maximum component values.</returns>
    public FloatingCoordinate<T> Max(FloatingCoordinate<T> other) =>
        new(T.Max(X, other.X), T.Max(Y, other.Y));

    /// <summary>
    /// Clamps components between the provided minimum and maximum values.
    /// </summary>
    /// <param name="min">Minimum allowed values.</param>
    /// <param name="max">Maximum allowed values.</param>
    /// <returns>Clamped coordinate.</returns>
    public FloatingCoordinate<T> Clamp(
        FloatingCoordinate<T> min,
        FloatingCoordinate<T> max
    ) => new(
        T.Clamp(X, min.X, max.X),
        T.Clamp(Y, min.Y, max.Y)
    );

    /// <summary>
    /// Copies sign information from another coordinate.
    /// </summary>
    /// <param name="sign">Coordinate providing sign.</param>
    /// <returns>Coordinate with magnitudes from this instance and signs from <paramref name="sign"/>.</returns>
    public FloatingCoordinate<T> CopySign(FloatingCoordinate<T> sign) => new(
        T.CopySign(X, sign.X),
        T.CopySign(Y, sign.Y)
    );

    /// <summary>
    /// Takes the absolute value of each component.
    /// </summary>
    /// <returns>Coordinate with absolute components.</returns>
    public FloatingCoordinate<T> Abs() => new(T.Abs(X), T.Abs(Y));

    /// <summary>
    /// Negates both components.
    /// </summary>
    /// <param name="coordinate">Coordinate to negate.</param>
    /// <returns>Negated coordinate.</returns>
    public static FloatingCoordinate<T> operator -(FloatingCoordinate<T> coordinate) =>
        new(-coordinate.X, -coordinate.Y);

    /// <summary>
    /// Adds two coordinates component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Sum of the coordinates.</returns>
    public static FloatingCoordinate<T> operator +(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// Subtracts one coordinate from another component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Difference of the coordinates.</returns>
    public static FloatingCoordinate<T> operator -(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// Determines whether two coordinates are equal.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><c>true</c> when both components match; otherwise <c>false</c>.</returns>
    public static bool operator ==(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        left.X == right.X && left.Y == right.Y;

    /// <summary>
    /// Determines whether two coordinates differ.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><c>true</c> when any component differs; otherwise <c>false</c>.</returns>
    public static bool operator !=(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        !(left == right);

    /// <summary>
    /// Multiplies the coordinate by a scalar factor.
    /// </summary>
    /// <param name="coordinate">Coordinate to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled coordinate.</returns>
    public static FloatingCoordinate<T> operator *(
        FloatingCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X * factor, coordinate.Y * factor);

    /// <summary>
    /// Divides the coordinate by a scalar factor.
    /// </summary>
    /// <param name="coordinate">Coordinate to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled coordinate.</returns>
    public static FloatingCoordinate<T> operator /(
        FloatingCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    /// <summary>
    /// Multiplies coordinates component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Component-wise product.</returns>
    public static FloatingCoordinate<T> operator *(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X * right.X, left.Y * right.Y);

    /// <summary>
    /// Divides coordinates component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Component-wise quotient.</returns>
    public static FloatingCoordinate<T> operator /(
        FloatingCoordinate<T> left,
        FloatingCoordinate<T> right
    ) =>
        new(left.X / right.X, left.Y / right.Y);

    /// <summary>
    /// Determines equality with another coordinate.
    /// </summary>
    /// <param name="other">Coordinate to compare.</param>
    /// <returns><c>true</c> when the components match; otherwise <c>false</c>.</returns>
    public bool Equals(FloatingCoordinate<T> other) => X == other.X && Y == other.Y;

    /// <summary>
    /// Determines equality with another object.
    /// </summary>
    /// <param name="obj">Object to compare.</param>
    /// <returns><c>true</c> when the object is a matching coordinate.</returns>
    public override bool Equals(object? obj) =>
        obj is FloatingCoordinate<T> other && Equals(other);

    /// <summary>
    /// Generates a hash code from the component values.
    /// </summary>
    /// <returns>Hash code for the coordinate.</returns>
    public override int GetHashCode() => HashCode.Combine(X, Y);

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
}
