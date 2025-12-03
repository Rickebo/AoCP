using Lib.Graphs;

namespace Lib.Graphs.Tests;

public class GraphTests
{
    private readonly record struct TestEdge(string From, string To) : IEdge<string>;

    [Test]
    public void AddNode_PreventsDuplicates()
    {
        var graph = new Graph<string, TestEdge>();

        Assert.Multiple(() =>
        {
            Assert.That(graph.AddNode("A"), Is.True);
            Assert.That(graph.AddNode("A"), Is.False);
            Assert.That(graph.Nodes, Contains.Item("A"));
        });
    }

    [Test]
    public void AddEdge_RequiresExistingNodes()
    {
        var graph = new Graph<string, TestEdge>();
        graph.AddNode("A");

        Assert.That(graph.AddEdge(new TestEdge("A", "B")), Is.False);

        graph.AddNode("B");
        Assert.That(graph.AddEdge(new TestEdge("A", "B")), Is.True);
    }

    [Test]
    public void ConnectivityAndNeighbours_UseEdgeDestinations()
    {
        var graph = new Graph<string, TestEdge>();
        graph.AddNode("A");
        graph.AddNode("B");
        graph.AddEdge(new TestEdge("A", "B"));

        Assert.Multiple(() =>
        {
            Assert.That(graph.IsConnected("A", "B"), Is.True);
            Assert.That(graph.IsConnected("B", "A"), Is.False);
            CollectionAssert.AreEqual(new[] { "B" }, graph.GetNeighbours("A"));
            Assert.That(graph.GetSourceEdges("A"), Has.Count.EqualTo(1));
            Assert.That(graph.GetDestinationEdges("B"), Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void RemoveEdgeAndNode_UpdateGraph()
    {
        var graph = new Graph<string, TestEdge>();
        graph.AddNode("A");
        graph.AddNode("B");
        var edge = new TestEdge("A", "B");
        graph.AddEdge(edge);

        Assert.That(graph.RemoveEdge(new TestEdge("B", "A")), Is.False);
        Assert.That(graph.RemoveEdge(edge), Is.True);
        Assert.That(graph.Edges, Is.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(graph.AddEdge(edge), Is.True);
            Assert.That(graph.RemoveNode("A"), Is.True);
            Assert.That(graph.Nodes.Contains("A"), Is.False);
            Assert.That(graph.Edges, Is.Empty);
        });
    }
}

