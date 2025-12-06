using System.Numerics;

namespace Lib.Search;

/// <summary>
/// Generic A* search implementation.
/// </summary>
/// <typeparam name="TSource">Search source type.</typeparam>
/// <typeparam name="TElement">Element type.</typeparam>
/// <typeparam name="TCost">Numeric cost type.</typeparam>
public sealed class AStarSearch<TSource, TElement, TCost>(
    TSource dataset,
    Func<TElement, TElement, TCost> heuristic)
    where TSource : ISearchSource<TElement, TCost>
    where TElement : ISearchElement<TCost>
    where TCost : INumber<TCost>
{
    /// <summary>
    /// Source dataset that provides neighbours.
    /// </summary>
    public TSource Dataset { get; } = dataset;

    /// <summary>
    /// Heuristic function estimating the cost between two elements.
    /// </summary>
    public Func<TElement, TElement, TCost> Heuristic { get; } = heuristic;

    /// <summary>
    /// Executes the A* search between two elements.
    /// </summary>
    /// <param name="start">Start element.</param>
    /// <param name="goal">Target element.</param>
    /// <returns>A search result describing success or failure.</returns>
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
    /// Successful search result.
    /// </summary>
    public sealed class SuccessfulResult : AStarResult
    {
        /// <summary>
        /// Total path cost.
        /// </summary>
        public required TCost Cost { get; init; }

        /// <summary>
        /// Reconstructed path from start to goal.
        /// </summary>
        public required IReadOnlyList<TElement> Path { get; init; }
    }

    /// <summary>
    /// Returned when no path is found.
    /// </summary>
    public sealed class UnsuccessfulResult : AStarResult { }

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

