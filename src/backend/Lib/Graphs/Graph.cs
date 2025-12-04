namespace Lib.Graphs;

/// <summary>
/// Lightweight directed graph with adjacency lookups for nodes and edges.
/// </summary>
public class Graph<TNode, TEdge> where TNode : notnull where TEdge : IEdge<TNode>
{
    private readonly HashSet<TEdge> _noEdge = [];
    private readonly HashSet<TNode> _nodes = [];
    private readonly HashSet<TEdge> _edges = [];

    /// <summary>
    /// A dictionary mapping a node to a set of edges starting from the node.
    /// </summary>
    private readonly Dictionary<TNode, HashSet<TEdge>> _nodeSource = new();

    /// <summary>
    /// A dictionary mapping a node to a set of edges ending in the node.
    /// </summary>
    private readonly Dictionary<TNode, HashSet<TEdge>> _nodeDestination = new();


    /// <summary>
    /// Initializes an empty directed graph.
    /// </summary>
    public Graph()
    {
    }

    /// <summary>
    /// Determines whether a destination node is reachable from a source node via a single edge.
    /// </summary>
    /// <param name="source">Node to start from.</param>
    /// <param name="destination">Node to reach.</param>
    /// <returns><c>true</c> if there is an outgoing edge to <paramref name="destination"/>; otherwise <c>false</c>.</returns>
    public bool IsConnected(TNode source, TNode destination) =>
        // Compare destination nodes rather than edges themselves.
        GetSourceEdges(source).Any(edge => edge.To.Equals(destination));

    /// <summary>
    /// Returns all nodes that are reachable from the specified node via one outgoing edge.
    /// </summary>
    /// <param name="node">Node whose neighbours should be returned.</param>
    /// <returns>All nodes directly connected from <paramref name="node"/>.</returns>
    public IEnumerable<TNode> GetNeighbours(TNode node) => GetSourceEdges(node).Select(edge => edge.To);

    /// <summary>
    /// Gets the set of edges that originate from the given node.
    /// </summary>
    /// <param name="node">Node whose outgoing edges to retrieve.</param>
    /// <returns>A read-only set of outgoing edges, or an empty set if none exist.</returns>
    public IReadOnlySet<TEdge> GetSourceEdges(TNode node) =>
        _nodeSource.GetValueOrDefault(node, _noEdge);

    /// <summary>
    /// Gets the set of edges that end at the given node.
    /// </summary>
    /// <param name="node">Node whose incoming edges to retrieve.</param>
    /// <returns>A read-only set of incoming edges, or an empty set if none exist.</returns>
    public IReadOnlySet<TEdge> GetDestinationEdges(TNode node) =>
        _nodeDestination.GetValueOrDefault(node, _noEdge);

    /// <summary>
    /// Gets all edges present in the graph.
    /// </summary>
    public IReadOnlySet<TEdge> Edges => _edges;

    /// <summary>
    /// Gets all nodes present in the graph.
    /// </summary>
    public IReadOnlySet<TNode> Nodes => _nodes;

    /// <summary>
    /// Adds a node to the graph if it is not already present.
    /// </summary>
    /// <param name="node">Node to add.</param>
    /// <returns><c>true</c> if the node was added; <c>false</c> if it already existed.</returns>
    public bool AddNode(TNode node)
    {
        if (!_nodes.Add(node))
            return false;

        _nodeSource[node] = [];
        _nodeDestination[node] = [];
        return true;
    }

    /// <summary>
    /// Adds an edge to the graph when both its endpoints are present.
    /// </summary>
    /// <param name="edge">Edge to add.</param>
    /// <returns><c>true</c> if the edge was added; otherwise <c>false</c>.</returns>
    public bool AddEdge(TEdge edge)
    {
        if (!_nodes.Contains(edge.From) || !_nodes.Contains(edge.To) || !_edges.Add(edge))
            return false;

        _nodeSource[edge.From].Add(edge);
        _nodeDestination[edge.To].Add(edge);
        return true;
    }

    /// <summary>
    /// Removes an edge from the graph when it exists.
    /// </summary>
    /// <param name="edge">Edge to remove.</param>
    /// <returns><c>true</c> if the edge was removed; otherwise <c>false</c>.</returns>
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
    /// Removes a node and all incident edges from the graph.
    /// </summary>
    /// <param name="node">Node to remove.</param>
    /// <returns><c>true</c> if the node was removed; otherwise <c>false</c>.</returns>
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

