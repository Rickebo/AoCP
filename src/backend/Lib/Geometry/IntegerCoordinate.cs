using System.Numerics;
using Lib.Math;

namespace Lib.Geometry;

/// <summary>
/// Integer-based 2D coordinate with helpers for neighbourhood traversal and arithmetic.
/// </summary>
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
    /// Gets a coordinate with all components set to zero.
    /// </summary>
    public static IntegerCoordinate<T> Zero { get; } = new(T.Zero, T.Zero);

    /// <summary>
    /// Gets a coordinate with all components set to one.
    /// </summary>
    public static IntegerCoordinate<T> One { get; } = new(T.One, T.One);

    /// <summary>
    /// Gets the unit vector along the X axis.
    /// </summary>
    public static IntegerCoordinate<T> UnitX { get; } = new(T.One, T.Zero);

    /// <summary>
    /// Gets the unit vector along the Y axis.
    /// </summary>
    public static IntegerCoordinate<T> UnitY { get; } = new(T.Zero, T.One);

    /// <summary>
    /// Enumerates neighbouring coordinates according to the chosen neighbourhood mask, excluding any specified directions.
    /// </summary>
    /// <param name="neighbourhood">Neighbourhood mask (cardinal or diagonal).</param>
    /// <param name="excluded">Directions to omit.</param>
    /// <returns>Neighbouring coordinates.</returns>
    public IEnumerable<IntegerCoordinate<T>> Neighbours(
        Neighbourhood neighbourhood = Neighbourhood.Cardinal,
        Direction excluded = Direction.None) =>
        Neighbourhoods.Neighbours(this, neighbourhood, excluded);

    /// <summary>
    /// Computes the Manhattan length (sum of absolute component values).
    /// </summary>
    /// <returns>Manhattan length.</returns>
    public T ManhattanLength() => T.Abs(X) + T.Abs(Y);

    /// <summary>
    /// Computes the component-wise distance vector to another coordinate.
    /// </summary>
    /// <param name="other">Destination coordinate.</param>
    /// <returns>Distance from this coordinate to <paramref name="other"/>.</returns>
    public Distance<T> Distance(IntegerCoordinate<T> other)
        => new(other.X - X, other.Y - Y);

    /// <summary>
    /// Moves the coordinate by a given distance vector.
    /// </summary>
    /// <param name="distance">Distance to move.</param>
    /// <returns>New coordinate offset by <paramref name="distance"/>.</returns>
    public IntegerCoordinate<T> Move(Distance<T> distance) =>
        new(X + distance.X, Y + distance.Y);

    /// <summary>
    /// Computes the component-wise minimum between this coordinate and another.
    /// </summary>
    /// <param name="other">Coordinate to compare with.</param>
    /// <returns>Minimum coordinate values.</returns>
    public IntegerCoordinate<T> Min(IntegerCoordinate<T> other) =>
        new(T.Min(X, other.X), T.Min(Y, other.Y));

    /// <summary>
    /// Computes the component-wise maximum between this coordinate and another.
    /// </summary>
    /// <param name="other">Coordinate to compare with.</param>
    /// <returns>Maximum coordinate values.</returns>
    public IntegerCoordinate<T> Max(IntegerCoordinate<T> other) =>
        new(T.Max(X, other.X), T.Max(Y, other.Y));

    /// <summary>
    /// Clamps each component between the provided minimum and maximum coordinates.
    /// </summary>
    /// <param name="min">Minimum values.</param>
    /// <param name="max">Maximum values.</param>
    /// <returns>Clamped coordinate.</returns>
    public IntegerCoordinate<T> Clamp(
        IntegerCoordinate<T> min,
        IntegerCoordinate<T> max
    ) => new(T.Clamp(X, min.X, max.X), T.Clamp(Y, min.Y, max.Y));

    /// <summary>
    /// Copies the sign of each component from another coordinate.
    /// </summary>
    /// <param name="sign">Coordinate providing sign.</param>
    /// <returns>A coordinate whose magnitudes come from this instance and signs from <paramref name="sign"/>.</returns>
    public IntegerCoordinate<T> CopySign(IntegerCoordinate<T> sign) =>
        new(T.CopySign(X, sign.X), T.CopySign(Y, sign.Y));

    /// <summary>
    /// Takes the absolute value of each component.
    /// </summary>
    /// <returns>Coordinate with absolute components.</returns>
    public IntegerCoordinate<T> Abs() =>
        new(T.Abs(X), T.Abs(Y));

    /// <summary>
    /// Moves the coordinate one step in the specified direction.
    /// </summary>
    /// <param name="direction">Direction to move.</param>
    /// <returns>New coordinate after moving.</returns>
    public IntegerCoordinate<T> Move(Direction direction) =>
        this + direction.ToCoordinate<T>();

    /// <summary>
    /// Determines the direction from this coordinate toward another, clamped to a single step.
    /// </summary>
    /// <param name="other">Target coordinate.</param>
    /// <returns>Direction vector with components in [-1, 1].</returns>
    public IntegerCoordinate<T> DirectionOf(IntegerCoordinate<T> other)
    {
        var dx = other.X - X;
        var dy = other.Y - Y;

        return new IntegerCoordinate<T>(dx, dy).Clamp(-One, One);
    }

    /// <summary>
    /// Enumerates coordinates from this point toward another, stepping one cell at a time.
    /// </summary>
    /// <param name="other">Destination coordinate.</param>
    /// <returns>Sequence of coordinates starting at the current position and moving toward <paramref name="other"/>.</returns>
    public IEnumerable<IntegerCoordinate<T>> MoveTo(IntegerCoordinate<T> other)
    {
        for (var src = this; src != other; src += src.DirectionOf(other))
            yield return src;
    }

    /// <summary>
    /// Applies a modulo operation to both components.
    /// </summary>
    /// <param name="other">Divisors for each component.</param>
    /// <returns>Coordinate containing the modulo results.</returns>
    public IntegerCoordinate<T> Modulo(IntegerCoordinate<T> other) => new(
        MathExtensions.Modulo(X, other.X),
        MathExtensions.Modulo(Y, other.Y)
    );

    /// <summary>
    /// Applies the remainder operator to both components.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Coordinate containing the remainders.</returns>
    public static IntegerCoordinate<T> operator %(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) => new(left.X % right.X, left.Y % right.Y);

    /// <summary>
    /// Negates both components.
    /// </summary>
    /// <param name="coordinate">Coordinate to negate.</param>
    /// <returns>Negated coordinate.</returns>
    public static IntegerCoordinate<T> operator -(IntegerCoordinate<T> coordinate) =>
        new(-coordinate.X, -coordinate.Y);

    /// <summary>
    /// Performs a bitwise AND on both components.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Coordinate containing the bitwise AND results.</returns>
    public static IntegerCoordinate<T> operator &(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X & right.X, left.Y & right.Y);

    /// <summary>
    /// Performs a bitwise OR on both components.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Coordinate containing the bitwise OR results.</returns>
    public static IntegerCoordinate<T> operator |(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X | right.X, left.Y | right.Y);

    /// <summary>
    /// Performs a bitwise XOR on both components.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Coordinate containing the bitwise XOR results.</returns>
    public static IntegerCoordinate<T> operator ^(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X ^ right.X, left.Y ^ right.Y);

    /// <summary>
    /// Performs a bitwise NOT on both components.
    /// </summary>
    /// <param name="coordinate">Coordinate to invert.</param>
    /// <returns>Coordinate with inverted bits.</returns>
    public static IntegerCoordinate<T> operator ~(IntegerCoordinate<T> coordinate) =>
        new(~coordinate.X, ~coordinate.Y);


    /// <summary>
    /// Adds two coordinates component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Sum of the coordinates.</returns>
    public static IntegerCoordinate<T> operator +(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// Subtracts one coordinate from another component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Difference of the coordinates.</returns>
    public static IntegerCoordinate<T> operator -(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// Determines whether two coordinates are equal.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><c>true</c> when both components match; otherwise <c>false</c>.</returns>
    public static bool operator ==(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        left.X == right.X && left.Y == right.Y;

    /// <summary>
    /// Determines whether two coordinates differ.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><c>true</c> when any component differs; otherwise <c>false</c>.</returns>
    public static bool operator !=(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        !(left == right);

    /// <summary>
    /// Multiplies both components by a scalar factor.
    /// </summary>
    /// <param name="coordinate">Coordinate to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled coordinate.</returns>
    public static IntegerCoordinate<T> operator *(
        IntegerCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X * factor, coordinate.Y * factor);

    /// <summary>
    /// Divides both components by a scalar factor.
    /// </summary>
    /// <param name="coordinate">Coordinate to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled coordinate.</returns>
    public static IntegerCoordinate<T> operator /(
        IntegerCoordinate<T> coordinate,
        T factor
    ) =>
        new(coordinate.X / factor, coordinate.Y / factor);

    /// <summary>
    /// Multiplies coordinates component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Component-wise product.</returns>
    public static IntegerCoordinate<T> operator *(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X * right.X, left.Y * right.Y);

    /// <summary>
    /// Divides coordinates component-wise.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>Component-wise quotient.</returns>
    public static IntegerCoordinate<T> operator /(
        IntegerCoordinate<T> left,
        IntegerCoordinate<T> right
    ) =>
        new(left.X / right.X, left.Y / right.Y);

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
    /// Determines equality with another coordinate.
    /// </summary>
    /// <param name="other">Coordinate to compare.</param>
    /// <returns><c>true</c> when both components match; otherwise <c>false</c>.</returns>
    public bool Equals(IntegerCoordinate<T> other) => X == other.X && Y == other.Y;

    /// <summary>
    /// Determines equality with another object.
    /// </summary>
    /// <param name="obj">Object to compare.</param>
    /// <returns><c>true</c> when the object is a matching coordinate.</returns>
    public override bool Equals(object? obj) =>
        obj is IntegerCoordinate<T> other && Equals(other);

    /// <summary>
    /// Generates a hash code from the components.
    /// </summary>
    /// <returns>Hash code for the coordinate.</returns>
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <summary>
    /// Returns a formatted string representing the coordinate.
    /// </summary>
    /// <returns>String representation of the coordinate.</returns>
    public override string ToString() => $"<{X} {Y}>";
}
