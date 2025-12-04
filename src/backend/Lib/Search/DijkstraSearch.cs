using System.Numerics;

namespace Lib.Search;

/// <summary>
/// Generic Dijkstra search that operates on the existing ISearchSource/ISearchElement abstractions.
/// </summary>
public sealed class DijkstraSearch<TSource, TElement, TCost>(TSource dataset)
    where TSource : ISearchSource<TElement, TCost>
    where TElement : ISearchElement<TCost>
    where TCost : INumber<TCost>
{
    /// <summary>
    /// Gets the dataset supplying neighbours for traversal.
    /// </summary>
    public TSource Dataset { get; } = dataset;

    /// <summary>
    /// Finds the lowest-cost path between <paramref name="start"/> and <paramref name="goal"/> using Dijkstra's algorithm.
    /// </summary>
    /// <param name="start">Element to start from.</param>
    /// <param name="goal">Element to reach.</param>
    /// <returns>A successful result with the path and cost when reachable; otherwise an unsuccessful result.</returns>
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
    /// Base type for Dijkstra search results.
    /// </summary>
    public abstract class DijkstraResult : ISearchResult { }

    /// <summary>
    /// Represents a successful Dijkstra search containing the resulting path and cost.
    /// </summary>
    public sealed class SuccessfulResult : DijkstraResult
    {
        /// <summary>
        /// Gets the total traversal cost of the discovered path.
        /// </summary>
        public required TCost Cost { get; init; }

        /// <summary>
        /// Gets the nodes visited from start to goal.
        /// </summary>
        public required IReadOnlyList<TElement> Path { get; init; }
    }

    /// <summary>
    /// Represents a Dijkstra search that could not reach the goal.
    /// </summary>
    public sealed class UnsuccessfulResult : DijkstraResult { }

    /// <summary>
    /// Reconstructs the path using the stored predecessors from goal back to start.
    /// </summary>
    /// <param name="start">Start element.</param>
    /// <param name="goal">Goal element.</param>
    /// <param name="predecessors">Mapping of elements to their predecessor on the optimal path.</param>
    /// <returns>An ordered list of elements representing the full path.</returns>
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

