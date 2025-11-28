using System.Text.RegularExpressions;
using Lib.Grid;

namespace Lib;

public static partial class Parser
{
    [GeneratedRegex(@"\d+")]
    private static partial Regex RegexUnsigned();

    [GeneratedRegex(@"[+-]?\d+")]
    private static partial Regex RegexSigned();

    [GeneratedRegex(@"[+-]?\d+(\.\d+)?")]
    private static partial Regex RegexDouble();

    public static T[] GetValues<T>(string str)
    {
        MatchCollection matches;

        if (typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
            matches = RegexUnsigned().Matches(str);
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
            matches = RegexSigned().Matches(str);
        else if (typeof(T) == typeof(double))
            matches = RegexDouble().Matches(str);
        else
            throw new Exception("Unsupported generic type.");

        T[]? numbers = null;
        if (typeof(T) == typeof(int))
            numbers = matches.Select(x => int.Parse(x.Value)).ToArray() as T[];
        if (typeof(T) == typeof(uint))
            numbers = matches.Select(x => uint.Parse(x.Value)).ToArray() as T[];
        if (typeof(T) == typeof(long))
            numbers = matches.Select(x => long.Parse(x.Value)).ToArray() as T[];
        if (typeof(T) == typeof(ulong))
            numbers = matches.Select(x => ulong.Parse(x.Value)).ToArray() as T[];
        if (typeof(T) == typeof(double))
            numbers = matches.Select(x => double.Parse(x.Value)).ToArray() as T[];

        return numbers ?? [];
    }

    public static T[] StringsToValues<T>(string[] str)
    {
        T[]? numbers = null;
        if (typeof(T) == typeof(int))
            numbers = str.Select(x => int.Parse(x.Trim())).ToArray() as T[];
        if (typeof(T) == typeof(uint))
            numbers = str.Select(x => uint.Parse(x.Trim())).ToArray() as T[];
        if (typeof(T) == typeof(long))
            numbers = str.Select(x => long.Parse(x.Trim())).ToArray() as T[];
        if (typeof(T) == typeof(ulong))
            numbers = str.Select(x => ulong.Parse(x.Trim())).ToArray() as T[];
        if (typeof(T) == typeof(double))
            numbers = str.Select(x => double.Parse(x.Trim())).ToArray() as T[];

        return numbers ?? [];
    }

    public static ArrayGrid<Direction> ParseDirectionGrid(string text)
    {
        var lines = SplitLines(text);
        var width = lines[0].Length;
        var height = lines.Length;
        var grid = new ArrayGrid<Direction>(width, height);

        for (var y = 0; y < height; y++)
        {
            var yIndex = new Index(y, fromEnd: true);
            var line = lines[yIndex];
            for (var x = 0; x < width; x++)
                grid[x, y] = DirectionExtensions.Parse(line[x]);
        }

        return grid;
    }

    public static string[] SplitLines(string str) => SplitBy(str, ["\r\n", "\n"]);

    public static string[] SplitBy(string str, string[] splits) => str
        .Split(
            splits,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );
}