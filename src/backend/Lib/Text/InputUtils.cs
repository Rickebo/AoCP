using System.Text.RegularExpressions;

namespace Lib.Text;

public static partial class InputUtils
{
    public static IReadOnlyList<IReadOnlyList<string>> SplitOnBlankLines(IEnumerable<string> lines)
    {
        var groups = new List<List<string>> { new() };
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (groups[^1].Count > 0)
                    groups.Add(new());
                continue;
            }

            groups[^1].Add(line);
        }

        if (groups.Count > 0 && groups[^1].Count == 0)
            groups.RemoveAt(groups.Count - 1);

        return groups;
    }

    public static IReadOnlyList<long> ExtractIntegers(string line)
    {
        return NumberRegex()
            .Matches(line)
            .Select(match => long.Parse(match.Value))
            .ToArray();
    }

    public static IReadOnlyList<string> SplitAndTrim(string input, params char[] separators)
    {
        return input
            .Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    [GeneratedRegex("-?\\d+")]
    private static partial Regex NumberRegex();
}

