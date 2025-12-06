namespace Lib.Graphs;

/// <summary>
/// Simple directed graph implementation with adjacency lookups.
/// </summary>
/// <typeparam name="TNode">Type representing nodes.</typeparam>
/// <typeparam name="TEdge">Type representing edges.</typeparam>
public class Graph<TNode, TEdge> where TNode : notnull where TEdge : IEdge<TNode>
{
    private readonly HashSet<TEdge> _noEdge = [];
    private readonly HashSet<TNode> _nodes = [];
    private readonly HashSet<TEdge> _edges = [];

    private readonly Dictionary<TNode, HashSet<TEdge>> _nodeSource = new();

    private readonly Dictionary<TNode, HashSet<TEdge>> _nodeDestination = new();


    /// <summary>
    /// Initializes an empty graph.
    /// </summary>
    public Graph()
    {
    }

    /// <summary>
    /// Determines whether there is at least one edge from <paramref name="source"/> to <paramref name="destination"/>.
    /// </summary>
    public bool IsConnected(TNode source, TNode destination) =>
        // Compare destination nodes rather than edges themselves.
        GetSourceEdges(source).Any(edge => edge.To.Equals(destination));

    /// <summary>
    /// Returns the immediate neighbours reachable from <paramref name="node"/>.
    /// </summary>
    public IEnumerable<TNode> GetNeighbours(TNode node) => GetSourceEdges(node).Select(edge => edge.To);

    /// <summary>
    /// Gets edges leaving a node.
    /// </summary>
    public IReadOnlySet<TEdge> GetSourceEdges(TNode node) =>
        _nodeSource.GetValueOrDefault(node, _noEdge);

    /// <summary>
    /// Gets edges entering a node.
    /// </summary>
    public IReadOnlySet<TEdge> GetDestinationEdges(TNode node) =>
        _nodeDestination.GetValueOrDefault(node, _noEdge);

    /// <summary>
    /// All edges in the graph.
    /// </summary>
    public IReadOnlySet<TEdge> Edges => _edges;

    /// <summary>
    /// All nodes in the graph.
    /// </summary>
    public IReadOnlySet<TNode> Nodes => _nodes;

    /// <summary>
    /// Adds a node to the graph.
    /// </summary>
    /// <returns><see langword="true"/> when the node was added; <see langword="false"/> if it already existed.</returns>
    public bool AddNode(TNode node)
    {
        if (!_nodes.Add(node))
            return false;

        _nodeSource[node] = [];
        _nodeDestination[node] = [];
        return true;
    }

    /// <summary>
    /// Adds an edge to the graph.
    /// </summary>
    /// <returns><see langword="true"/> when the edge was added; otherwise <see langword="false"/>.</returns>
    public bool AddEdge(TEdge edge)
    {
        if (!_nodes.Contains(edge.From) || !_nodes.Contains(edge.To) || !_edges.Add(edge))
            return false;

        _nodeSource[edge.From].Add(edge);
        _nodeDestination[edge.To].Add(edge);
        return true;
    }

    /// <summary>
    /// Removes an edge if it exists in the graph.
    /// </summary>
    public bool RemoveEdge(TEdge edge)
    {
        if (!_nodes.Contains(edge.From) || !_nodes.Contains(edge.To))
            return false;

        if (!_edges.Contains(edge))
            return false;

        _nodeSource[edge.From].Remove(edge);
        _nodeDestination[edge.To].Remove(edge);
        _edges.Remove(edge);
        return true;
    }

    /// <summary>
    /// Removes a node and any edges incident to it.
    /// </summary>
    public bool RemoveNode(TNode node)
    {
        if (!_nodes.Remove(node))
            return false;

        var comparer = EqualityComparer<TNode>.Default;
        var edgesToKeep = new List<TEdge>();
        foreach (var edge in _edges)
        {
            if (comparer.Equals(edge.From, node) || comparer.Equals(edge.To, node))
            {
                if (_nodeSource.TryGetValue(edge.From, out var fromSet))
                    fromSet.Remove(edge);
                if (_nodeDestination.TryGetValue(edge.To, out var toSet))
                    toSet.Remove(edge);
            }
            else
            {
                edgesToKeep.Add(edge);
            }
        }

        _edges.Clear();
        foreach (var edge in edgesToKeep)
            _edges.Add(edge);

        // Finally drop the node lookup entries.
        _nodeSource.Remove(node);
        _nodeDestination.Remove(node);
        return true;
    }
}

