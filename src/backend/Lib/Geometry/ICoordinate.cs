using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Defines a numeric 2D coordinate with helper members for common operations.
/// </summary>
/// <typeparam name="TCoordinate">Concrete coordinate type.</typeparam>
/// <typeparam name="TNumber">Numeric component type.</typeparam>
public interface ICoordinate<TCoordinate, TNumber> : IStringCoordinate
    where TCoordinate : ICoordinate<TCoordinate, TNumber>
    where TNumber : INumber<TNumber>
{
    /// <summary>
    /// X component.
    /// </summary>
    TNumber X { get; }

    /// <summary>
    /// Y component.
    /// </summary>
    TNumber Y { get; }

    /// <summary>
    /// Coordinate at (0, 0).
    /// </summary>
    static abstract TCoordinate Zero { get; }

    /// <summary>
    /// Coordinate at (1, 1).
    /// </summary>
    static abstract TCoordinate One { get; }

    /// <summary>
    /// Unit vector along X.
    /// </summary>
    static abstract TCoordinate UnitX { get; }

    /// <summary>
    /// Unit vector along Y.
    /// </summary>
    static abstract TCoordinate UnitY { get; }

    /// <summary>
    /// Component-wise minimum.
    /// </summary>
    TCoordinate Min(TCoordinate other);

    /// <summary>
    /// Component-wise maximum.
    /// </summary>
    TCoordinate Max(TCoordinate other);

    /// <summary>
    /// Clamps each component between bounds.
    /// </summary>
    TCoordinate Clamp(TCoordinate min, TCoordinate max);

    /// <summary>
    /// Copies the sign of another coordinate.
    /// </summary>
    TCoordinate CopySign(TCoordinate sign);

    /// <summary>
    /// Returns the absolute value of each component.
    /// </summary>
    TCoordinate Abs();

    /// <summary>
    /// Computes the Manhattan length of the coordinate.
    /// </summary>
    TNumber ManhattanLength() => TNumber.Abs(X) + TNumber.Abs(Y);
}

/// <summary>
/// Defines a numeric 3D coordinate with string accessors for each component.
/// </summary>
/// <typeparam name="TCoordinate">Concrete coordinate type.</typeparam>
/// <typeparam name="TNumber">Numeric component type.</typeparam>
public interface ICoordinate3D<TCoordinate, TNumber> : ICoordinate<TCoordinate, TNumber>
    where TCoordinate : ICoordinate<TCoordinate, TNumber>
    where TNumber : INumber<TNumber>
{
    /// <summary>
    /// Z component.
    /// </summary>
    TNumber Z { get; }

    /// <summary>
    /// Returns the Z component as a string.
    /// </summary>
    string? GetStringZ() => Z.ToString();
}
