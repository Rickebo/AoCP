namespace Lib.Text;

/// <summary>
/// String utilities tailored for puzzle input parsing.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Splits a string into lines handling different newline styles.
    /// </summary>
    public static string[] SplitLines(this string str, 
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) =>
        str.SplitBy(["\r\n", "\n"], options);

    /// <summary>
    /// Splits a string by any of the provided separators.
    /// </summary>
    public static string[] SplitBy(this string str, string[] splits,
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) => 
        str.Split(splits, options);

    /// <summary>
    /// Splits text into groups of lines separated by blank lines or a custom predicate.
    /// </summary>
    /// <param name="input">Input text.</param>
    /// <param name="isGroupSeparator">Optional predicate that marks group separation.</param>
    /// <returns>Grouped lines.</returns>
    public static string[][] SplitToLineGroups(
        this string input,
        Func<string, bool>? isGroupSeparator = null)
    {
        var shouldSplit = isGroupSeparator ?? string.IsNullOrWhiteSpace;
        var groups = new List<List<string>> { new() };

        foreach (var line in input.SplitLines(StringSplitOptions.None))
        {
            if (shouldSplit(line))
            {
                if (groups[^1].Count > 0)
                    groups.Add([]);
                continue;
            }

            groups[^1].Add(line);
        }

        if (groups.Count > 0 && groups[^1].Count == 0)
            groups.RemoveAt(groups.Count - 1);

        return [.. groups.Select(g => g.ToArray())];
    }
}
