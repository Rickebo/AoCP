namespace Lib.Search;

/// <summary>
/// Represents a neighbouring element in a search graph and the cost to reach it.
/// </summary>
public class SearchNeighbour<TNeighbour, TCost> 
    where TNeighbour : ISearchElement<TCost>
{
    /// <summary>
    /// The neighbouring element.
    /// </summary>
    public required TNeighbour Element { get; init; }

    /// <summary>
    /// Cost to traverse to <see cref="Element"/>.
    /// </summary>
    public required TCost Cost { get; init; }
}


