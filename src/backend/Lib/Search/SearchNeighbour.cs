namespace Lib.Search;

public class SearchNeighbour<TNeighbour, TCost> 
    where TNeighbour : ISearchElement<TCost>
{
    /// <summary>
    /// Gets the neighbouring element reachable from the current node.
    /// </summary>
    public required TNeighbour Element { get; init; }

    /// <summary>
    /// Gets the traversal cost to reach <see cref="Element"/>.
    /// </summary>
    public required TCost Cost { get; init; }
}


