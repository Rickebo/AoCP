using Lib.Search;

namespace Lib.Tests.Search;

public class DijkstraSearchTests
{
    [Test]
    public void Find_ReturnsShortestPathAndCost()
    {
        var source = SearchTestHelpers.CreateDefaultSource();
        var search = new DijkstraSearch<TestSearchSource, TestNode, int>(source);

        var result = search.Find(new TestNode("A"), new TestNode("C"));

        Assert.That(result, Is.InstanceOf<DijkstraSearch<TestSearchSource, TestNode, int>.SuccessfulResult>());

        var success = (DijkstraSearch<TestSearchSource, TestNode, int>.SuccessfulResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(success.Cost, Is.EqualTo(2));
            Assert.That(success.Path, Is.EqualTo(new[] { new TestNode("A"), new TestNode("B"), new TestNode("C") }).AsCollection);
        });
    }

    [Test]
    public void Find_ReturnsUnsuccessfulWhenNoPathExists()
    {
        var source = new TestSearchSource([]);
        var search = new DijkstraSearch<TestSearchSource, TestNode, int>(source);

        var result = search.Find(new TestNode("A"), new TestNode("B"));

        Assert.That(result, Is.InstanceOf<DijkstraSearch<TestSearchSource, TestNode, int>.UnsuccessfulResult>());
    }
}


