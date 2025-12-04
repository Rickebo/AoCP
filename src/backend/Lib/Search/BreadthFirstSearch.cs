using System.Numerics;

namespace Lib.Search;

/// <summary>
/// Basic breadth-first search implementation for the shared search abstractions.
/// </summary>
public class BreadthFirstSearch<TSource, TElement, TCost>(TSource dataset) : ISearchAlgorithm<TSource, TElement, TCost> 
    where TSource : ISearchSource<TElement, TCost>
    where TElement : ISearchElement<TCost>
    where TCost : INumber<TCost>
{
    /// <summary>
    /// Gets the search dataset providing neighbours.
    /// </summary>
    public TSource Dataset { get; init; } = dataset;

    /// <summary>
    /// Performs a breadth-first traversal until the end element is reached or no nodes remain.
    /// </summary>
    /// <param name="start">Element to begin the search from.</param>
    /// <param name="initialCost">Initial cost applied to the starting element.</param>
    /// <param name="end">Target element to locate.</param>
    /// <returns>A successful result when the target is found; otherwise an unsuccessful result.</returns>
    public ISearchResult Find(TElement start, TCost initialCost, TElement end)
    {
        var frontier = new Queue<(TElement Element, TCost Cost)>();
        var visited = new HashSet<TElement>();
        frontier.Enqueue((start, initialCost));

        while (frontier.Count > 0)
        {
            var (currentElement, currentCost) = frontier.Dequeue();
            if (!visited.Add(currentElement))
                continue;

            if (currentElement.Equals(end))
                return new SuccessfulBreadthFirstSearchResult
                {
                    Cost = currentCost
                };

            foreach (var neighbour in Dataset.GetNeighbours(currentElement))
            {
                if (visited.Contains(neighbour.Element))
                    continue;

                var nextCost = currentCost + neighbour.Cost;
                if (neighbour.Element.Equals(end))
                    return new SuccessfulBreadthFirstSearchResult
                    {
                        Cost = nextCost
                    };

                frontier.Enqueue((neighbour.Element, nextCost));
            }
        }

        return new UnsuccessfulBreadthFirstSearchResult();
    }

    /// <summary>
    /// Base type for breadth-first search results.
    /// </summary>
    public abstract class BreadthFirstSearchResult : ISearchResult
    {

    }

    /// <summary>
    /// Represents a successful breadth-first search with the resulting traversal cost.
    /// </summary>
    public class SuccessfulBreadthFirstSearchResult : BreadthFirstSearchResult
    {
        /// <summary>
        /// Gets the cumulative cost from the start node to the found end node.
        /// </summary>
        public required TCost Cost { get; init; }
    }

    /// <summary>
    /// Represents an unsuccessful breadth-first search.
    /// </summary>
    public class UnsuccessfulBreadthFirstSearchResult : BreadthFirstSearchResult
    {

    }
}


