using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Represents a 2D displacement with arithmetic helpers.
/// </summary>
public readonly struct Distance<T>(T x, T y) where T : INumber<T>
{
    /// <summary>
    /// Gets the X component of the displacement.
    /// </summary>
    public T X { get; } = x;

    /// <summary>
    /// Gets the Y component of the displacement.
    /// </summary>
    public T Y { get; } = y;

    /// <summary>
    /// Gets a zero distance.
    /// </summary>
    public static Distance<T> Zero => new(T.Zero, T.Zero);

    /// <summary>
    /// Computes the Manhattan length of the distance.
    /// </summary>
    /// <returns>Sum of the absolute component values.</returns>
    public T Manhattan() => T.Abs(X) + T.Abs(Y);

    /// <summary>
    /// Takes the absolute value of each component.
    /// </summary>
    /// <returns>Distance with absolute components.</returns>
    public Distance<T> Abs() => new(T.Abs(X), T.Abs(Y));

    /// <summary>
    /// Adds two distances component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Combined distance.</returns>
    public static Distance<T> operator +(Distance<T> left, Distance<T> right) =>
        new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// Subtracts one distance from another component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Resulting distance.</returns>
    public static Distance<T> operator -(Distance<T> left, Distance<T> right) =>
        new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// Determines whether two distances are equal.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><c>true</c> when both components match; otherwise <c>false</c>.</returns>
    public static bool operator ==(Distance<T> left, Distance<T> right) =>
        left.X == right.X && left.Y == right.Y;

    /// <summary>
    /// Determines whether two distances differ.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><c>true</c> when any component differs; otherwise <c>false</c>.</returns>
    public static bool operator !=(Distance<T> left, Distance<T> right) =>
        !(left == right);

    /// <summary>
    /// Multiplies the distance by a scalar factor.
    /// </summary>
    /// <param name="coordinate">Distance to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled distance.</returns>
    public static Distance<T> operator *(Distance<T> coordinate, T factor) =>
    new(coordinate.X * factor, coordinate.Y * factor);

    /// <summary>
    /// Divides the distance by a scalar factor.
    /// </summary>
    /// <param name="coordinate">Distance to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled distance.</returns>
    public static Distance<T> operator /(Distance<T> coordinate, T factor) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    /// <summary>
    /// Multiplies two distances component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Component-wise product.</returns>
    public static Distance<T> operator *(Distance<T> left, Distance<T> right) =>
        new(left.X * right.X, left.Y * right.Y);

    /// <summary>
    /// Divides two distances component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Component-wise quotient.</returns>
    public static Distance<T> operator /(Distance<T> left, Distance<T> right) =>
        new(left.X / right.X, left.Y / right.Y);

    /// <summary>
    /// Determines equality with another distance.
    /// </summary>
    /// <param name="other">Distance to compare.</param>
    /// <returns><c>true</c> when the components match; otherwise <c>false</c>.</returns>
    public bool Equals(Distance<T> other) =>
        X == other.X && Y == other.Y;

    /// <summary>
    /// Determines equality with another object.
    /// </summary>
    /// <param name="obj">Object to compare.</param>
    /// <returns><c>true</c> when the object is a matching distance.</returns>
    public override bool Equals(object? obj) =>
        obj is Distance<T> other && Equals(other);

    /// <summary>
    /// Generates a hash code from the component values.
    /// </summary>
    /// <returns>Hash code for the distance.</returns>
    public override int GetHashCode() => HashCode.Combine(X, Y);
}


