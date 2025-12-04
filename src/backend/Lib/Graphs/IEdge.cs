namespace Lib.Graphs;

public interface IEdge<TNode> where TNode : notnull
{
    /// <summary>
    /// Gets the node this edge originates from.
    /// </summary>
    TNode From { get; }

    /// <summary>
    /// Gets the node this edge points to.
    /// </summary>
    TNode To { get; }
}
