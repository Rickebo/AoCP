using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Backend.Problems.Metadata;
using Backend.Services;

namespace Backend.Problems;

public abstract class ProblemSet
{
    protected ProblemSet([CallerFilePath] string? sourceFilePath = null)
    {
        SolutionFilePath = sourceFilePath != null ? Path.GetFullPath(sourceFilePath) : null;
    }

    public abstract DateTime ReleaseTime { get; }
    public abstract List<Problem> Problems { get; }
    public abstract string Name { get; }
    public string Author { get; internal set; }
    public string? SolutionFilePath { get; internal set; }

    public ProblemSetMetadata GetMetadata() =>
        new(
            Name,
            Author,
            ReleaseTime,
            SolutionFilePath ?? FindFallbackPath(),
            Problems.Select(p => p.GetMetadata()).ToList()
        );

    private string? FindFallbackPath()
    {
        var root = ProblemSourceRegistry.GetRoot();
        if (root == null) return null;

        var candidate = Path.Combine(root, $"Year{ReleaseTime.Year}", Author, $"{GetType().Name}.cs");
        return File.Exists(candidate) ? candidate : null;
    }
}
