using System.Numerics;

namespace Lib;

public static class DirectionExtensions
{
    public static Direction[] Cardinals =>
    [
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
    ];

    public static Direction Opposite(this Direction direction) => direction switch
    {
        Direction.North => Direction.South,
        Direction.NorthEast => Direction.SouthWest,
        Direction.East => Direction.West,
        Direction.SouthEast => Direction.NorthWest,
        Direction.South => Direction.North,
        Direction.SouthWest => Direction.NorthEast,
        Direction.West => Direction.East,
        Direction.NorthWest => Direction.SouthEast,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    public static Direction[] All() => Enum.GetValues<Direction>();

    public static Direction RotateClockwise(this Direction direction)
    {
        var next = (int)direction << 1;
        return next > (int)Direction.West ? Direction.North : (Direction)next;
    }

    public static Direction RotateCounterClockwise(this Direction direction)
    {
        var next = (int)direction >> 1;
        return next == 0 ? Direction.West : (Direction)next;
    }

    public static bool Has(this Direction baseDirection, Direction direction) =>
        (baseDirection & direction) != 0;

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