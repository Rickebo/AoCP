namespace Lib.Search;

public class SearchNeighbour<TNeighbour, TCost> 
    where TNeighbour : ISearchElement<TCost>
{
    public required TNeighbour Element { get; init; }
    public required TCost Cost { get; init; }
}
