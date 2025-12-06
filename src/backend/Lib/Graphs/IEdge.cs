namespace Lib.Graphs;

/// <summary>
/// Represents a directed connection between two nodes.
/// </summary>
public interface IEdge<TNode> where TNode : notnull
{
    /// <summary>
    /// Source node.
    /// </summary>
    TNode From { get; }

    /// <summary>
    /// Destination node.
    /// </summary>
    TNode To { get; }
}
