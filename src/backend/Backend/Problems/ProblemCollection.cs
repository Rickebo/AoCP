using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Backend.Problems.Metadata;
using Backend.Services;

namespace Backend.Problems;

public abstract class ProblemCollection
{
    public abstract int Year { get; }
    public abstract string Source { get; }
    public abstract Type ProblemRootType { get; }
    public Dictionary<string, List<ProblemSet>> Problems { get; } = new();

    public EventHandler? OnUpdate = null;

    public ProblemCollection()
    {
        FindProblems(Assembly.GetExecutingAssembly());
    }
    
    public ProblemCollectionMetadata GetMetadata() => new(
        Source,
        Year,
        Problems.ToDictionary(
            p => p.Key,
            p => p.Value.Select(ps => ps.GetMetadata()).ToList()
        )
    );
    
    public void FindProblems(
        Assembly? assembly = null
    )
    {
        var rootNs = ProblemRootType.Namespace ?? throw new Exception(
            "Failed to find namespace of specified type."
        );

        
        var classes = (assembly ?? Assembly.GetExecutingAssembly())
            .GetTypes()
            .Where(
                t => t.IsClass &&
                     (t.Namespace?.StartsWith(rootNs) ?? false) &&
                     t.IsAssignableTo(typeof(ProblemSet))
            );

        var instances = classes
            .Select(
                c => (ProblemSet)
                    (Activator.CreateInstance(c) ??
                     throw new Exception(
                         "Failed to instantiate problem set of type: " + c
                     ))
            )
            .ToList();


        foreach (var inst in instances)
        {
            var instType = inst.GetType();
            if (instType.Namespace!.Length > rootNs.Length)
            {
                var rest = instType.Namespace[rootNs.Length..];
                inst.Author = rest
                    .Split(
                        '.',
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries
                    )
                    .First();
            }
            else
            {
                inst.Author = "Unknown";
            }

            if (!Problems.TryGetValue(inst.Author, out var probList))
                probList = Problems[inst.Author] = [];

            var added = false;
            for (var i = 0; i < probList.Count; i++)
            {
                if (probList[i].ReleaseTime != inst.ReleaseTime)
                    continue;
                
                probList[i] = inst;
                added = true;
            }

            if (!added)
                probList.Add(inst);
        }

        foreach (var probList in Problems.Values)
            probList.Sort((a, b) => a.ReleaseTime.CompareTo(b.ReleaseTime));
        
        OnUpdate?.Invoke(this, EventArgs.Empty);
    }
}
