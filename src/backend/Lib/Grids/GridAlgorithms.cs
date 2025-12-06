using Lib.Geometry;

namespace Lib.Grids;

public static class GridAlgorithms
{
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

