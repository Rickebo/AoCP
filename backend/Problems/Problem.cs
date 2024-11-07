using Backend.Problems.Metadata;
using Backend.Services;

namespace Backend.Problems;

public abstract class Problem
{
    public virtual string? Name { get; } = null;
    public virtual string? Description { get; } = null;

    public ProblemMetadata GetMetadata() => new(Name, Description);

    public abstract string Solve(string input);
}