using Lib.Search;

namespace Lib.UnitTests.Search;

public class ISearchAlgorithmTests
{
    [Test]
    public void BreadthFirstSearch_ExposesDatasetThroughInterface()
    {
        var dataset = SearchTestHelpers.CreateDefaultSource();
        ISearchAlgorithm<TestSearchSource, TestNode, int> algorithm = new BreadthFirstSearch<TestSearchSource, TestNode, int>(dataset);

        Assert.That(algorithm.Dataset, Is.EqualTo(dataset));
    }
}
