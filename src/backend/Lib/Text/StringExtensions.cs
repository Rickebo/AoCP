namespace Lib.Text;

public static class StringExtensions
{
    public static string[] SplitLines(this string str, 
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) =>
        str.SplitBy(["\r\n", "\n"], options);

    public static string[] SplitBy(this string str, string[] splits,
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries) => 
        str.Split(splits, options);
}

