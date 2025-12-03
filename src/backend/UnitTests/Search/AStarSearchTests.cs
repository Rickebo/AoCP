using Lib.Search;

namespace Lib.Search.Tests;

public class AStarSearchTests
{
    [Test]
    public void Find_UsesHeuristicToReachGoal()
    {
        var source = SearchTestHelpers.CreateDefaultSource();
        var search = new AStarSearch<TestSearchSource, TestNode, int>(
            source,
            (node, _) => node.Id == "C" ? 0 : 1); // simple admissible heuristic

        var result = search.Find(new TestNode("A"), new TestNode("C"));

        Assert.That(result, Is.InstanceOf<AStarSearch<TestSearchSource, TestNode, int>.SuccessfulResult>());
        var success = (AStarSearch<TestSearchSource, TestNode, int>.SuccessfulResult)result;

        Assert.Multiple(() =>
        {
            Assert.That(success.Cost, Is.EqualTo(2));
            CollectionAssert.AreEqual(new[] { new TestNode("A"), new TestNode("B"), new TestNode("C") }, success.Path);
        });
    }

    [Test]
    public void Find_ReturnsUnsuccessfulWhenGoalIsUnreachable()
    {
        var source = new TestSearchSource(new Dictionary<string, (string To, int Cost)[]>());
        var search = new AStarSearch<TestSearchSource, TestNode, int>(source, (_, _) => 0);

        var result = search.Find(new TestNode("A"), new TestNode("B"));

        Assert.That(result, Is.InstanceOf<AStarSearch<TestSearchSource, TestNode, int>.UnsuccessfulResult>());
    }
}

