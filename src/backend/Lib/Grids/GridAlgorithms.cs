using Lib.Coordinate;
using Lib.Enums;

namespace Lib.Grid;

public static class GridAlgorithms
{
    /// <summary>
    /// Flood fill that returns all coordinates reachable from start while the predicate is true.
    /// </summary>
    public static IReadOnlyCollection<IntegerCoordinate<int>> FloodFill(
        ArrayGrid<char> grid,
        IntegerCoordinate<int> start,
        Func<char, bool> predicate
    )
    {
        var visited = new HashSet<IntegerCoordinate<int>>();
        var queue = new Queue<IntegerCoordinate<int>>();

        if (!predicate(grid[start]))
            return visited;

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.TryDequeue(out var current))
        {
            foreach (var neighbour in Neighbourhoods.Orthogonal(current))
            {
                if (neighbour.X < 0 || neighbour.Y < 0 || neighbour.Y >= grid.Height || neighbour.X >= grid.Width)
                    continue;

                if (!predicate(grid[neighbour]))
                    continue;

                if (!visited.Add(neighbour))
                    continue;

                queue.Enqueue(neighbour);
            }
        }

        return visited;
    }
}
