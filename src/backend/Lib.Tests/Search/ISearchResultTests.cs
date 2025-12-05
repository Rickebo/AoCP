using Lib.Search;

namespace Lib.Tests.Search;

public class ISearchResultTests
{
    [Test]
    public void ResultTypes_ImplementInterface()
    {
        ISearchResult unsuccessful = new BreadthFirstSearch<TestSearchSource, TestNode, int>.UnsuccessfulBreadthFirstSearchResult();
        ISearchResult dijkstraResult = new DijkstraSearch<TestSearchSource, TestNode, int>.UnsuccessfulResult();
        ISearchResult aStarResult = new AStarSearch<TestSearchSource, TestNode, int>(SearchTestHelpers.CreateDefaultSource(), (_, _) => 0).Find(new TestNode("Z"), new TestNode("Q"));

        Assert.Multiple(() =>
        {
            Assert.That(unsuccessful, Is.InstanceOf<ISearchResult>());
            Assert.That(dijkstraResult, Is.InstanceOf<ISearchResult>());
            Assert.That(aStarResult, Is.InstanceOf<ISearchResult>());
        });
    }
}


