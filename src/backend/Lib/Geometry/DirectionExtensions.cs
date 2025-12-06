using System;
using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Extension helpers for working with <see cref="Direction"/> flags.
/// </summary>
public static class DirectionExtensions
{
    /// <summary>
    /// Directions representing horizontal movement (west, east).
    /// </summary>
    public static Direction[] Horizontal =>
    [
        Direction.West,
        Direction.East
    ];

    /// <summary>
    /// Directions representing vertical movement (north, south).
    /// </summary>
    public static Direction[] Vertical =>
    [
        Direction.North,
        Direction.South
    ];

    /// <summary>
    /// The four cardinal directions.
    /// </summary>
    public static Direction[] Cardinals =>
    [
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
    ];

    /// <summary>
    /// The four diagonal directions.
    /// </summary>
    public static Direction[] Diagonals =>
    [
        Direction.NorthEast,
        Direction.SouthEast,
        Direction.SouthWest,
        Direction.NorthWest
    ];

    /// <summary>
    /// Converts a direction to an ASCII/box-drawing glyph useful for visualization.
    /// </summary>
    /// <param name="direction">Direction to convert.</param>
    /// <returns>The representative glyph.</returns>
    public static char ToGlyph(this Direction direction) =>
        direction switch
        {
            Direction.North => 'N',
            Direction.East => 'E',
            Direction.South => 'S',
            Direction.West => 'W',
            Direction.NorthWest => 'J',
            Direction.NorthEast => 'L',
            Direction.SouthWest => '7',
            Direction.SouthEast => 'F',
            Direction.East | Direction.South | Direction.West => '\u2533',
            Direction.East | Direction.North | Direction.West => '\u253b',
            Direction.South | Direction.North | Direction.West => '\u252b',
            Direction.South | Direction.North | Direction.East => '\u2523',
            Direction.NorthEast | Direction.SouthWest => '+',
            Direction.North | Direction.South => '|',
            Direction.East | Direction.West => '-',
            Direction.None => '*',
            _ => throw new ArgumentException(null, nameof(direction))
        };

    /// <summary>
    /// Maximum direction value (used to bound enumerations).
    /// </summary>
    public static Direction Max { get; } = (Direction)All().Max(d => (int)d);

    /// <summary>
    /// Returns the opposite direction (e.g., north -> south).
    /// </summary>
    public static Direction Opposite(this Direction direction)
    {
        if (direction == Direction.None)
            return Direction.None;

        var result = Direction.None;

        if (direction.Has(Direction.North))
            result |= Direction.South;

        if (direction.Has(Direction.South))
            result |= Direction.North;

        if (direction.Has(Direction.East))
            result |= Direction.West;

        if (direction.Has(Direction.West))
            result |= Direction.East;

        return result;
    }

    /// <summary>
    /// Parses a direction from a character glyph.
    /// </summary>
    /// <param name="character">Glyph to parse.</param>
    /// <returns>The parsed <see cref="Direction"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the glyph is unknown.</exception>
    public static Direction Parse(char character) => character switch
    {
        '-' => Direction.East | Direction.West,
        '|' => Direction.North | Direction.South,
        '+' => Direction.North | Direction.East | Direction.South | Direction.West,
        '>' => Direction.East,
        '<' => Direction.West,
        '^' => Direction.North,
        'v' => Direction.South,
        '7' => Direction.South | Direction.West,
        'F' => Direction.East | Direction.South,
        'L' => Direction.North | Direction.East,
        'J' => Direction.North | Direction.West,
        'N' => Direction.North,
        'E' => Direction.East,
        'S' => Direction.South,
        'W' => Direction.West,
        ' ' => Direction.None,
        '.' => Direction.None,
        _ => throw new ArgumentException($"Unknown direction for character {character}")
    };

    /// <summary>
    /// Mirrors a direction over the X axis (swapping east and west when not both set).
    /// </summary>
    public static Direction FlipX(this Direction direction)
    {
        const Direction both = Direction.East | Direction.West;
        if ((direction & both) == both)
            return direction;

        var result = direction;
        if ((direction & Direction.East) != 0)
            result = Direction.West | (direction & ~Direction.East);

        if ((direction & Direction.West) != 0)
            result = Direction.East | (direction & ~Direction.West);

        return result;
    }

    /// <summary>
    /// Mirrors a direction over the Y axis (swapping north and south when not both set).
    /// </summary>
    public static Direction FlipY(this Direction direction)
    {
        const Direction both = Direction.North | Direction.South;
        if ((direction & both) == both)
            return direction;

        var result = direction;

        if ((direction & Direction.North) != 0)
            result = Direction.South | (direction & ~Direction.North);

        if ((direction & Direction.South) != 0)
            result = Direction.North | (direction & ~Direction.South);

        return result;
    }

    /// <summary>
    /// Returns all possible direction flag combinations.
    /// </summary>
    public static Direction[] All() => Enum.GetValues<Direction>();

    /// <summary>
    /// Rotates 90 degrees clockwise.
    /// </summary>
    public static Direction Right(this Direction direction) => direction.Rotate(Rotation.Clockwise);

    /// <summary>
    /// Rotates 90 degrees counter-clockwise.
    /// </summary>
    public static Direction Left(this Direction direction) => direction.Rotate(Rotation.CounterClockwise);

    /// <summary>
    /// Converts a direction to its equivalent <see cref="Angle"/>.
    /// </summary>
    /// <param name="direction">Direction to convert.</param>
    /// <returns>The corresponding <see cref="Angle"/>.</returns>
    public static Angle ToAngle(this Direction direction) => direction switch
    {
        Direction.North or Direction.Up => Angle.None,
        Direction.NorthEast or Direction.UpRight => Angle.EighthTurn,
        Direction.East or Direction.Right => Angle.QuarterTurn,
        Direction.SouthEast or Direction.DownRight => Angle.QuarterTurn | Angle.EighthTurn,
        Direction.South or Direction.Down => Angle.HalfTurn,
        Direction.SouthWest or Direction.DownLeft => Angle.HalfTurn | Angle.EighthTurn,
        Direction.West or Direction.Left => Angle.HalfTurn | Angle.QuarterTurn,
        Direction.NorthWest or Direction.UpLeft => Angle.HalfTurn | Angle.QuarterTurn | Angle.EighthTurn,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction,
            "Unsupported direction for angle conversion.")
    };

    /// <summary>
    /// Converts a discrete angle (multiple of 45) to a <see cref="Direction"/>.
    /// </summary>
    /// <param name="angle">Angle to convert.</param>
    /// <returns>The matching <see cref="Direction"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the angle is not a supported multiple.</exception>
    public static Direction ToDirection(this Angle angle)
    {
        var degrees = angle.ToDegrees();

        return degrees switch
        {
            0 => Direction.North,
            45 => Direction.NorthEast,
            90 => Direction.East,
            135 => Direction.SouthEast,
            180 => Direction.South,
            225 => Direction.SouthWest,
            270 => Direction.West,
            315 => Direction.NorthWest,
            _ => throw new ArgumentOutOfRangeException(nameof(angle), angle,
                "Angle must be a multiple of 45 degrees (0-315).")
        };
    }

    /// <summary>
    /// Rotates a direction by the specified rotation and amount.
    /// </summary>
    /// <param name="direction">Original direction.</param>
    /// <param name="rotation">Rotation direction.</param>
    /// <param name="by">Angle to rotate by.</param>
    /// <returns>The rotated <see cref="Direction"/>.</returns>
    public static Direction Rotate(this Direction direction, Rotation rotation, Angle by = Angle.QuarterTurn)
    {
        if (direction == Direction.None)
            return Direction.None;

        if (rotation == Rotation.None)
            return direction;

        return rotation == Rotation.Clockwise
            ? direction.ToAngle().Add(by).ToDirection()
            : direction.ToAngle().Subtract(by).ToDirection();
    }

    /// <summary>
    /// Returns the neighbours around a direction for a given arc (default 90 degrees on each side).
    /// </summary>
    /// <param name="direction">The base direction.</param>
    /// <param name="angle">Total arc to include on each side.</param>
    /// <returns>A sequence of neighbouring directions.</returns>
    public static IEnumerable<Direction> Neighbours(this Direction direction, Angle angle = Angle.QuarterTurn) =>
    [
        direction.Rotate(Rotation.CounterClockwise, angle),
        direction,
        direction.Rotate(Rotation.Clockwise, angle)
    ];

    /// <summary>
    /// Checks if a base direction contains the provided flag.
    /// </summary>
    public static bool Has(this Direction baseDirection, Direction direction) =>
        (baseDirection & direction) != 0;

    /// <summary>
    /// Computes a direction from the origin to the coordinate by looking at the sign of each component.
    /// </summary>
    public static Direction DirectionFromOrigin<TCoordinate, TCoordinateNumber>(
        this ICoordinate<TCoordinate, TCoordinateNumber> coordinate
    ) where TCoordinate : ICoordinate<TCoordinate, TCoordinateNumber>
        where TCoordinateNumber : INumber<TCoordinateNumber>
    {
        var result = Direction.None;

        if (coordinate.X < TCoordinateNumber.Zero)
            result |= Direction.West;
        else if (coordinate.X > TCoordinateNumber.Zero)
            result |= Direction.East;

        if (coordinate.Y > TCoordinateNumber.Zero)
            result |= Direction.North;
        else if (coordinate.Y < TCoordinateNumber.Zero)
            result |= Direction.South;

        return result;
    }

    /// <summary>
    /// Converts a direction to a unit integer coordinate.
    /// </summary>
    public static IntegerCoordinate<T> ToCoordinate<T>(this Direction direction)
        where T : INumber<T>, IBinaryInteger<T>
    {
        var x = (direction.Has(Direction.East) ? T.One : T.Zero) +
                (direction.Has(Direction.West) ? -T.One : T.Zero);
        var y = (direction.Has(Direction.North) ? T.One : T.Zero) +
                (direction.Has(Direction.South) ? -T.One : T.Zero);

        return new IntegerCoordinate<T>(x, y);
    }

    /// <summary>
    /// Returns an ordinal for the four cardinal directions (N=0, E=1, S=2, W=3).
    /// </summary>
    public static int Ordinal(this Direction direction) =>
        direction switch
        {
            Direction.North => 0,
            Direction.East => 1,
            Direction.South => 2,
            Direction.West => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    /// <summary>
    /// Returns a simple arrow glyph representing the direction.
    /// </summary>
    public static char Arrow(this Direction direction) =>
        direction switch
        {
            Direction.East => '>',
            Direction.West => '<',
            Direction.North => '^',
            Direction.South => 'v',
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    /// <summary>
    /// Converts an ordinal in the range 0-3 to its corresponding cardinal direction.
    /// </summary>
    /// <param name="ordinal">Ordinal value (0-3).</param>
    /// <returns>The corresponding <see cref="Direction"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when ordinal is outside 0-3.</exception>
    public static Direction FromOrdinal(int ordinal) =>
        ordinal is >= 0 and < 4
            ? (Direction)(1 << ordinal)
            : throw new ArgumentOutOfRangeException(nameof(ordinal));

    /// <summary>
    /// Determines if the direction is purely vertical.
    /// </summary>
    public static bool IsVertical(this Direction direction) =>
        IsSubsetOf(direction, Direction.North | Direction.South);

    /// <summary>
    /// Determines if the direction is purely horizontal.
    /// </summary>
    public static bool IsHorizontal(this Direction direction) =>
        IsSubsetOf(direction, Direction.West | Direction.East);

    private static bool IsSubsetOf(Direction direction, Direction mask)
    {
        var result = direction & mask;
        return result == direction && (int)result != 0;
    }
}

