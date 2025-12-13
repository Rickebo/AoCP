namespace Lib.Geometry;

/// <summary>
/// Provides helper extensions for converting and applying <see cref="Rotation"/> values.
/// </summary>
public static class RotationExtensions
{
    /// <summary>
    /// Gets the numeric sign that corresponds to a rotation direction.
    /// </summary>
    /// <param name="rotation">The rotation direction.</param>
    /// <returns>1 for clockwise, -1 for counter-clockwise, or 0 for no rotation.</returns>
    public static int Sign(this Rotation rotation) => rotation switch
    {
        Rotation.Clockwise => 1,
        Rotation.CounterClockwise => -1,
        _ => 0
    };

    /// <summary>
    /// Returns the opposite rotation direction, preserving <see cref="Rotation.None"/>.
    /// </summary>
    /// <param name="rotation">The rotation direction to invert.</param>
    /// <returns>The inverted rotation direction, or <see cref="Rotation.None"/> when no rotation was provided.</returns>
    public static Rotation Invert(this Rotation rotation) => rotation switch
    {
        Rotation.Clockwise => Rotation.CounterClockwise,
        Rotation.CounterClockwise => Rotation.Clockwise,
        _ => Rotation.None
    };

    /// <summary>
    /// Converts an integer sign into a <see cref="Rotation"/> direction.
    /// </summary>
    /// <param name="sign">The numeric sign to convert.</param>
    /// <returns><see cref="Rotation.Clockwise"/> for positive values, <see cref="Rotation.CounterClockwise"/> for negative values, or <see cref="Rotation.None"/> for zero.</returns>
    public static Rotation FromSign(int sign) => sign switch
    {
        > 0 => Rotation.Clockwise,
        < 0 => Rotation.CounterClockwise,
        _ => Rotation.None
    };

    /// <summary>
    /// Parses a rotation glyph into a <see cref="Rotation"/> direction.
    /// </summary>
    /// <param name="value">The glyph to parse (case-insensitive 'L' or 'R').</param>
    /// <returns>The parsed rotation direction.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the glyph is not 'L' or 'R'.</exception>
    public static Rotation Parse(char value) => value switch
    {
        'R' or 'r' => Rotation.Clockwise,
        'L' or 'l' => Rotation.CounterClockwise,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Rotation must be 'L' or 'R'.")
    };

    /// <summary>
    /// Converts a <see cref="Rotation"/> direction to its uppercase glyph representation.
    /// </summary>
    /// <param name="rotation">The rotation direction.</param>
    /// <returns>'R' for clockwise, 'L' for counter-clockwise, or '-' when no rotation is specified.</returns>
    public static char ToGlyph(this Rotation rotation) => rotation switch
    {
        Rotation.Clockwise => 'R',
        Rotation.CounterClockwise => 'L',
        _ => '-'
    };

    /// <summary>
    /// Applies a rotation to an <see cref="Angle"/> using the provided step.
    /// </summary>
    /// <param name="rotation">The rotation direction to apply.</param>
    /// <param name="angle">The starting angle.</param>
    /// <param name="step">The angular step to rotate by. Defaults to <see cref="Angle.QuarterTurn"/>.</param>
    /// <returns>The resulting angle after the rotation is applied.</returns>
    public static Angle ApplyTo(this Rotation rotation, Angle angle, Angle step = Angle.QuarterTurn) => rotation switch
    {
        Rotation.Clockwise => angle.Add(step),
        Rotation.CounterClockwise => angle.Subtract(step),
        _ => angle
    };
}
