using Lib.Graphs;

namespace Lib.Graphs.Tests;

public class IEdgeTests
{
    private readonly record struct SimpleEdge(string From, string To) : IEdge<string>;

    [Test]
    public void EdgeExposesFromAndTo()
    {
        IEdge<string> edge = new SimpleEdge("from", "to");

        Assert.Multiple(() =>
        {
            Assert.That(edge.From, Is.EqualTo("from"));
            Assert.That(edge.To, Is.EqualTo("to"));
        });
    }
}

