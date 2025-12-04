namespace Lib.Search;

public interface ISearchSource<TElement, TCost> where TElement : ISearchElement<TCost>
{
    /// <summary>
    /// Retrieves neighbours that can be reached from the given element.
    /// </summary>
    /// <param name="element">Element whose neighbours should be explored.</param>
    /// <returns>A collection of neighbours with their traversal costs.</returns>
    public IEnumerable<SearchNeighbour<TElement, TCost>> GetNeighbours(TElement element);
}


