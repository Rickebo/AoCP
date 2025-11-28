using System.Numerics;
using Lib.Coordinate;
using Lib.Enums;

namespace Lib.Extensions;

public static class DirectionExtensions
{
    public static Direction[] Horizontal =>
    [
        Enums.Direction.West,
        Enums.Direction.East
    ];

    public static Direction[] Vertical =>
    [
        Enums.Direction.North,
        Enums.Direction.South
    ];

    public static Direction[] Cardinals =>
    [
        Enums.Direction.North,
        Enums.Direction.East,
        Enums.Direction.South,
        Enums.Direction.West
    ];

    public static Direction[] Diagonals =>
    [
        Enums.Direction.NorthEast,
        Enums.Direction.SouthEast,
        Enums.Direction.SouthWest,
        Enums.Direction.NorthWest
    ];

    public static char ToGlyph(this Direction direction) =>
        direction switch
        {
            Enums.Direction.North => 'N',
            Enums.Direction.East => 'E',
            Enums.Direction.South => 'S',
            Enums.Direction.West => 'W',
            Enums.Direction.NorthWest => 'J',
            Enums.Direction.NorthEast => 'L',
            Enums.Direction.SouthWest => '7',
            Enums.Direction.SouthEast => 'F',
            Enums.Direction.East | Enums.Direction.South | Enums.Direction.West => '\u2533',
            Enums.Direction.East | Enums.Direction.North | Enums.Direction.West => '\u253b',
            Enums.Direction.South | Enums.Direction.North | Enums.Direction.West => '\u252b',
            Enums.Direction.South | Enums.Direction.North | Enums.Direction.East => '\u2523',
            Enums.Direction.NorthEast | Enums.Direction.SouthWest => '+',
            Enums.Direction.North | Enums.Direction.South => '|',
            Enums.Direction.East | Enums.Direction.West => '-',
            Enums.Direction.None => '*',
            _ => throw new ArgumentException(null, nameof(direction))
        };

    public static Direction Max { get; } = (Direction)All().Max(d => (int)d);

    public static Direction Opposite(this Direction direction) => direction switch
    {
        Enums.Direction.North => Enums.Direction.South,
        Enums.Direction.NorthEast => Enums.Direction.SouthWest,
        Enums.Direction.East => Enums.Direction.West,
        Enums.Direction.SouthEast => Enums.Direction.NorthWest,
        Enums.Direction.South => Enums.Direction.North,
        Enums.Direction.SouthWest => Enums.Direction.NorthEast,
        Enums.Direction.West => Enums.Direction.East,
        Enums.Direction.NorthWest => Enums.Direction.SouthEast,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    public static Direction Parse(char character) => character switch
    {
        '-' => Enums.Direction.East | Enums.Direction.West,
        '|' => Enums.Direction.North | Enums.Direction.South,
        '+' => Enums.Direction.North |
               Enums.Direction.East |
               Enums.Direction.South |
               Enums.Direction.West,
        '>' => Enums.Direction.East,
        '<' => Enums.Direction.West,
        '^' => Enums.Direction.North,
        'v' => Enums.Direction.South,
        '7' => Enums.Direction.South | Enums.Direction.West,
        'F' => Enums.Direction.East | Enums.Direction.South,
        'L' => Enums.Direction.North | Enums.Direction.East,
        'J' => Enums.Direction.North | Enums.Direction.West,
        'N' => Enums.Direction.North,
        'E' => Enums.Direction.East,
        'S' => Enums.Direction.South,
        'W' => Enums.Direction.West,
        ' ' => Enums.Direction.None,
        '.' => Enums.Direction.None,
        _ => throw new ArgumentException($"Unknown direction for character {character}")
    };

    public static Direction FlipX(this Direction direction)
    {
        const Direction both = Enums.Direction.East | Enums.Direction.West;
        if ((direction & both) == both)
            return direction;

        var result = direction;
        if ((direction & Enums.Direction.East) != 0)
            result = Enums.Direction.West | (direction & ~Enums.Direction.East);

        if ((direction & Enums.Direction.West) != 0)
            result = Enums.Direction.East | (direction & ~Enums.Direction.West);

        return result;
    }

    public static Direction FlipY(this Direction direction)
    {
        const Direction both = Enums.Direction.North | Enums.Direction.South;
        if ((direction & both) == both)
            return direction;

        var result = direction;

        if ((direction & Enums.Direction.North) != 0)
            result = Enums.Direction.South | (direction & ~Enums.Direction.North);

        if ((direction & Enums.Direction.South) != 0)
            result = Enums.Direction.North | (direction & ~Enums.Direction.South);

        return result;
    }

    public static Direction[] All() => Enum.GetValues<Direction>();

    public static Direction Right(this Direction direction) => direction.RotateClockwise();
    public static Direction Left(this Direction direction) => direction.RotateCounterClockwise();

    public static Direction RotateClockwise(this Direction direction)
    {
        var next = (int)direction << 1;
        return next > (int)Enums.Direction.West ? Enums.Direction.North : (Direction)next;
    }

    public static Direction RotateClockwiseDiagonal(this Direction direction) =>
        direction switch
        {
            Enums.Direction.North => Enums.Direction.NorthEast,
            Enums.Direction.NorthEast => Enums.Direction.East,
            Enums.Direction.East => Enums.Direction.SouthEast,
            Enums.Direction.SouthEast => Enums.Direction.South,
            Enums.Direction.South => Enums.Direction.SouthWest,
            Enums.Direction.SouthWest => Enums.Direction.West,
            Enums.Direction.West => Enums.Direction.NorthWest,
            Enums.Direction.NorthWest => Enums.Direction.North,
            Enums.Direction.None => Enums.Direction.None,
            _ => throw new ArgumentException(
                $"Unknown clockwise diagonal direction for direction {direction}"
            ),
        };

    public static IEnumerable<Direction> Neighbours(this Direction direction) =>
    [
        direction.RotateCounterClockwise(),
        direction,
        direction.RotateClockwise()
    ];

    public static Direction RotateCounterClockwise(this Direction direction)
    {
        var next = (int)direction >> 1;
        return next == 0 ? Enums.Direction.West : (Direction)next;
    }

    public static bool Has(this Direction baseDirection, Direction direction) =>
        (baseDirection & direction) != 0;

    public static Direction Direction<
        TCoordinate, TCoordinateNumber>(
        this ICoordinate<TCoordinate, TCoordinateNumber> coordinate
    ) where TCoordinate : IStringCoordinate
        where TCoordinateNumber : INumber<TCoordinateNumber>
    {
        var result = Enums.Direction.None;

        if (coordinate.X < TCoordinateNumber.Zero)
            result |= Enums.Direction.West;
        else if (coordinate.X > TCoordinateNumber.Zero)
            result |= Enums.Direction.East;

        if (coordinate.Y > TCoordinateNumber.Zero)
            result |= Enums.Direction.North;
        else if (coordinate.Y < TCoordinateNumber.Zero)
            result |= Enums.Direction.South;

        return result;
    }

    public static IntegerCoordinate<T> ToCoordinate<T>(this Direction direction)
        where T : INumber<T>, IBinaryInteger<T>
    {
        var x = (direction.Has(Enums.Direction.East) ? T.One : T.Zero) +
                (direction.Has(Enums.Direction.West) ? -T.One : T.Zero);
        var y = (direction.Has(Enums.Direction.North) ? T.One : T.Zero) +
                (direction.Has(Enums.Direction.South) ? -T.One : T.Zero);

        return new IntegerCoordinate<T>(x, y);
    }

    public static int Ordinal(this Direction direction) =>
        direction switch
        {
            Enums.Direction.North => 0,
            Enums.Direction.East => 1,
            Enums.Direction.South => 2,
            Enums.Direction.West => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    public static char Arrow(this Direction direction) =>
        direction switch
        {
            Enums.Direction.East => '>',
            Enums.Direction.West => '<',
            Enums.Direction.North => '^',
            Enums.Direction.South => 'v',
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    public static Direction FromOrdinal(int ordinal) =>
        ordinal is >= 0 and < 4
            ? (Direction)(1 << ordinal)
            : throw new ArgumentOutOfRangeException(nameof(ordinal));

    public static bool IsVertical(this Direction direction) =>
        IsSubsetOf(direction, Enums.Direction.North | Enums.Direction.South);

    public static bool IsHorizontal(this Direction direction) =>
        IsSubsetOf(direction, Enums.Direction.West | Enums.Direction.East);

    private static bool IsSubsetOf(Direction direction, Direction mask)
    {
        var result = direction & mask;
        return result == direction && (int)result != 0;
    }
}