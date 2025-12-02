using System.Text.RegularExpressions;

namespace Lib.Text;

public static partial class InputUtils
{
    /// <summary>
    /// Splits the input into groups separated by blank lines.
    /// </summary>
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

    /// <summary>
    /// Returns all signed integers found in the string.
    /// </summary>
    public static IReadOnlyList<long> ExtractIntegers(string line)
    {
        return NumberRegex()
            .Matches(line)
            .Select(match => long.Parse(match.Value))
            .ToArray();
    }

    /// <summary>
    /// Splits the string using the provided separators, trimming whitespace and discarding empties.
    /// </summary>
    public static IReadOnlyList<string> SplitAndTrim(string input, params char[] separators)
    {
        return input
            .Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    [GeneratedRegex("-?\\d+")]
    private static partial Regex NumberRegex();
}
