using Lib.Geometry;
using System.Numerics;

namespace Lib.Geometry;

public static class Neighbourhoods
{
    private static readonly IntegerCoordinate<int>[] Orthogonal2D =
    [
        new(1, 0),
        new(-1, 0),
        new(0, 1),
        new(0, -1)
    ];

    private static readonly IntegerCoordinate<int>[] All2D =
    [
        new(-1, -1), new(0, -1), new(1, -1),
        new(-1, 0), /* origin */ new(1, 0),
        new(-1, 1), new(0, 1), new(1, 1)
    ];

    private static readonly Coordinate3D<int>[] Orthogonal3D =
    [
        new(1, 0, 0), new(-1, 0, 0),
        new(0, 1, 0), new(0, -1, 0),
        new(0, 0, 1), new(0, 0, -1)
    ];

    /// <summary>
    /// Returns orthogonal neighbours (N/E/S/W) for a given coordinate.
    /// </summary>
    public static IEnumerable<IntegerCoordinate<T>> Orthogonal<T>(IntegerCoordinate<T> origin)
        where T : INumber<T>, IBinaryInteger<T>
    {
        foreach (var offset in Orthogonal2D)
            yield return new IntegerCoordinate<T>(
                origin.X + T.CreateChecked(offset.X),
                origin.Y + T.CreateChecked(offset.Y)
            );
    }

    /// <summary>
    /// Returns 8 surrounding neighbours in 2D (including diagonals).
    /// </summary>
    public static IEnumerable<IntegerCoordinate<T>> All2DNeighbours<T>(IntegerCoordinate<T> origin)
        where T : INumber<T>, IBinaryInteger<T>
    {
        foreach (var offset in All2D)
            yield return new IntegerCoordinate<T>(
                origin.X + T.CreateChecked(offset.X),
                origin.Y + T.CreateChecked(offset.Y)
            );
    }

    /// <summary>
    /// Returns 6 orthogonal neighbours in 3D.
    /// </summary>
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
}

