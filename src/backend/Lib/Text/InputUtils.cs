using System.Text.RegularExpressions;

namespace Lib.Text;

public static partial class InputUtils
{
    /// <summary>
    /// Splits the input into groups separated by blank lines.
    /// </summary>
    /// <param name="lines">Input lines.</param>
    /// <returns>List of grouped lines.</returns>
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
    /// <param name="line">Input text.</param>
    /// <returns>Collection of parsed integers.</returns>
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
    /// <param name="input">String to split.</param>
    /// <param name="separators">Characters to split on.</param>
    /// <returns>Split and trimmed segments.</returns>
    public static IReadOnlyList<string> SplitAndTrim(string input, params char[] separators)
    {
        return input
            .Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    [GeneratedRegex("-?\\d+")]
    private static partial Regex NumberRegex();
}

