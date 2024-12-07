using System.Runtime.CompilerServices;
using Backend.Problems.Metadata;
using Common;

namespace Backend.Problems;

public abstract class Problem
{
    public virtual string? Name => GetType().Name;

    public virtual string? Description { get; } = null;

    public ProblemMetadata GetMetadata() => new(Name, Description);

    public abstract Task Solve(string input, Reporter reporter);
}