using Lib.Math;

namespace Lib.Geometry;

public static class AngleExtensions
{
    public static int ToDegrees(this Angle angle)
    {
        int degrees = 0;
        if (angle.HasFlag(Angle.EighthTurn)) degrees += 45;
        if (angle.HasFlag(Angle.QuarterTurn)) degrees += 90;
        if (angle.HasFlag(Angle.HalfTurn)) degrees += 180;
        return degrees;
    }

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

    public static Angle Add(this Angle angle, Angle other)
        => (angle.ToDegrees() + other.ToDegrees()).ToAngle();

    public static Angle Subtract(this Angle angle, Angle other)
        => (angle.ToDegrees() - other.ToDegrees()).ToAngle();

    public static Angle RotateBy(this Angle angle, Angle rotation)
        => angle.Add(rotation);

    public static Angle RotateClockwise(this Angle angle, Angle amount)
        => angle.Add(amount);

    public static Angle RotateCounterClockwise(this Angle angle, Angle amount)
        => angle.Subtract(amount);

    public static Angle Opposite(this Angle angle)
        => angle.Add(Angle.HalfTurn);

    public static Angle Negate(this Angle angle)
        => (-angle.ToDegrees()).ToAngle();

    public static Angle Normalize(this Angle angle)
        => angle.ToDegrees().ToAngle();

    public static bool IsCardinal(this Angle angle)
        => !angle.HasFlag(Angle.EighthTurn);

    public static bool IsDiagonal(this Angle angle)
        => angle.HasFlag(Angle.EighthTurn);

    public static bool IsHorizontal(this Angle angle)
        => angle == Angle.None || angle == Angle.HalfTurn;

    public static bool IsVertical(this Angle angle)
        => angle == Angle.QuarterTurn || angle == (Angle.QuarterTurn | Angle.HalfTurn);

    public static bool IsPerpendicularTo(this Angle angle, Angle other)
    {
        int diff = System.Math.Abs(angle.ToDegrees() - other.ToDegrees());
        return diff == 90 || diff == 270;
    }

    public static bool IsParallelTo(this Angle angle, Angle other)
    {
        int diff = System.Math.Abs(angle.ToDegrees() - other.ToDegrees());
        return diff == 0 || diff == 180;
    }

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

    public static int HorizontalSign(this Angle angle)
    {
        int degrees = angle.ToDegrees();
        if (degrees == 90 || degrees == 270) return 0;
        return (degrees > 90 && degrees < 270) ? -1 : 1;
    }

    public static int VerticalSign(this Angle angle)
    {
        int degrees = angle.ToDegrees();
        if (degrees == 0 || degrees == 180) return 0;
        return (degrees > 180) ? -1 : 1;
    }

    public static int ShortestDistanceTo(this Angle from, Angle to)
    {
        int diff = to.ToDegrees() - from.ToDegrees();

        // Normalize to -180 to 180 range
        while (diff > 180) diff -= 360;
        while (diff < -180) diff += 360;

        return diff;
    }

    public static int GetRotationDirection(this Angle from, Angle to)
        => System.Math.Sign(from.ShortestDistanceTo(to));

    public static Angle Lerp(this Angle from, Angle to, float t)
    {
        int distance = from.ShortestDistanceTo(to);
        int result = from.ToDegrees() + (int)(distance * System.Math.Clamp(t, 0f, 1f));
        return result.ToAngle();
    }

    public static Angle StepToward(this Angle from, Angle to, int stepDegrees)
    {
        int distance = from.ShortestDistanceTo(to);
        if (System.Math.Abs(distance) <= stepDegrees)
            return to;

        int step = System.Math.Sign(distance) * stepDegrees;
        return (from.ToDegrees() + step).ToAngle();
    }

    public static Angle SnapToNearest45(int degrees)
        => ((int)System.Math.Round(degrees / 45.0) * 45).ToAngle();

    public static Angle SnapToNearest90(int degrees)
        => ((int)System.Math.Round(degrees / 90.0) * 90).ToAngle();

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

    public static Angle[] GetCardinalAngles()
        =>
        [
            Angle.None,
            Angle.QuarterTurn,
            Angle.HalfTurn,
            Angle.QuarterTurn | Angle.HalfTurn
        ];

    public static Angle[] GetDiagonalAngles()
        =>
        [
            Angle.EighthTurn,
            Angle.EighthTurn | Angle.QuarterTurn,
            Angle.EighthTurn | Angle.HalfTurn,
            Angle.EighthTurn | Angle.QuarterTurn | Angle.HalfTurn
        ];

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


