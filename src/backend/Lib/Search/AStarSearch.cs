using System.Numerics;

namespace Lib.Search;

/// <summary>
/// A* search built on the shared search abstractions. Requires a heuristic function that never
/// overestimates the true cost (admissible) to guarantee optimality.
/// </summary>
public sealed class AStarSearch<TSource, TElement, TCost>(
    TSource dataset,
    Func<TElement, TElement, TCost> heuristic)
    where TSource : ISearchSource<TElement, TCost>
    where TElement : ISearchElement<TCost>
    where TCost : INumber<TCost>
{
    /// <summary>
    /// Gets the dataset supplying neighbour relationships.
    /// </summary>
    public TSource Dataset { get; } = dataset;

    /// <summary>
    /// Gets the heuristic that estimates the remaining cost between nodes.
    /// </summary>
    public Func<TElement, TElement, TCost> Heuristic { get; } = heuristic;

    /// <summary>
    /// Finds the lowest-cost path between <paramref name="start"/> and <paramref name="goal"/> using A*.
    /// </summary>
    /// <param name="start">Element to start from.</param>
    /// <param name="goal">Destination element.</param>
    /// <returns>A successful result with path and cost when found; otherwise an unsuccessful result.</returns>
    public ISearchResult Find(TElement start, TElement goal)
    {
        var frontier = new System.Collections.Generic.PriorityQueue<TElement, TCost>();
        var costSoFar = new Dictionary<TElement, TCost>();
        var predecessors = new Dictionary<TElement, TElement>();

        frontier.Enqueue(start, Heuristic(start, goal));
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
                    var priority = newCost + Heuristic(neighbour.Element, goal);
                    frontier.Enqueue(neighbour.Element, priority);
                }
            }
        }

        return new UnsuccessfulResult();
    }

    /// <summary>
    /// Base type for A* search results.
    /// </summary>
    public abstract class AStarResult : ISearchResult { }

    /// <summary>
    /// Represents a successful A* search containing the final cost and path.
    /// </summary>
    public sealed class SuccessfulResult : AStarResult
    {
        /// <summary>
        /// Gets the total traversal cost of the path.
        /// </summary>
        public required TCost Cost { get; init; }

        /// <summary>
        /// Gets the sequence of elements that forms the optimal path.
        /// </summary>
        public required IReadOnlyList<TElement> Path { get; init; }
    }

    /// <summary>
    /// Represents an A* search that failed to reach the goal.
    /// </summary>
    public sealed class UnsuccessfulResult : AStarResult { }

    /// <summary>
    /// Rebuilds the path from the start to the goal using the predecessor map.
    /// </summary>
    /// <param name="start">Start element.</param>
    /// <param name="goal">Goal element.</param>
    /// <param name="predecessors">Mapping of each element to its predecessor.</param>
    /// <returns>An ordered path from start to goal.</returns>
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

