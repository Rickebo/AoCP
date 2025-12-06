using System.Numerics;

namespace Lib.Search;

public class BreadthFirstSearch<TSource, TElement, TCost>(TSource dataset) : ISearchAlgorithm<TSource, TElement, TCost> 
    where TSource : ISearchSource<TElement, TCost>
    where TElement : ISearchElement<TCost>
    where TCost : INumber<TCost>
{
    public TSource Dataset { get; init; } = dataset;

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

    public abstract class BreadthFirstSearchResult : ISearchResult
    {

    }

    public class SuccessfulBreadthFirstSearchResult : BreadthFirstSearchResult
    {
        public required TCost Cost { get; init; }
    }

    public class UnsuccessfulBreadthFirstSearchResult : BreadthFirstSearchResult
    {

    }
}


