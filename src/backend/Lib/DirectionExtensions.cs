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
        '+' => Lib.Direction.North | Lib.Direction.East | Lib.Direction.South |
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

    public static Direction[] All() => Enum.GetValues<Direction>();

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