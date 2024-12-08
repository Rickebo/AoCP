using System.Reflection;
using Backend.Problems.Metadata;
using Backend.Services;

namespace Backend.Problems;

public abstract class ProblemCollection
{
    public abstract int Year { get; }
    public abstract Dictionary<string, List<ProblemSet>> Problems { get; }

    public ProblemCollectionMetadata GetMetadata() => new(
        Year,
        Problems.ToDictionary(
            p => p.Key,
            p => p.Value.Select(ps => ps.GetMetadata()).ToList()
        )
    );

    public static Dictionary<string, List<ProblemSet>> FindProblems(Type ns)
    {
        var rootNs = ns.Namespace ?? throw new Exception(
            "Failed to find namespace of specified type."
        );

        var classes = Assembly.GetExecutingAssembly().GetTypes().Where(
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


        var result = new Dictionary<string, List<ProblemSet>>();
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

            if (!result.TryGetValue(inst.Author, out var probList))
                probList = result[inst.Author] = [];
            
            probList.Add(inst);
        }
        
        foreach (var probList in result.Values) 
            probList.Sort((a, b) => a.ReleaseTime.CompareTo(b.ReleaseTime));

        return result;
    }
}