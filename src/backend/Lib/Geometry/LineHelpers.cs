using System.Numerics;

namespace Lib.Geometry;

/// <summary>
/// Utility methods for working with line segments and distances.
/// </summary>
public static class LineHelpers
{
    /// <summary>
    /// Enumerates all coordinates on a straight horizontal, vertical, or 45-degree diagonal line segment.
    /// </summary>
    /// <param name="start">Start coordinate (inclusive).</param>
    /// <param name="end">End coordinate (inclusive).</param>
    /// <returns>All coordinates along the segment.</returns>
    /// <exception cref="ArgumentException">Thrown when the segment is not horizontal, vertical, or 45 degrees.</exception>
    public static IEnumerable<Coordinate<int>> EnumerateSegment(Coordinate<int> start, Coordinate<int> end)
    {
        var delta = new Coordinate<int>(end.X - start.X, end.Y - start.Y);
        var stepX = System.Math.Sign(delta.X);
        var stepY = System.Math.Sign(delta.Y);

        if (delta.X != 0 && delta.Y != 0 && System.Math.Abs(delta.X) != System.Math.Abs(delta.Y))
            throw new ArgumentException("Only horizontal, vertical or 45-degree diagonal lines are supported.");

        var current = start;
        yield return current;

        while (current != end)
        {
            current = new Coordinate<int>(current.X + stepX, current.Y + stepY);
            yield return current;
        }
    }

    /// <summary>
    /// Computes the Manhattan distance between two 2D coordinates.
    /// </summary>
    public static T ManhattanDistance<T>(Coordinate<T> a, Coordinate<T> b) where T : INumber<T>
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return T.Abs(dx) + T.Abs(dy);
    }

    /// <summary>
    /// Computes the Manhattan distance between two 3D coordinates.
    /// </summary>
    public static T ManhattanDistance<T>(Coordinate3D<T> a, Coordinate3D<T> b) where T : INumber<T>
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return T.Abs(dx) + T.Abs(dy) + T.Abs(dz);
    }
}
