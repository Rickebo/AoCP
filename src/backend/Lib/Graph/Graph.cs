namespace Lib.Graph;

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


    public Graph()
    {
    }

    public bool IsConnected(TNode source, TNode destination) =>
        GetSourceEdges(source).Any(edge => edge.Equals(destination));

    public IEnumerable<TNode> GetNeighbours(TNode node) => GetSourceEdges(node).Select(edge => edge.To);

    public IReadOnlySet<TEdge> GetSourceEdges(TNode node) =>
        _nodeSource.GetValueOrDefault(node, _noEdge);

    public IReadOnlySet<TEdge> GetDestinationEdges(TNode node) =>
        _nodeDestination.GetValueOrDefault(node, _noEdge);

    public IReadOnlySet<TEdge> Edges => _edges;
    public IReadOnlySet<TNode> Nodes => _nodes;

    public bool AddNode(TNode node)
    {
        if (!_nodes.Add(node))
            return false;

        _nodeSource[node] = [];
        _nodeDestination[node] = [];
        return true;
    }

    public bool AddEdge(TEdge edge)
    {
        if (!_nodes.Contains(edge.From) || !_nodes.Contains(edge.To) || !_edges.Add(edge))
            return false;

        _nodeSource[edge.From].Add(edge);
        _nodeDestination[edge.To].Add(edge);
        return true;
    }

    public bool RemoveEdge(TEdge edge)
    {
        if (!_nodes.Contains(edge.From) || !_nodes.Contains(edge.To))
            return false;

        if (!_edges.Contains(edge))
            return false;

        _nodeSource[edge.From].Remove(edge);
        _nodeDestination[edge.To].Remove(edge);
        return true;
    }

    public bool RemoveNode(TNode node)
    {
        if (!_nodes.Remove(node))
            return false;

        // Remove all edges ending in the node
        if (_nodeDestination[node].Any(edge => !_nodeSource[edge.From].Remove(edge) || !_edges.Remove(edge)))
            throw new Exception("Failed to remove node source edge from graph.");

        if (_nodeSource[node].Any(edge => !_nodeDestination[edge.To].Remove(edge) || !_edges.Remove(edge)))
            throw new Exception("Failed to remove node destination edge from graph.");

        if (!_nodeSource.Remove(node) || !_nodeDestination.Remove(node))
            throw new Exception("Failed to remove node from graph.");

        return true;
    }
}