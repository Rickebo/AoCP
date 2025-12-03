namespace Lib.Search;

public interface ISearchSource<TElement, TCost> where TElement : ISearchElement<TCost>
{
    public IEnumerable<SearchNeighbour<TElement, TCost>> GetNeighbours(TElement element);
}


