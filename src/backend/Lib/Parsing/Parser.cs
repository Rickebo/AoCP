using System.Globalization;
using System.Text.RegularExpressions;
using Lib.Enums;
using Lib.Extensions;
using Lib.Grid;

namespace Lib.Parsing;

public static partial class Parser
{
    [GeneratedRegex(@"\d+")]
    private static partial Regex RegexUnsigned();

    [GeneratedRegex(@"[+-]?\d+")]
    private static partial Regex RegexSigned();

    [GeneratedRegex(@"[+-]?\d+(\.\d+)?")]
    private static partial Regex RegexDecimalsDot();

    [GeneratedRegex(@"[+-]?\d+(\,\d+)?")]
    private static partial Regex RegexDecimalsComma();

    public static T[] GetValues<T>(string str, string decimalSeparator = ".")
    {
        // Guard decimal separator
        if ((typeof(T) == typeof(double) || typeof(T) == typeof(decimal)) && decimalSeparator != "." && decimalSeparator != ",")
            throw new NotSupportedException("Unsupported decimal separator.");

        // Choose regex pattern
        MatchCollection matches;
        if (typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
            matches = RegexUnsigned().Matches(str);
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
            matches = RegexSigned().Matches(str);
        else if (typeof(T) == typeof(double) || typeof(T) == typeof(decimal))
            matches = decimalSeparator == "." ? RegexDecimalsDot().Matches(str) : RegexDecimalsComma().Matches(str);
        else
            throw new NotSupportedException("Unsupported generic type.");

        // Parse values
        T[]? numbers = null;
        NumberFormatInfo nfi = new()
        {
            NumberDecimalSeparator = decimalSeparator
        };
        if (typeof(T) == typeof(int))
            numbers = matches.Select(x => int.Parse(x.Value)).ToArray() as T[];
        else if (typeof(T) == typeof(uint))
            numbers = matches.Select(x => uint.Parse(x.Value)).ToArray() as T[];
        else if (typeof(T) == typeof(long))
            numbers = matches.Select(x => long.Parse(x.Value)).ToArray() as T[];
        else if (typeof(T) == typeof(ulong))
            numbers = matches.Select(x => ulong.Parse(x.Value)).ToArray() as T[];
        else if (typeof(T) == typeof(double))
            numbers = matches.Select(x => double.Parse(x.Value, nfi)).ToArray() as T[];
        else if (typeof(T) == typeof(decimal))
            numbers = matches.Select(x => decimal.Parse(x.Value, nfi)).ToArray() as T[];

        return numbers ?? [];
    }

    public static T[] GetValues<T>(string[] str)
    {
        List<T> values = [];
        foreach (var entry in str)
            values.AddRange(GetValues<T>(entry));

        return [.. values];
    }

    public static T[][] GetValueArrays<T>(string[] str)
    {
        List<T[]> values = [];
        foreach (var entry in str)
            values.Add(GetValues<T>(entry));

        return [.. values];
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