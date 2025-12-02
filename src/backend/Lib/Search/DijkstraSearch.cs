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
    public TSource Dataset { get; } = dataset;

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

    public abstract class DijkstraResult : ISearchResult { }

    public sealed class SuccessfulResult : DijkstraResult
    {
        public required TCost Cost { get; init; }
        public required IReadOnlyList<TElement> Path { get; init; }
    }

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
