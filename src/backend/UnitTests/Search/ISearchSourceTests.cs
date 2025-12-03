using Lib.Search;

namespace Lib.UnitTests.Search;

public class ISearchSourceTests
{
    [Test]
    public void GetNeighbours_ReturnsConfiguredEdges()
    {
        ISearchSource<TestNode, int> source = SearchTestHelpers.CreateDefaultSource();
        var neighbours = source.GetNeighbours(new TestNode("A")).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(neighbours.Length, Is.EqualTo(2));
            CollectionAssert.Contains(neighbours.Select(n => n.Element.Id), "B");
        });
    }
}
