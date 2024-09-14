using Backend.Problems.Metadata;
using Backend.Services;

namespace Backend.Problems;

public abstract class ProblemSet
{
    public abstract DateTime ReleaseTime { get; }
    public abstract List<Problem> Problems { get; }

    public ProblemSetMetadata GetMetadata() =>
        new(
            ReleaseTime,
            Problems.Select(p => p.GetMetadata()).ToList()
        );
}