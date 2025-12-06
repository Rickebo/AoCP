namespace Lib.Search;

/// <summary>
/// Provides neighbour information for a given search element.
/// </summary>
public interface ISearchSource<TElement, TCost> where TElement : ISearchElement<TCost>
{
    /// <summary>
    /// Retrieves neighbours of the given element and their associated traversal costs.
    /// </summary>
    public IEnumerable<SearchNeighbour<TElement, TCost>> GetNeighbours(TElement element);
}


