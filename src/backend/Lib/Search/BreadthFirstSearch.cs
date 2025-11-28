using System.Numerics;

namespace Lib.Search
{
    public class BreadthFirstSearch<TSource, TElement, TCost>(TSource dataset) : ISearchAlgorithm<TSource, TElement, TCost> 
        where TSource : ISearchSource<TElement, TCost>
        where TElement : ISearchElement<TCost>
        where TCost : INumber<TCost>
    {
        public TSource Dataset { get; init; } = dataset;

        public ISearchResult Find(TElement start, TCost initialCost, TElement end)
        {
            var frontier = new PriorityQueue<TElement, TCost>();
            frontier.Enqueue(start, initialCost);

            while (frontier.TryDequeue(out var currentElement, out var currentCost))
            {
                foreach (var neighbour in Dataset.GetNeighbours(currentElement))
                {
                    if (neighbour.Element.Equals(end))
                        return new SuccessfulBreadthFirstSearchResult
                        {
                            Cost = currentCost
                        };

                    frontier.Enqueue(neighbour.Element, currentCost + neighbour.Cost);
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
}
