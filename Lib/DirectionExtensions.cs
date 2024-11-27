using System.Numerics;

namespace Lib;

public static class DirectionExtensions
{
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
}