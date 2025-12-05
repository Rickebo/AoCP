using Lib.Search;

namespace Lib.Tests.Search;

public class ISearchAlgorithmTests
{
    [Test]
    public void BreadthFirstSearch_ExposesDatasetThroughInterface()
    {
        var dataset = SearchTestHelpers.CreateDefaultSource();

        Assert.That(new BreadthFirstSearch<TestSearchSource, TestNode, int>(dataset).Dataset, Is.EqualTo(dataset));
    }
}


