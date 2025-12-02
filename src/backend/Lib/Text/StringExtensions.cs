namespace Lib.Extensions;

public static class StringExtensions
{
    public static string[] SplitLines(this string str) =>
        str.SplitBy(["\r\n", "\n"]);

    public static string[] SplitBy(this string str, string[] splits,
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        ) => str.Split(splits, options);
}
