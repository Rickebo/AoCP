using System.Numerics;

namespace Lib.Coordinate;

public readonly struct Distance<T>(T x, T y) where T : INumber<T>
{
    public T X { get; } = x;
    public T Y { get; } = y;

    public T Manhattan() => T.Abs(X) + T.Abs(Y);
}
