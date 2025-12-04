using System.Globalization;
using System.Text.RegularExpressions;
using Lib.Geometry;
using Lib.Grids;

namespace Lib.Text;

/// <summary>
/// Helper methods for extracting numeric values and parsing grid-based inputs.
/// </summary>
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

    /// <summary>
    /// Extracts numeric values from a string using the requested decimal separator.
    /// </summary>
    /// <typeparam name="T">Numeric type to parse.</typeparam>
    /// <param name="str">Input string.</param>
    /// <param name="decimalSeparator">Decimal separator for floating-point parsing.</param>
    /// <returns>Array of parsed numbers.</returns>
    /// <exception cref="NotSupportedException">Thrown for unsupported types or separators.</exception>
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

    /// <summary>
    /// Extracts numeric values from multiple strings.
    /// </summary>
    /// <typeparam name="T">Numeric type to parse.</typeparam>
    /// <param name="str">Array of strings to search.</param>
    /// <returns>All numbers found across the input strings.</returns>
    public static T[] GetValues<T>(string[] str)
    {
        List<T> values = [];
        foreach (var entry in str)
            values.AddRange(GetValues<T>(entry));

        return [.. values];
    }

    /// <summary>
    /// Extracts numeric values from each input string and groups them into arrays.
    /// </summary>
    /// <typeparam name="T">Numeric type to parse.</typeparam>
    /// <param name="str">Array of strings.</param>
    /// <returns>Array of parsed number arrays corresponding to each input string.</returns>
    public static T[][] GetValueArrays<T>(string[] str)
    {
        List<T[]> values = [];
        foreach (var entry in str)
            values.Add(GetValues<T>(entry));

        return [.. values];
    }

    /// <summary>
    /// Parses a grid of directions from a multiline string.
    /// </summary>
    /// <param name="text">Input text containing direction characters.</param>
    /// <returns>A grid of <see cref="Direction"/> values.</returns>
    public static ArrayGrid<Direction> ParseDirectionGrid(string text)
    {
        var lines = text.SplitLines();
        var width = lines[0].Length;
        var height = lines.Length;
        var grid = new ArrayGrid<Direction>(width, height);

        for (var y = 0; y < height; y++)
        {
            var line = lines[^(y + 1)];
            for (var x = 0; x < width; x++)
                grid[x, y] = DirectionExtensions.Parse(line[x]);
        }

        return grid;
    }
}

