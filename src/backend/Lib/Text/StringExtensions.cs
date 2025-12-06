namespace Lib.Text;

public static class StringExtensions
{
    /// <summary>
    /// Splits a string into lines handling CRLF and LF separators.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <returns>Array of lines.</returns>
    public static string[] SplitLines(this string str, 
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) =>
        str.SplitBy(["\r\n", "\n"], options);

    /// <summary>
    /// Splits a string by the provided separators with trimming and empty-entry removal enabled by default.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splits">Delimiters to split on.</param>
    /// <param name="options">String split options.</param>
    /// <returns>Array of split segments.</returns>
    public static string[] SplitBy(this string str, string[] splits,
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) => 
        str.Split(splits, options);
}

