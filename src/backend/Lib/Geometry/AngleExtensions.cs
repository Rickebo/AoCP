using Lib.Enums;

namespace Lib.Extensions;

public static class AngleExtensions
{
    /// <summary>Converts the angle to degrees (0-315).</summary>
    public static int ToDegrees(this Angle angle)
    {
        int degrees = 0;
        if (angle.HasFlag(Angle.EighthTurn)) degrees += 45;
        if (angle.HasFlag(Angle.QuarterTurn)) degrees += 90;
        if (angle.HasFlag(Angle.HalfTurn)) degrees += 180;
        return degrees;
    }

    /// <summary>Converts degrees to an Angle enum value.</summary>
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

    /// <summary>Adds two angles together, normalizing the result to 0-315 degrees.</summary>
    public static Angle Add(this Angle angle, Angle other)
        => (angle.ToDegrees() + other.ToDegrees()).ToAngle();

    /// <summary>Subtracts an angle, normalizing the result to 0-315 degrees.</summary>
    public static Angle Subtract(this Angle angle, Angle other)
        => (angle.ToDegrees() - other.ToDegrees()).ToAngle();

    /// <summary>Rotates the angle by a specified amount.</summary>
    public static Angle RotateBy(this Angle angle, Angle rotation)
        => angle.Add(rotation);

    /// <summary>Rotates the angle clockwise by the specified amount.</summary>
    public static Angle RotateClockwise(this Angle angle, Angle amount)
        => angle.Add(amount);

    /// <summary>Rotates the angle counter-clockwise by the specified amount.</summary>
    public static Angle RotateCounterClockwise(this Angle angle, Angle amount)
        => angle.Subtract(amount);

    /// <summary>Gets the opposite angle (rotated by 180 degrees).</summary>
    public static Angle Opposite(this Angle angle)
        => angle.Add(Angle.HalfTurn);

    /// <summary>Negates the angle (360 - angle).</summary>
    public static Angle Negate(this Angle angle)
        => (-angle.ToDegrees()).ToAngle();

    /// <summary>Normalizes the angle to a valid 0-315 degree range.</summary>
    public static Angle Normalize(this Angle angle)
        => angle.ToDegrees().ToAngle();

    /// <summary>Returns true if the angle is cardinal (0, 90, 180, or 270 degrees).</summary>
    public static bool IsCardinal(this Angle angle)
        => !angle.HasFlag(Angle.EighthTurn);

    /// <summary>Returns true if the angle is diagonal (45, 135, 225, or 315 degrees).</summary>
    public static bool IsDiagonal(this Angle angle)
        => angle.HasFlag(Angle.EighthTurn);

    /// <summary>Returns true if the angle is axis-aligned (0 or 180 degrees).</summary>
    public static bool IsHorizontal(this Angle angle)
        => angle == Angle.None || angle == Angle.HalfTurn;

    /// <summary>Returns true if the angle is axis-aligned (90 or 270 degrees).</summary>
    public static bool IsVertical(this Angle angle)
        => angle == Angle.QuarterTurn || angle == (Angle.QuarterTurn | Angle.HalfTurn);

    /// <summary>Returns true if two angles are perpendicular (90 degrees apart).</summary>
    public static bool IsPerpendicularTo(this Angle angle, Angle other)
    {
        int diff = Math.Abs(angle.ToDegrees() - other.ToDegrees());
        return diff == 90 || diff == 270;
    }

    /// <summary>Returns true if two angles are parallel (0 or 180 degrees apart).</summary>
    public static bool IsParallelTo(this Angle angle, Angle other)
    {
        int diff = Math.Abs(angle.ToDegrees() - other.ToDegrees());
        return diff == 0 || diff == 180;
    }

    /// <summary>Gets the quadrant (1-4) of the angle. Returns 0 for axis-aligned angles.</summary>
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

    /// <summary>Returns the sign of the horizontal component (-1, 0, or 1).</summary>
    public static int HorizontalSign(this Angle angle)
    {
        int degrees = angle.ToDegrees();
        if (degrees == 90 || degrees == 270) return 0;
        return (degrees > 90 && degrees < 270) ? -1 : 1;
    }

    /// <summary>Returns the sign of the vertical component (-1, 0, or 1).</summary>
    public static int VerticalSign(this Angle angle)
    {
        int degrees = angle.ToDegrees();
        if (degrees == 0 || degrees == 180) return 0;
        return (degrees > 180) ? -1 : 1;
    }

    /// <summary>Gets the shortest angular distance to another angle.</summary>
    public static int ShortestDistanceTo(this Angle from, Angle to)
    {
        int diff = to.ToDegrees() - from.ToDegrees();

        // Normalize to -180 to 180 range
        while (diff > 180) diff -= 360;
        while (diff < -180) diff += 360;

        return diff;
    }

    /// <summary>Gets the shortest rotation direction to another angle (-1 = CCW, 0 = same, 1 = CW).</summary>
    public static int GetRotationDirection(this Angle from, Angle to)
        => Math.Sign(from.ShortestDistanceTo(to));

    /// <summary>Linearly interpolates between two angles using the shortest path.</summary>
    public static Angle Lerp(this Angle from, Angle to, float t)
    {
        int distance = from.ShortestDistanceTo(to);
        int result = from.ToDegrees() + (int)(distance * Math.Clamp(t, 0f, 1f));
        return result.ToAngle();
    }

    /// <summary>Steps toward the target angle by the specified amount.</summary>
    public static Angle StepToward(this Angle from, Angle to, int stepDegrees)
    {
        int distance = from.ShortestDistanceTo(to);
        if (Math.Abs(distance) <= stepDegrees)
            return to;

        int step = Math.Sign(distance) * stepDegrees;
        return (from.ToDegrees() + step).ToAngle();
    }

    /// <summary>Snaps degrees to the nearest 45-degree increment.</summary>
    public static Angle SnapToNearest45(int degrees)
        => ((int)Math.Round(degrees / 45.0) * 45).ToAngle();

    /// <summary>Snaps degrees to the nearest 90-degree increment.</summary>
    public static Angle SnapToNearest90(int degrees)
        => ((int)Math.Round(degrees / 90.0) * 90).ToAngle();

    /// <summary>Returns all valid angle values.</summary>
    public static Angle[] GetAllAngles()
        =>
        [
            Angle.None,                                           // 0°
            Angle.EighthTurn,                                     // 45°
            Angle.QuarterTurn,                                    // 90°
            Angle.EighthTurn | Angle.QuarterTurn,                 // 135°
            Angle.HalfTurn,                                       // 180°
            Angle.EighthTurn | Angle.HalfTurn,                    // 225°
            Angle.QuarterTurn | Angle.HalfTurn,                   // 270°
            Angle.EighthTurn | Angle.QuarterTurn | Angle.HalfTurn // 315°
        ];

    /// <summary>Returns all cardinal angles (0, 90, 180, 270).</summary>
    public static Angle[] GetCardinalAngles()
        =>
        [
            Angle.None,
            Angle.QuarterTurn,
            Angle.HalfTurn,
            Angle.QuarterTurn | Angle.HalfTurn
        ];

    /// <summary>Returns all diagonal angles (45, 135, 225, 315).</summary>
    public static Angle[] GetDiagonalAngles()
        =>
        [
            Angle.EighthTurn,
            Angle.EighthTurn | Angle.QuarterTurn,
            Angle.EighthTurn | Angle.HalfTurn,
            Angle.EighthTurn | Angle.QuarterTurn | Angle.HalfTurn
        ];

    /// <summary>Clamps an arbitrary degree value to the nearest valid angle increment.</summary>
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
