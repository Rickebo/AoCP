namespace Lib.Text;

public static class StringExtensions
{
    public static string[] SplitLines(this string str, 
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) =>
        str.SplitBy(["\r\n", "\n"], options);

    public static string[] SplitBy(this string str, string[] splits,
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) => 
        str.Split(splits, options);

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
