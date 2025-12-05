namespace Lib.Search.Tests;

public class ISearchSourceTests
{
    [Test]
    public void GetNeighbours_ReturnsConfiguredEdges()
    {
        ISearchSource<TestNode, int> source = SearchTestHelpers.CreateDefaultSource();
        var neighbours = source.GetNeighbours(new TestNode("A")).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(neighbours, Has.Length.EqualTo(2));
            Assert.That(neighbours.Select(n => n.Element.Id), Has.Member("B"));
        });
    }
}

