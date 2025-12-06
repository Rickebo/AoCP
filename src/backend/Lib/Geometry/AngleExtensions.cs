using Lib.Math;

namespace Lib.Geometry;

/// <summary>
/// Helpers for converting and manipulating <see cref="Angle"/> values.
/// </summary>
public static class AngleExtensions
{
/// <summary>
/// Converts a bit-flag angle to degrees.
/// </summary>
/// <param name="angle">Angle to convert.</param>
/// <returns>Degrees represented by the flags.</returns>
public static int ToDegrees(this Angle angle)
    {
        int degrees = 0;
        if (angle.HasFlag(Angle.EighthTurn)) degrees += 45;
        if (angle.HasFlag(Angle.QuarterTurn)) degrees += 90;
        if (angle.HasFlag(Angle.HalfTurn)) degrees += 180;
        return degrees;
    }

    /// <summary>
    /// Converts degrees to the corresponding <see cref="Angle"/> flags, normalizing to 0-359 degrees.
    /// </summary>
    /// <param name="degrees">Degrees to convert.</param>
    /// <returns>The normalized <see cref="Angle"/>.</returns>
    public static Angle ToAngle(this int degrees)
    {
        // Normalize to 0-359 range
        degrees = MathExtensions.Modulo(degrees, 360);

        Angle result = Angle.None;

        if (degrees >= 180)
        {
            result |= Angle.HalfTurn;
            degrees -= 180;
        }
        if (degrees >= 90)
        {
            result |= Angle.QuarterTurn;
            degrees -= 90;
        }
        if (degrees >= 45)
        {
            result |= Angle.EighthTurn;
        }

        return result;
    }

    /// <summary>
    /// Adds two angles and normalizes the result.
    /// </summary>
    public static Angle Add(this Angle angle, Angle other)
        => (angle.ToDegrees() + other.ToDegrees()).ToAngle();

    /// <summary>
    /// Subtracts <paramref name="other"/> from <paramref name="angle"/> and normalizes the result.
    /// </summary>
    public static Angle Subtract(this Angle angle, Angle other)
        => (angle.ToDegrees() - other.ToDegrees()).ToAngle();

    /// <summary>
    /// Rotates an angle clockwise by the specified <paramref name="rotation"/>.
    /// </summary>
    public static Angle RotateBy(this Angle angle, Angle rotation)
        => angle.Add(rotation);

    /// <summary>
    /// Rotates clockwise by the specified amount.
    /// </summary>
    public static Angle RotateClockwise(this Angle angle, Angle amount)
        => angle.Add(amount);

    /// <summary>
    /// Rotates counter-clockwise by the specified amount.
    /// </summary>
    public static Angle RotateCounterClockwise(this Angle angle, Angle amount)
        => angle.Subtract(amount);

    /// <summary>
    /// Returns the opposite angle (adds 180 degrees).
    /// </summary>
    public static Angle Opposite(this Angle angle)
        => angle.Add(Angle.HalfTurn);

    /// <summary>
    /// Negates an angle.
    /// </summary>
    public static Angle Negate(this Angle angle)
        => (-angle.ToDegrees()).ToAngle();

    /// <summary>
    /// Normalizes an angle to the nearest valid flag representation.
    /// </summary>
    public static Angle Normalize(this Angle angle)
        => angle.ToDegrees().ToAngle();

    /// <summary>
    /// Determines if the angle is cardinal (N, E, S, W).
    /// </summary>
    public static bool IsCardinal(this Angle angle)
        => !angle.HasFlag(Angle.EighthTurn);

    /// <summary>
    /// Determines if the angle is diagonal.
    /// </summary>
    public static bool IsDiagonal(this Angle angle)
        => angle.HasFlag(Angle.EighthTurn);

    /// <summary>
    /// Checks whether the angle is horizontal (east or west).
    /// </summary>
    public static bool IsHorizontal(this Angle angle)
        => angle == Angle.None || angle == Angle.HalfTurn;

    /// <summary>
    /// Checks whether the angle is vertical (north or south).
    /// </summary>
    public static bool IsVertical(this Angle angle)
        => angle == Angle.QuarterTurn || angle == (Angle.QuarterTurn | Angle.HalfTurn);

    /// <summary>
    /// Determines whether two angles are perpendicular.
    /// </summary>
    /// <param name="angle">First angle.</param>
    /// <param name="other">Second angle.</param>
    /// <returns><see langword="true"/> when the angles differ by 90 degrees.</returns>
    public static bool IsPerpendicularTo(this Angle angle, Angle other)
    {
        int diff = System.Math.Abs(angle.ToDegrees() - other.ToDegrees());
        return diff == 90 || diff == 270;
    }

    /// <summary>
    /// Determines whether two angles are parallel (same or opposite).
    /// </summary>
    public static bool IsParallelTo(this Angle angle, Angle other)
    {
        int diff = System.Math.Abs(angle.ToDegrees() - other.ToDegrees());
        return diff == 0 || diff == 180;
    }

    /// <summary>
    /// Returns the quadrant (1-4) for the angle or 0 if it lies on an axis.
    /// </summary>
    public static int GetQuadrant(this Angle angle)
    {
        int degrees = angle.ToDegrees();
        return degrees switch
        {
            > 0 and < 90 => 1,
            > 90 and < 180 => 2,
            > 180 and < 270 => 3,
            > 270 and < 360 => 4,
            _ => 0 // On an axis
        };
    }

    /// <summary>
    /// Computes the horizontal sign (-1, 0, 1) for the angle.
    /// </summary>
    public static int HorizontalSign(this Angle angle)
    {
        int degrees = angle.ToDegrees();
        if (degrees == 90 || degrees == 270) return 0;
        return (degrees > 90 && degrees < 270) ? -1 : 1;
    }

    /// <summary>
    /// Computes the vertical sign (-1, 0, 1) for the angle.
    /// </summary>
    public static int VerticalSign(this Angle angle)
    {
        int degrees = angle.ToDegrees();
        if (degrees == 0 || degrees == 180) return 0;
        return (degrees > 180) ? -1 : 1;
    }

    /// <summary>
    /// Finds the shortest signed distance in degrees to another angle.
    /// </summary>
    /// <param name="from">Starting angle.</param>
    /// <param name="to">Target angle.</param>
    /// <returns>Degrees in the range [-180, 180].</returns>
    public static int ShortestDistanceTo(this Angle from, Angle to)
    {
        int diff = to.ToDegrees() - from.ToDegrees();

        // Normalize to -180 to 180 range
        while (diff > 180) diff -= 360;
        while (diff < -180) diff += 360;

        return diff;
    }

    /// <summary>
    /// Gets the rotation direction (-1, 0, 1) to move from one angle toward another.
    /// </summary>
    public static int GetRotationDirection(this Angle from, Angle to)
        => System.Math.Sign(from.ShortestDistanceTo(to));

    /// <summary>
    /// Linearly interpolates between two angles.
    /// </summary>
    /// <param name="from">Starting angle.</param>
    /// <param name="to">Target angle.</param>
    /// <param name="t">Interpolation factor in [0,1].</param>
    /// <returns>The interpolated <see cref="Angle"/>.</returns>
    public static Angle Lerp(this Angle from, Angle to, float t)
    {
        int distance = from.ShortestDistanceTo(to);
        int result = from.ToDegrees() + (int)(distance * System.Math.Clamp(t, 0f, 1f));
        return result.ToAngle();
    }

    /// <summary>
    /// Steps an angle toward a target by a maximum number of degrees.
    /// </summary>
    /// <param name="from">Starting angle.</param>
    /// <param name="to">Target angle.</param>
    /// <param name="stepDegrees">Maximum degrees to rotate.</param>
    /// <returns>The stepped <see cref="Angle"/>.</returns>
    public static Angle StepToward(this Angle from, Angle to, int stepDegrees)
    {
        int distance = from.ShortestDistanceTo(to);
        if (System.Math.Abs(distance) <= stepDegrees)
            return to;

        int step = System.Math.Sign(distance) * stepDegrees;
        return (from.ToDegrees() + step).ToAngle();
    }

    /// <summary>
    /// Snaps an arbitrary degree value to the nearest 45-degree increment.
    /// </summary>
    /// <param name="degrees">Degrees to snap.</param>
    /// <returns>The snapped <see cref="Angle"/>.</returns>
    public static Angle SnapToNearest45(int degrees)
        => ((int)System.Math.Round(degrees / 45.0) * 45).ToAngle();

    /// <summary>
    /// Snaps an arbitrary degree value to the nearest 90-degree increment.
    /// </summary>
    /// <param name="degrees">Degrees to snap.</param>
    /// <returns>The snapped <see cref="Angle"/>.</returns>
    public static Angle SnapToNearest90(int degrees)
        => ((int)System.Math.Round(degrees / 90.0) * 90).ToAngle();

    /// <summary>
    /// Returns all eight angles in clockwise order starting at north.
    /// </summary>
    public static Angle[] GetAllAngles()
        =>
        [
            Angle.None,                                           // 0 deg
            Angle.EighthTurn,                                     // 45 deg
            Angle.QuarterTurn,                                    // 90 deg
            Angle.EighthTurn | Angle.QuarterTurn,                 // 135 deg
            Angle.HalfTurn,                                       // 180 deg
            Angle.EighthTurn | Angle.HalfTurn,                    // 225 deg
            Angle.QuarterTurn | Angle.HalfTurn,                   // 270 deg
            Angle.EighthTurn | Angle.QuarterTurn | Angle.HalfTurn // 315 deg
        ];

    /// <summary>
    /// Returns the four cardinal angles.
    /// </summary>
    public static Angle[] GetCardinalAngles()
        =>
        [
            Angle.None,
            Angle.QuarterTurn,
            Angle.HalfTurn,
            Angle.QuarterTurn | Angle.HalfTurn
        ];

    /// <summary>
    /// Returns the four diagonal angles.
    /// </summary>
    public static Angle[] GetDiagonalAngles()
        =>
        [
            Angle.EighthTurn,
            Angle.EighthTurn | Angle.QuarterTurn,
            Angle.EighthTurn | Angle.HalfTurn,
            Angle.EighthTurn | Angle.QuarterTurn | Angle.HalfTurn
        ];

    /// <summary>
    /// Attempts to parse an exact 45-degree multiple into an <see cref="Angle"/>.
    /// </summary>
    /// <param name="degrees">Degrees to convert.</param>
    /// <param name="angle">Result when successful.</param>
    /// <returns><see langword="true"/> if <paramref name="degrees"/> is a multiple of 45.</returns>
    public static bool TryGetExactAngle(int degrees, out Angle angle)
    {
        degrees = ((degrees % 360) + 360) % 360;
        if (degrees % 45 == 0)
        {
            angle = degrees.ToAngle();
            return true;
        }
        angle = Angle.None;
        return false;
    }
}


