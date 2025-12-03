using Lib.Search;

namespace Lib.Search.Tests;

public class SearchNeighbourTests
{
    [Test]
    public void SearchNeighbour_HoldsElementAndCost()
    {
        var neighbour = new SearchNeighbour<TestNode, int>
        {
            Element = new TestNode("X"),
            Cost = 3
        };

        Assert.Multiple(() =>
        {
            Assert.That(neighbour.Element.Id, Is.EqualTo("X"));
            Assert.That(neighbour.Cost, Is.EqualTo(3));
        });
    }
}

