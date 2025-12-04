using System;
using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Extension helpers for working with <see cref="Direction"/> flags.
/// </summary>
public static class DirectionExtensions
{
    /// <summary>
    /// Gets the horizontal directions (west and east).
    /// </summary>
    public static Direction[] Horizontal =>
    [
        Direction.West,
        Direction.East
    ];

    /// <summary>
    /// Gets the vertical directions (north and south).
    /// </summary>
    public static Direction[] Vertical =>
    [
        Direction.North,
        Direction.South
    ];

    /// <summary>
    /// Gets the four cardinal directions.
    /// </summary>
    public static Direction[] Cardinals =>
    [
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
    ];

    /// <summary>
    /// Gets the diagonal directions.
    /// </summary>
    public static Direction[] Diagonals =>
    [
        Direction.NorthEast,
        Direction.SouthEast,
        Direction.SouthWest,
        Direction.NorthWest
    ];

    /// <summary>
    /// Converts a direction to a glyph used by some AoC puzzles.
    /// </summary>
    /// <param name="direction">Direction to convert.</param>
    /// <returns>Character glyph representing the direction.</returns>
    /// <exception cref="ArgumentException">Thrown when the direction is unsupported.</exception>
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
    /// Gets the maximum combined direction value from the <see cref="Direction"/> enum.
    /// </summary>
    public static Direction Max { get; } = (Direction)All().Max(d => (int)d);

    /// <summary>
    /// Returns the opposite of the provided direction.
    /// </summary>
    /// <param name="direction">Direction to invert.</param>
    /// <returns>The opposing direction.</returns>
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
    /// Parses a direction from a character representation commonly used in grid problems.
    /// </summary>
    /// <param name="character">Character to parse.</param>
    /// <returns>The parsed direction.</returns>
    /// <exception cref="ArgumentException">Thrown when the character is not recognized.</exception>
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
    /// Mirrors the direction horizontally.
    /// </summary>
    /// <param name="direction">Direction to mirror.</param>
    /// <returns>Direction mirrored across the Y axis.</returns>
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
    /// Mirrors the direction vertically.
    /// </summary>
    /// <param name="direction">Direction to mirror.</param>
    /// <returns>Direction mirrored across the X axis.</returns>
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
    /// Gets all possible values of the <see cref="Direction"/> enum.
    /// </summary>
    /// <returns>All defined directions.</returns>
    public static Direction[] All() => Enum.GetValues<Direction>();

    /// <summary>
    /// Rotates the direction 90 degrees clockwise.
    /// </summary>
    /// <param name="direction">Direction to rotate.</param>
    /// <returns>Rotated direction.</returns>
    public static Direction Right(this Direction direction) => direction.Rotate(Rotation.Clockwise);

    /// <summary>
    /// Rotates the direction 90 degrees counter-clockwise.
    /// </summary>
    /// <param name="direction">Direction to rotate.</param>
    /// <returns>Rotated direction.</returns>
    public static Direction Left(this Direction direction) => direction.Rotate(Rotation.CounterClockwise);

    /// <summary>
    /// Converts a direction to the corresponding <see cref="Angle"/>.
    /// </summary>
    /// <param name="direction">Direction to convert.</param>
    /// <returns>Angle representing the direction.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the direction is not supported.</exception>
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
    /// Converts an angle to the corresponding <see cref="Direction"/>.
    /// </summary>
    /// <param name="angle">Angle to convert.</param>
    /// <returns>Direction matching the angle.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the angle is not a multiple of 45 degrees.</exception>
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
    /// Rotates a direction by the specified rotation and angle step.
    /// </summary>
    /// <param name="direction">Direction to rotate.</param>
    /// <param name="rotation">Rotation direction.</param>
    /// <param name="by">Angle step to rotate by.</param>
    /// <returns>Rotated direction.</returns>
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
    /// Returns the neighbour directions around the given direction.
    /// </summary>
    /// <param name="direction">Base direction.</param>
    /// <param name="angle">Offset angle between neighbours.</param>
    /// <returns>Neighbouring directions in counter-clockwise, self, clockwise order.</returns>
    public static IEnumerable<Direction> Neighbours(this Direction direction, Angle angle = Angle.QuarterTurn) =>
    [
        direction.Rotate(Rotation.CounterClockwise, angle),
        direction,
        direction.Rotate(Rotation.Clockwise, angle)
    ];

    /// <summary>
    /// Checks whether a direction flag contains another direction.
    /// </summary>
    /// <param name="baseDirection">Direction flag to test.</param>
    /// <param name="direction">Direction to look for.</param>
    /// <returns><c>true</c> when <paramref name="baseDirection"/> includes <paramref name="direction"/>.</returns>
    public static bool Has(this Direction baseDirection, Direction direction) =>
        (baseDirection & direction) != 0;

    /// <summary>
    /// Determines the direction of a coordinate relative to the origin.
    /// </summary>
    /// <typeparam name="TCoordinate">Coordinate type.</typeparam>
    /// <typeparam name="TCoordinateNumber">Numeric component type.</typeparam>
    /// <param name="coordinate">Coordinate to evaluate.</param>
    /// <returns>Direction pointing from the origin toward the coordinate.</returns>
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
    /// Converts a direction into a unit coordinate of the specified numeric type.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="direction">Direction to convert.</param>
    /// <returns>Integer coordinate representing the direction.</returns>
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
    /// Gets the ordinal position of a cardinal direction (N=0, E=1, S=2, W=3).
    /// </summary>
    /// <param name="direction">Direction to convert.</param>
    /// <returns>Ordinal index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for non-cardinal directions.</exception>
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
    /// Converts a direction to an arrow character.
    /// </summary>
    /// <param name="direction">Direction to convert.</param>
    /// <returns>Arrow glyph.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for non-cardinal directions.</exception>
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
    /// Converts an ordinal index to its corresponding cardinal direction.
    /// </summary>
    /// <param name="ordinal">Ordinal in the range [0,3].</param>
    /// <returns>Cardinal direction.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="ordinal"/> is outside the valid range.</exception>
    public static Direction FromOrdinal(int ordinal) =>
        ordinal is >= 0 and < 4
            ? (Direction)(1 << ordinal)
            : throw new ArgumentOutOfRangeException(nameof(ordinal));

    /// <summary>
    /// Determines whether a direction is fully vertical (north or south).
    /// </summary>
    /// <param name="direction">Direction to test.</param>
    /// <returns><c>true</c> when the direction only contains north/south flags.</returns>
    public static bool IsVertical(this Direction direction) =>
        IsSubsetOf(direction, Direction.North | Direction.South);

    /// <summary>
    /// Determines whether a direction is fully horizontal (east or west).
    /// </summary>
    /// <param name="direction">Direction to test.</param>
    /// <returns><c>true</c> when the direction only contains east/west flags.</returns>
    public static bool IsHorizontal(this Direction direction) =>
        IsSubsetOf(direction, Direction.West | Direction.East);

    /// <summary>
    /// Checks whether the direction is a subset of the provided mask.
    /// </summary>
    /// <param name="direction">Direction to check.</param>
    /// <param name="mask">Mask to compare against.</param>
    /// <returns><c>true</c> when the direction only includes flags within the mask.</returns>
    private static bool IsSubsetOf(Direction direction, Direction mask)
    {
        var result = direction & mask;
        return result == direction && (int)result != 0;
    }
}

