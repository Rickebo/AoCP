using System.Numerics;

namespace Lib.Search;

/// <summary>
/// Breadth-first search implementation for unweighted graphs.
/// </summary>
/// <typeparam name="TSource">Search source type.</typeparam>
/// <typeparam name="TElement">Element type.</typeparam>
/// <typeparam name="TCost">Cost numeric type.</typeparam>
public class BreadthFirstSearch<TSource, TElement, TCost>(TSource dataset) : ISearchAlgorithm<TSource, TElement, TCost> 
    where TSource : ISearchSource<TElement, TCost>
    where TElement : ISearchElement<TCost>
    where TCost : INumber<TCost>
{
    /// <summary>
    /// Source dataset that provides neighbours.
    /// </summary>
    public TSource Dataset { get; init; } = dataset;

    /// <summary>
    /// Runs breadth-first search from <paramref name="start"/> to <paramref name="end"/>.
    /// </summary>
    /// <param name="start">Starting element.</param>
    /// <param name="initialCost">Initial cost value.</param>
    /// <param name="end">Target element.</param>
    /// <returns>Search result indicating success or failure.</returns>
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
    /// Base result for breadth-first search.
    /// </summary>
    public abstract class BreadthFirstSearchResult : ISearchResult
    {

    }

    /// <summary>
    /// Successful breadth-first search result with cumulative cost.
    /// </summary>
    public class SuccessfulBreadthFirstSearchResult : BreadthFirstSearchResult
    {
        /// <summary>
        /// Total cost accumulated along the found path.
        /// </summary>
        public required TCost Cost { get; init; }
    }

    /// <summary>
    /// Returned when no path is found.
    /// </summary>
    public class UnsuccessfulBreadthFirstSearchResult : BreadthFirstSearchResult
    {

    }
}


