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
    public TSource Dataset { get; } = dataset;
    public Func<TElement, TElement, TCost> Heuristic { get; } = heuristic;

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

    public abstract class AStarResult : ISearchResult { }

    public sealed class SuccessfulResult : AStarResult
    {
        public required TCost Cost { get; init; }
        public required IReadOnlyList<TElement> Path { get; init; }
    }

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
