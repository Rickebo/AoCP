using System.Numerics;
using Lib.Coordinate;

namespace Lib;

public static class DirectionExtensions
{
    public static Direction[] Horizontal =>
    [
        Lib.Direction.West,
        Lib.Direction.East
    ];

    public static Direction[] Vertical =>
    [
        Lib.Direction.North,
        Lib.Direction.South
    ];

    public static Direction[] Cardinals =>
    [
        Lib.Direction.North,
        Lib.Direction.East,
        Lib.Direction.South,
        Lib.Direction.West
    ];

    public static Direction[] Diagonals =>
    [
        Lib.Direction.NorthEast,
        Lib.Direction.SouthEast,
        Lib.Direction.SouthWest,
        Lib.Direction.NorthWest
    ];

    public static char ToGlyph(this Direction direction) =>
        direction switch
        {
            Lib.Direction.North => 'N',
            Lib.Direction.East => 'E',
            Lib.Direction.South => 'S',
            Lib.Direction.West => 'W',
            Lib.Direction.NorthWest => 'J',
            Lib.Direction.NorthEast => 'L',
            Lib.Direction.SouthWest => '7',
            Lib.Direction.SouthEast => 'F',
            Lib.Direction.East | Lib.Direction.South | Lib.Direction.West => '\u2533',
            Lib.Direction.East | Lib.Direction.North | Lib.Direction.West => '\u253b',
            Lib.Direction.South | Lib.Direction.North | Lib.Direction.West => '\u252b',
            Lib.Direction.South | Lib.Direction.North | Lib.Direction.East => '\u2523',
            Lib.Direction.NorthEast | Lib.Direction.SouthWest => '+',
            Lib.Direction.North | Lib.Direction.South => '|',
            Lib.Direction.East | Lib.Direction.West => '-',
            Lib.Direction.None => '*',
            _ => throw new ArgumentException(null, nameof(direction))
        };

    public static Direction Max { get; } = (Direction)All().Max(d => (int)d);

    public static Direction Opposite(this Direction direction) => direction switch
    {
        Lib.Direction.North => Lib.Direction.South,
        Lib.Direction.NorthEast => Lib.Direction.SouthWest,
        Lib.Direction.East => Lib.Direction.West,
        Lib.Direction.SouthEast => Lib.Direction.NorthWest,
        Lib.Direction.South => Lib.Direction.North,
        Lib.Direction.SouthWest => Lib.Direction.NorthEast,
        Lib.Direction.West => Lib.Direction.East,
        Lib.Direction.NorthWest => Lib.Direction.SouthEast,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    public static Direction Parse(char character) => character switch
    {
        '-' => Lib.Direction.East | Lib.Direction.West,
        '|' => Lib.Direction.North | Lib.Direction.South,
        '+' => Lib.Direction.North |
               Lib.Direction.East |
               Lib.Direction.South |
               Lib.Direction.West,
        '>' => Lib.Direction.East,
        '<' => Lib.Direction.West,
        '^' => Lib.Direction.North,
        'v' => Lib.Direction.South,
        '7' => Lib.Direction.South | Lib.Direction.West,
        'F' => Lib.Direction.East | Lib.Direction.South,
        'L' => Lib.Direction.North | Lib.Direction.East,
        'J' => Lib.Direction.North | Lib.Direction.West,
        'N' => Lib.Direction.North,
        'E' => Lib.Direction.East,
        'S' => Lib.Direction.South,
        'W' => Lib.Direction.West,
        ' ' => Lib.Direction.None,
        '.' => Lib.Direction.None,
        _ => throw new ArgumentException($"Unknown direction for character {character}")
    };

    public static Direction FlipX(this Direction direction)
    {
        const Direction both = Lib.Direction.East | Lib.Direction.West;
        if ((direction & both) == both)
            return direction;

        var result = direction;
        if ((direction & Lib.Direction.East) != 0)
            result = Lib.Direction.West | (direction & ~Lib.Direction.East);

        if ((direction & Lib.Direction.West) != 0)
            result = Lib.Direction.East | (direction & ~Lib.Direction.West);

        return result;
    }

    public static Direction FlipY(this Direction direction)
    {
        const Direction both = Lib.Direction.North | Lib.Direction.South;
        if ((direction & both) == both)
            return direction;

        var result = direction;

        if ((direction & Lib.Direction.North) != 0)
            result = Lib.Direction.South | (direction & ~Lib.Direction.North);

        if ((direction & Lib.Direction.South) != 0)
            result = Lib.Direction.North | (direction & ~Lib.Direction.South);

        return result;
    }

    public static Direction[] All() => Enum.GetValues<Direction>();

    public static Direction Right(this Direction direction) => direction.RotateClockwise();
    public static Direction Left(this Direction direction) => direction.RotateCounterClockwise();

    public static Direction RotateClockwise(this Direction direction)
    {
        var next = (int)direction << 1;
        return next > (int)Lib.Direction.West ? Lib.Direction.North : (Direction)next;
    }

    public static Direction RotateClockwiseDiagonal(this Direction direction) =>
        direction switch
        {
            Lib.Direction.North => Lib.Direction.NorthEast,
            Lib.Direction.NorthEast => Lib.Direction.East,
            Lib.Direction.East => Lib.Direction.SouthEast,
            Lib.Direction.SouthEast => Lib.Direction.South,
            Lib.Direction.South => Lib.Direction.SouthWest,
            Lib.Direction.SouthWest => Lib.Direction.West,
            Lib.Direction.West => Lib.Direction.NorthWest,
            Lib.Direction.NorthWest => Lib.Direction.North,
            Lib.Direction.None => Lib.Direction.None,
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
        return next == 0 ? Lib.Direction.West : (Direction)next;
    }

    public static bool Has(this Direction baseDirection, Direction direction) =>
        (baseDirection & direction) != 0;

    public static Direction Direction<
        TCoordinate, TCoordinateNumber>(
        this ICoordinate<TCoordinate, TCoordinateNumber> coordinate
    ) where TCoordinate : IStringCoordinate
        where TCoordinateNumber : INumber<TCoordinateNumber>
    {
        var result = Lib.Direction.None;

        if (coordinate.X < TCoordinateNumber.Zero)
            result |= Lib.Direction.West;
        else if (coordinate.X > TCoordinateNumber.Zero)
            result |= Lib.Direction.East;

        if (coordinate.Y > TCoordinateNumber.Zero)
            result |= Lib.Direction.North;
        else if (coordinate.Y < TCoordinateNumber.Zero)
            result |= Lib.Direction.South;

        return result;
    }

    public static IntegerCoordinate<T> ToCoordinate<T>(this Direction direction)
        where T : INumber<T>, IBinaryInteger<T>
    {
        var x = (direction.Has(Lib.Direction.East) ? T.One : T.Zero) +
                (direction.Has(Lib.Direction.West) ? -T.One : T.Zero);
        var y = (direction.Has(Lib.Direction.North) ? T.One : T.Zero) +
                (direction.Has(Lib.Direction.South) ? -T.One : T.Zero);

        return new IntegerCoordinate<T>(x, y);
    }

    public static int Ordinal(this Direction direction) =>
        direction switch
        {
            Lib.Direction.North => 0,
            Lib.Direction.East => 1,
            Lib.Direction.South => 2,
            Lib.Direction.West => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    public static char Arrow(this Direction direction) =>
        direction switch
        {
            Lib.Direction.East => '>',
            Lib.Direction.West => '<',
            Lib.Direction.North => '^',
            Lib.Direction.South => 'v',
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    public static Direction FromOrdinal(int ordinal) =>
        ordinal is >= 0 and < 4
            ? (Direction)(1 << ordinal)
            : throw new ArgumentOutOfRangeException(nameof(ordinal));

    public static bool IsVertical(this Direction direction) =>
        IsSubsetOf(direction, Lib.Direction.North | Lib.Direction.South);

    public static bool IsHorizontal(this Direction direction) =>
        IsSubsetOf(direction, Lib.Direction.West | Lib.Direction.East);

    private static bool IsSubsetOf(Direction direction, Direction mask)
    {
        var result = direction & mask;
        return result == direction && (int)result != 0;
    }
}