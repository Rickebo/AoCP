using Backend.Problems.Metadata;
using Backend.Services;

namespace Backend.Problems;

public abstract class Problem
{
    public abstract string Name { get; }

    public ProblemMetadata GetMetadata() => new(Name);
}