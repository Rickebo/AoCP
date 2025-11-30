using Lib.Parsing;

namespace Lib.Extensions;

public static class StringExtensions
{
    public static string[] SplitLines(this string str) => 
        Parser.SplitLines(str);
}