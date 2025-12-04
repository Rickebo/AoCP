using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Helper methods for working with neighbourhood offsets in grids.
/// </summary>
public static class Neighbourhoods
{
    private static readonly Coordinate3D<int>[] Orthogonal3D =
    [
        new(1, 0, 0), new(-1, 0, 0),
        new(0, 1, 0), new(0, -1, 0),
        new(0, 0, 1), new(0, 0, -1)
    ];

    /// <summary>
    /// Returns orthogonal neighbours (N/E/S/W) for a given coordinate.
    /// </summary>
    /// <param name="origin">Coordinate whose neighbours should be enumerated.</param>
    /// <typeparam name="T">Numeric coordinate type.</typeparam>
    /// <returns>Neighbouring coordinates in the cardinal directions.</returns>
    public static IEnumerable<IntegerCoordinate<T>> Orthogonal<T>(IntegerCoordinate<T> origin)
        where T : INumber<T>, IBinaryInteger<T> =>
        Neighbours(origin, Neighbourhood.Cardinal);

    /// <summary>
    /// Returns 8 surrounding neighbours in 2D (including diagonals).
    /// </summary>
    /// <param name="origin">Coordinate whose neighbours should be enumerated.</param>
    /// <typeparam name="T">Numeric coordinate type.</typeparam>
    /// <returns>All coordinates adjacent to <paramref name="origin"/>.</returns>
    public static IEnumerable<IntegerCoordinate<T>> All2DNeighbours<T>(IntegerCoordinate<T> origin)
        where T : INumber<T>, IBinaryInteger<T> =>
        Neighbours(origin, Neighbourhood.All);

    /// <summary>
    /// Returns 6 orthogonal neighbours in 3D.
    /// </summary>
    /// <param name="origin">Coordinate whose neighbours should be enumerated.</param>
    /// <typeparam name="T">Numeric coordinate type.</typeparam>
    /// <returns>All orthogonal neighbours of <paramref name="origin"/> in 3D space.</returns>
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

    /// <summary>
    /// Returns the 2D directions for a neighbourhood, optionally excluding any direction that overlaps the mask in <paramref name="excluded"/>.
    /// </summary>
    /// <param name="neighbourhood">Neighbourhood mask to expand.</param>
    /// <param name="excluded">Directions to omit from the result.</param>
    /// <returns>Directions matching the mask.</returns>
    public static IEnumerable<Direction> Directions(Neighbourhood neighbourhood, Direction excluded = Direction.None)
    {
        foreach (var direction in Expand(neighbourhood))
        {
            if ((direction & excluded) != 0)
                continue;

            yield return direction;
        }
    }

    /// <summary>
    /// Returns neighbours of <paramref name="origin"/> based on the requested <paramref name="neighbourhood"/>.
    /// </summary>
    /// <param name="origin">Coordinate whose neighbours should be enumerated.</param>
    /// <param name="neighbourhood">Neighbourhood mask to use.</param>
    /// <param name="excluded">Directions to omit from the result.</param>
    /// <typeparam name="T">Numeric coordinate type.</typeparam>
    /// <returns>Neighbour coordinates based on the neighbourhood mask.</returns>
    public static IEnumerable<IntegerCoordinate<T>> Neighbours<T>(
        IntegerCoordinate<T> origin,
        Neighbourhood neighbourhood = Neighbourhood.Cardinal,
        Direction excluded = Direction.None)
        where T : INumber<T>, IBinaryInteger<T>
    {
        foreach (var direction in Directions(neighbourhood, excluded))
            yield return origin + direction.ToCoordinate<T>();
    }

    /// <summary>
    /// Expands a neighbourhood mask into its constituent directions.
    /// </summary>
    /// <param name="neighbourhood">Neighbourhood mask to expand.</param>
    /// <returns>All directions included in the mask.</returns>
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
