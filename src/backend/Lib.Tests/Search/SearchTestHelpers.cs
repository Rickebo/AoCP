using Lib.Search;

namespace Lib.Tests.Search;

internal readonly record struct TestNode(string Id) : ISearchElement<int>;

internal sealed class TestSearchSource : ISearchSource<TestNode, int>
{
    private readonly Dictionary<TestNode, List<SearchNeighbour<TestNode, int>>> _edges;

    public TestSearchSource(Dictionary<string, (string To, int Cost)[]> edges)
    {
        _edges = edges.ToDictionary(
            pair => new TestNode(pair.Key),
            pair => pair.Value.Select(v => new SearchNeighbour<TestNode, int>
            {
                Element = new TestNode(v.To),
                Cost = v.Cost
            }).ToList());
    }

    public IEnumerable<SearchNeighbour<TestNode, int>> GetNeighbours(TestNode element) =>
        _edges.TryGetValue(element, out var list) ? list : [];
}

internal static class SearchTestHelpers
{
    public static TestSearchSource CreateDefaultSource()
    {
        return new TestSearchSource(
            new Dictionary<string, (string To, int Cost)[]>
            {
                ["A"] = [("B", 1), ("C", 5)],
                ["B"] = [("C", 1)],
                ["C"] = [],
            });
    }
}


