using Backend.Problems;
using Backend.Problems.Metadata;
using Common;

namespace Backend.Services;

public class ProblemService
{
    private readonly Dictionary<int, ProblemCollection> _problemCollections;
    private readonly Dictionary<int, IndexedProblemCollection> _indexedProblemCollections;

    public ProblemService(IEnumerable<ProblemCollection> problemCollections)
    {
        var collections = problemCollections.ToArray();

        _problemCollections = collections
            .ToDictionary(col => col.Year, col => col);

        _indexedProblemCollections = collections.ToDictionary(
            col => col.Year,
            col => new IndexedProblemCollection(col)
        );
    }

    private void ValidateYear(int year)
    {
        if (year is < 2015 or > 2100)
            throw new InvalidOperationException(
                "Invalid year"
            );
    }

    public void Validate(ProblemsMetadata metadata)
    {
        foreach (var problemCollection in metadata.Collections.Values)
        {
            ValidateYear(problemCollection.Year);

            foreach (var authorProblems in problemCollection.ProblemSets)
            {
                if (string.IsNullOrEmpty(authorProblems.Key))
                    throw new InvalidOperationException(
                        "Invalid author name: null or empty."
                    );

                foreach (var problemSet in authorProblems.Value)
                {
                    if (string.IsNullOrEmpty(problemSet.Name))
                        throw new InvalidOperationException(
                            "Problem set name cannot be empty"
                        );

                    if (problemSet.ReleaseTime.Year != problemCollection.Year)
                        throw new InvalidOperationException(
                            "Problem set release time must be in the same year as the collection"
                        );

                    foreach (var problem in problemSet.Problems)
                    {
                        if (string.IsNullOrEmpty(problem.Name))
                            throw new InvalidOperationException(
                                "Problem name cannot be empty"
                            );
                    }
                }
            }
        }
    }

    public ProblemsMetadata GetMetadata() =>
        new()
        {
            Collections = _problemCollections
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value.GetMetadata()
                )
        };

    public Problem? GetProblem(ProblemId problemId) =>
        _indexedProblemCollections.TryGetValue(problemId.Year, out var collection)
            ? collection.Get(problemId)
            : null;

    private record ProblemCollectionKey(int Year, string Author);

    private class IndexedProblemCollection
    {
        private Dictionary<string, Dictionary<string, ProblemSet>> ProblemSets { get; } =
            new();

        private Dictionary<string, Dictionary<string, Dictionary<string, Problem>>>
            Problems { get; } =
            new();

        public IndexedProblemCollection(ProblemCollection collection)
        {
            foreach (var authorSets in collection.Problems)
            {
                if (!ProblemSets.TryGetValue(authorSets.Key, out var problemSets))
                    problemSets = ProblemSets[authorSets.Key] = new();

                if (!Problems.TryGetValue(authorSets.Key, out var problems))
                    problems = Problems[authorSets.Key] = new();

                foreach (var set in authorSets.Value)
                {
                    problemSets[set.Name] = set;
                    problems[set.Name] = set.Problems
                        .Where(p => p.Name != null)
                        .ToDictionary(p => p.Name!, p => p);
                }
            }
        }

        public Problem? Get(ProblemId id) =>
            !Problems.TryGetValue(id.Author, out var authorProblems) ||
            !authorProblems.TryGetValue(id.SetName, out var setProblems)
                ? null
                : setProblems.GetValueOrDefault(id.ProblemName);
    }
}