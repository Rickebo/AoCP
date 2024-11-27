using Backend.Problems.Metadata;

namespace Backend.Problems;

public abstract class Problem
{
    public virtual string? Name { get; } = null;
    public virtual string? Description { get; } = null;

    public ProblemMetadata GetMetadata() => new(Name, Description);

    public abstract Task Solve(string input, Reporter reporter);
}