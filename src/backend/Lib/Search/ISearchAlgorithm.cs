namespace Lib.Search;

public interface ISearchAlgorithm<TSource, TElement, TCost> 
    where TElement : ISearchElement<TCost> 
    where TSource : ISearchSource<TElement, TCost>
{
    /// <summary>
    /// Gets the dataset the algorithm traverses.
    /// </summary>
    TSource Dataset { get; }

    /// <summary>
    /// Performs a search between the provided start and end elements.
    /// </summary>
    /// <param name="start">Element to start from.</param>
    /// <param name="initialCost">Starting cost for the search.</param>
    /// <param name="end">Element to search for.</param>
    /// <returns>A search result describing whether a path was found.</returns>
    ISearchResult Find(TElement start, TCost initialCost, TElement end);

}


