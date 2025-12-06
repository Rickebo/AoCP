namespace Lib.Search;

/// <summary>
/// Contract for search algorithms operating on a data source.
/// </summary>
public interface ISearchAlgorithm<TSource, TElement, TCost> 
    where TElement : ISearchElement<TCost> 
    where TSource : ISearchSource<TElement, TCost>
{
    /// <summary>
    /// Source that provides neighbours for each element.
    /// </summary>
    TSource Dataset { get; }

    /// <summary>
    /// Executes a search from <paramref name="start"/> to <paramref name="end"/>.
    /// </summary>
    /// <param name="start">Starting element.</param>
    /// <param name="initialCost">Initial cumulative cost.</param>
    /// <param name="end">Target element.</param>
    /// <returns>A search result describing the outcome.</returns>
    ISearchResult Find(TElement start, TCost initialCost, TElement end);

}


