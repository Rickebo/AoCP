using System;
using System.Numerics;

namespace Lib.Geometry;

public static class DirectionExtensions
{
    public static Direction[] Horizontal =>
    [
        Direction.West,
        Direction.East
    ];

    public static Direction[] Vertical =>
    [
        Direction.North,
        Direction.South
    ];

    public static Direction[] Cardinals =>
    [
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
    ];

    public static Direction[] Diagonals =>
    [
        Direction.NorthEast,
        Direction.SouthEast,
        Direction.SouthWest,
        Direction.NorthWest
    ];

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

    public static Direction Max { get; } = (Direction)All().Max(d => (int)d);

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

    public static Direction[] All() => Enum.GetValues<Direction>();

    public static Direction Right(this Direction direction) => direction.Rotate(Rotation.Clockwise);

    public static Direction Left(this Direction direction) => direction.Rotate(Rotation.CounterClockwise);

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

    public static IEnumerable<Direction> Neighbours(this Direction direction, Angle angle = Angle.QuarterTurn) =>
    [
        direction.Rotate(Rotation.CounterClockwise, angle),
        direction,
        direction.Rotate(Rotation.Clockwise, angle)
    ];

    public static bool Has(this Direction baseDirection, Direction direction) =>
        (baseDirection & direction) != 0;

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

    public static IntegerCoordinate<T> ToCoordinate<T>(this Direction direction)
        where T : INumber<T>, IBinaryInteger<T>
    {
        var x = (direction.Has(Direction.East) ? T.One : T.Zero) +
                (direction.Has(Direction.West) ? -T.One : T.Zero);
        var y = (direction.Has(Direction.North) ? T.One : T.Zero) +
                (direction.Has(Direction.South) ? -T.One : T.Zero);

        return new IntegerCoordinate<T>(x, y);
    }

    public static int Ordinal(this Direction direction) =>
        direction switch
        {
            Direction.North => 0,
            Direction.East => 1,
            Direction.South => 2,
            Direction.West => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    public static char Arrow(this Direction direction) =>
        direction switch
        {
            Direction.East => '>',
            Direction.West => '<',
            Direction.North => '^',
            Direction.South => 'v',
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    public static Direction FromOrdinal(int ordinal) =>
        ordinal is >= 0 and < 4
            ? (Direction)(1 << ordinal)
            : throw new ArgumentOutOfRangeException(nameof(ordinal));

    public static bool IsVertical(this Direction direction) =>
        IsSubsetOf(direction, Direction.North | Direction.South);

    public static bool IsHorizontal(this Direction direction) =>
        IsSubsetOf(direction, Direction.West | Direction.East);

    private static bool IsSubsetOf(Direction direction, Direction mask)
    {
        var result = direction & mask;
        return result == direction && (int)result != 0;
    }
}

