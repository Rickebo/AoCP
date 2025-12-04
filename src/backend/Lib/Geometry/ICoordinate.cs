using System.Numerics;

namespace Lib.Geometry;

public interface ICoordinate<TCoordinate, TNumber> : IStringCoordinate
    where TCoordinate : ICoordinate<TCoordinate, TNumber>
    where TNumber : INumber<TNumber>
{
    /// <summary>
    /// Gets the x-component.
    /// </summary>
    TNumber X { get; }

    /// <summary>
    /// Gets the y-component.
    /// </summary>
    TNumber Y { get; }

    /// <summary>
    /// Gets a coordinate with all components set to zero.
    /// </summary>
    static abstract TCoordinate Zero { get; }

    /// <summary>
    /// Gets a coordinate with all components set to one.
    /// </summary>
    static abstract TCoordinate One { get; }

    /// <summary>
    /// Gets the unit vector along the X axis.
    /// </summary>
    static abstract TCoordinate UnitX { get; }

    /// <summary>
    /// Gets the unit vector along the Y axis.
    /// </summary>
    static abstract TCoordinate UnitY { get; }

    /// <summary>
    /// Returns the component-wise minimum between this coordinate and another.
    /// </summary>
    /// <param name="other">Coordinate to compare with.</param>
    /// <returns>The minimum coordinate values.</returns>
    TCoordinate Min(TCoordinate other);

    /// <summary>
    /// Returns the component-wise maximum between this coordinate and another.
    /// </summary>
    /// <param name="other">Coordinate to compare with.</param>
    /// <returns>The maximum coordinate values.</returns>
    TCoordinate Max(TCoordinate other);

    /// <summary>
    /// Clamps each component between the provided minimum and maximum coordinates.
    /// </summary>
    /// <param name="min">Minimum values.</param>
    /// <param name="max">Maximum values.</param>
    /// <returns>The clamped coordinate.</returns>
    TCoordinate Clamp(TCoordinate min, TCoordinate max);

    /// <summary>
    /// Copies the sign of each component from another coordinate.
    /// </summary>
    /// <param name="sign">Coordinate providing sign information.</param>
    /// <returns>Coordinate with magnitudes from this and signs from <paramref name="sign"/>.</returns>
    TCoordinate CopySign(TCoordinate sign);

    /// <summary>
    /// Returns the absolute value of each component.
    /// </summary>
    /// <returns>Component-wise absolute value.</returns>
    TCoordinate Abs();

    /// <summary>
    /// Computes the Manhattan length (sum of absolute components).
    /// </summary>
    /// <returns>Sum of <see cref="X"/> and <see cref="Y"/>.</returns>
    TNumber ManhattanLength() => X + Y;
}

public interface ICoordinate3D<TCoordinate, TNumber> : ICoordinate<TCoordinate, TNumber>
    where TCoordinate : ICoordinate<TCoordinate, TNumber>
    where TNumber : INumber<TNumber>
{
    /// <summary>
    /// Gets the z-component.
    /// </summary>
    TNumber Z { get; }

    /// <summary>
    /// Gets the z-component as a string for display purposes.
    /// </summary>
    string? GetStringZ() => Z.ToString();
}
