using System.Numerics;

namespace Lib.Geometry;

public static class Neighbourhoods
{
    private static readonly Coordinate3D<int>[] Orthogonal3D =
    [
        new(1, 0, 0), new(-1, 0, 0),
        new(0, 1, 0), new(0, -1, 0),
        new(0, 0, 1), new(0, 0, -1)
    ];

    public static IEnumerable<IntegerCoordinate<T>> Orthogonal<T>(IntegerCoordinate<T> origin)
        where T : INumber<T>, IBinaryInteger<T> =>
        Neighbours(origin, Neighbourhood.Cardinal);

    public static IEnumerable<IntegerCoordinate<T>> All2DNeighbours<T>(IntegerCoordinate<T> origin)
        where T : INumber<T>, IBinaryInteger<T> =>
        Neighbours(origin, Neighbourhood.All);

    public static IEnumerable<Coordinate3D<T>> Orthogonal3DNeighbours<T>(Coordinate3D<T> origin)
        where T : INumber<T>
    {
        foreach (var offset in Orthogonal3D)
            yield return new Coordinate3D<T>(
                origin.X + T.CreateChecked(offset.X),
                origin.Y + T.CreateChecked(offset.Y),
                origin.Z + T.CreateChecked(offset.Z)
            );
    }

    public static IEnumerable<Direction> Directions(Neighbourhood neighbourhood, Direction excluded = Direction.None)
    {
        foreach (var direction in Expand(neighbourhood))
        {
            if ((direction & excluded) != 0)
                continue;

            yield return direction;
        }
    }

    public static IEnumerable<IntegerCoordinate<T>> Neighbours<T>(
        IntegerCoordinate<T> origin,
        Neighbourhood neighbourhood = Neighbourhood.Cardinal,
        Direction excluded = Direction.None)
        where T : INumber<T>, IBinaryInteger<T>
    {
        foreach (var direction in Directions(neighbourhood, excluded))
            yield return origin + direction.ToCoordinate<T>();
    }

    private static IEnumerable<Direction> Expand(Neighbourhood neighbourhood)
    {
        if ((neighbourhood & Neighbourhood.Horizontal) != 0)
        {
            foreach (var direction in DirectionExtensions.Horizontal)
                yield return direction;
        }

        if ((neighbourhood & Neighbourhood.Vertical) != 0)
        {
            foreach (var direction in DirectionExtensions.Vertical)
                yield return direction;
        }

        if ((neighbourhood & Neighbourhood.Diagonal) != 0)
        {
            foreach (var direction in DirectionExtensions.Diagonals)
                yield return direction;
        }
    }
}
