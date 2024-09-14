using Backend.Problems.Metadata;
using Backend.Services;

namespace Backend.Problems;

public abstract class ProblemCollection
{
    public abstract int Year { get; }
    public abstract List<ProblemSet> Problems { get; }
    
    public ProblemCollectionMetadata GetMetadata() => new(
        Year, 
        Problems.Select(p => p.GetMetadata()).ToList()
        );
}