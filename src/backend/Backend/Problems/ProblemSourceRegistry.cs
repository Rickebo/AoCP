using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Backend.Problems;

internal static class ProblemSourceRegistry
{
    private static readonly ConcurrentDictionary<Assembly, string> Paths = new();
    private static string? _rootPath;

    public static void SetRoot(string rootPath)
    {
        _rootPath = rootPath;
    }

    public static string? GetRoot() => _rootPath;

    public static void SetPath(Assembly assembly, string path)
    {
        if (assembly == null || string.IsNullOrWhiteSpace(path)) return;
        Paths[assembly] = path;
    }

    public static bool TryGetPath(Assembly assembly, out string path)
    {
        if (assembly == null)
        {
            path = string.Empty;
            return false;
        }

        return Paths.TryGetValue(assembly, out path);
    }
}
