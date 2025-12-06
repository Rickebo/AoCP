using System.Numerics;

namespace Lib.Search;

/// <summary>
/// Dijkstra's algorithm for shortest path search on weighted graphs.
/// </summary>
/// <typeparam name="TSource">Search source type.</typeparam>
/// <typeparam name="TElement">Element type.</typeparam>
/// <typeparam name="TCost">Cost numeric type.</typeparam>
public sealed class DijkstraSearch<TSource, TElement, TCost>(TSource dataset)
    where TSource : ISearchSource<TElement, TCost>
    where TElement : ISearchElement<TCost>
    where TCost : INumber<TCost>
{
    /// <summary>
    /// Source dataset that provides neighbours.
    /// </summary>
    public TSource Dataset { get; } = dataset;

    /// <summary>
    /// Executes the search between <paramref name="start"/> and <paramref name="goal"/>.
    /// </summary>
    public ISearchResult Find(TElement start, TElement goal)
    {
        var frontier = new System.Collections.Generic.PriorityQueue<TElement, TCost>();
        var costSoFar = new Dictionary<TElement, TCost>();
        var predecessors = new Dictionary<TElement, TElement>();

        frontier.Enqueue(start, TCost.Zero);
        costSoFar[start] = TCost.Zero;

        while (frontier.TryDequeue(out var current, out _))
        {
            if (current.Equals(goal))
            {
                return new SuccessfulResult
                {
                    Cost = costSoFar[current],
                    Path = ReconstructPath(start, goal, predecessors)
                };
            }

            foreach (var neighbour in Dataset.GetNeighbours(current))
            {
                var newCost = costSoFar[current] + neighbour.Cost;
                if (!costSoFar.TryGetValue(neighbour.Element, out var existingCost) || newCost < existingCost)
                {
                    costSoFar[neighbour.Element] = newCost;
                    predecessors[neighbour.Element] = current;
                    frontier.Enqueue(neighbour.Element, newCost);
                }
            }
        }

        return new UnsuccessfulResult();
    }

    /// <summary>
    /// Base class for Dijkstra search results.
    /// </summary>
    public abstract class DijkstraResult : ISearchResult { }

    /// <summary>
    /// Successful result containing cost and path.
    /// </summary>
    public sealed class SuccessfulResult : DijkstraResult
    {
        /// <summary>
        /// Total cost from start to goal.
        /// </summary>
        public required TCost Cost { get; init; }

        /// <summary>
        /// Path of elements from start to goal.
        /// </summary>
        public required IReadOnlyList<TElement> Path { get; init; }
    }

    /// <summary>
    /// Returned when no path is found.
    /// </summary>
    public sealed class UnsuccessfulResult : DijkstraResult { }

    private static IReadOnlyList<TElement> ReconstructPath(TElement start, TElement goal, IReadOnlyDictionary<TElement, TElement> predecessors)
    {
        var path = new List<TElement> { goal };
        var current = goal;
        while (!current.Equals(start) && predecessors.TryGetValue(current, out var prev))
        {
            current = prev;
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}

