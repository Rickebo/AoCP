using System.Numerics;

namespace Lib.Geometry;

public static class LineHelpers
{
    /// <summary>
    /// Enumerates every integer coordinate on a straight segment between start and end, provided the
    /// line is horizontal, vertical, or a 45-degree diagonal. Throws otherwise.
    /// </summary>
    /// <param name="start">Start coordinate for the segment.</param>
    /// <param name="end">End coordinate for the segment.</param>
    /// <returns>All coordinates along the segment, including start and end.</returns>
    /// <exception cref="ArgumentException">Thrown when the line is not axis-aligned or 45 degrees.</exception>
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
    /// Calculates the Manhattan distance between two 2D coordinates.
    /// </summary>
    /// <param name="a">First coordinate.</param>
    /// <param name="b">Second coordinate.</param>
    /// <returns>Manhattan distance between the two points.</returns>
    public static T ManhattanDistance<T>(Coordinate<T> a, Coordinate<T> b) where T : INumber<T>
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return T.Abs(dx) + T.Abs(dy);
    }

    /// <summary>
    /// Calculates the Manhattan distance between two 3D coordinates.
    /// </summary>
    /// <param name="a">First coordinate.</param>
    /// <param name="b">Second coordinate.</param>
    /// <returns>Manhattan distance between the two points.</returns>
    public static T ManhattanDistance<T>(Coordinate3D<T> a, Coordinate3D<T> b) where T : INumber<T>
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return T.Abs(dx) + T.Abs(dy) + T.Abs(dz);
    }
}
