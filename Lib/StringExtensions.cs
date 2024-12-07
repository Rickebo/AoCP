namespace Lib;

public static class StringExtensions
{
    public static string[] SplitLines(this string str) => 
        Parser.SplitLines(str);
}