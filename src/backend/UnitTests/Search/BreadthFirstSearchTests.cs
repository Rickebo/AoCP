using Lib.Search;

namespace Lib.UnitTests.Search;

public class BreadthFirstSearchTests
{
    [Test]
    public void Find_ReturnsCostForReachableNode()
    {
        var dataset = SearchTestHelpers.CreateDefaultSource();
        var bfs = new BreadthFirstSearch<TestSearchSource, TestNode, int>(dataset);

        var result = bfs.Find(new TestNode("A"), 0, new TestNode("C"));

        Assert.That(result, Is.InstanceOf<BreadthFirstSearch<TestSearchSource, TestNode, int>.SuccessfulBreadthFirstSearchResult>());
        var success = (BreadthFirstSearch<TestSearchSource, TestNode, int>.SuccessfulBreadthFirstSearchResult)result;
        Assert.That(success.Cost, Is.EqualTo(5));
    }

    [Test]
    public void Find_ReturnsUnsuccessfulWhenMissingPath()
    {
        var dataset = new TestSearchSource(new Dictionary<string, (string To, int Cost)[]>());
        var bfs = new BreadthFirstSearch<TestSearchSource, TestNode, int>(dataset);

        var result = bfs.Find(new TestNode("A"), 0, new TestNode("B"));

        Assert.That(result, Is.InstanceOf<BreadthFirstSearch<TestSearchSource, TestNode, int>.UnsuccessfulBreadthFirstSearchResult>());
    }
}
