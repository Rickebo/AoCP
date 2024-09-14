using Backend.Problems;
using Backend.Problems.Metadata;

namespace Backend.Services;

public class ProblemService
{
    private readonly Dictionary<int, ProblemCollection> _problemCollections;

    public ProblemService(IEnumerable<ProblemCollection> problemCollections)
    {
        _problemCollections =
            problemCollections.ToDictionary(col => col.Year, col => col);
    }

    public ProblemsMetadata GetMetadata()
    {
        return new ProblemsMetadata(
            _problemCollections
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value.GetMetadata()
                )
        );
    }
}