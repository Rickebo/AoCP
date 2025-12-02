using System.Numerics;

namespace Lib.Extensions;

public static class MathExtensions
{
    public const string HexadecimalCharacters = "0123456789ABCDEF";

    public static int Remainder(int x, int divisor) =>
        (x % divisor + divisor) % divisor;

    public static T Modulo<T>(T left, T right) where T : INumber<T>, IBinaryInteger<T>
    {
        var result = left % right;
        if ((result < T.Zero && right > T.Zero) || (result > T.Zero && right < T.Zero))
            result += right;

        return result;
    }
    
    /// <summary>
    /// Converts a hex value to a float value
    /// </summary>
    /// <param name="hex">The hexadecimal number to convert</param>
    /// <returns>A float ranging from 0 to 1</returns>
    public static float HexToFloat(string hex)
    {
        if (hex == null)
            throw new NullReferenceException(
                "Cannot convert hex to float: Input string is null."
            );
        if (hex.Length == 0)
            throw new ArgumentException(
                "Cannot convert hex to float: Input string is of an invalid length (0)."
            );

        foreach (char c in hex)
            if (!HexadecimalCharacters.Contains(char.ToUpper(c)))
                throw new ArgumentException(
                    "Cannot convert hex to float: Input string is not a hexadecimal number."
                );

        var val = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        var max = (int)Math.Pow(2, 4 * hex.Length) - 1;
        return ((float)val) / max;
    }

    public static T Ten<T>() where T : INumber<T> =>
        T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One;

    public static T CeilLog10<T>(T number) where T : INumber<T>, IComparisonOperators<T, T, bool>, IBinaryInteger<T>
    {
        var n = T.One;
        var i = T.Zero;
        var ten = Ten<T>();

        checked
        {
            while (n < number)
            {
                i++;
                n *= ten;
            }
        }

        return i;
    }

    public static T Pow10<T>(T power) where T : INumber<T>, IComparisonOperators<T, T, bool>
    {
        var n = T.One;
        var ten = Ten<T>();
        
        checked
        {
            for (var i = T.Zero; i < power; i++)
                n *= ten;
        }

        return n;
    }
}
