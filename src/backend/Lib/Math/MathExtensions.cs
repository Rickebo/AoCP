using System.Numerics;

namespace Lib.Math;

public static class MathExtensions
{
    /// <summary>
    /// Characters considered valid when parsing hexadecimal values.
    /// </summary>
    public const string HexadecimalCharacters = "0123456789ABCDEF";

    /// <summary>
    /// Returns the mathematical remainder that is always non-negative.
    /// </summary>
    /// <param name="x">Dividend.</param>
    /// <param name="divisor">Divisor.</param>
    /// <returns>Non-negative remainder.</returns>
    public static int Remainder(int x, int divisor) =>
        (x % divisor + divisor) % divisor;

    /// <summary>
    /// Computes the modulo of two numbers with the sign of the divisor.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="left">Dividend.</param>
    /// <param name="right">Divisor.</param>
    /// <returns>Modulo result matching the divisor's sign.</returns>
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
        var max = (int)System.Math.Pow(2, 4 * hex.Length) - 1;
        return ((float)val) / max;
    }

    /// <summary>
    /// Returns the value ten as the specified numeric type.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <returns>The value ten represented as <typeparamref name="T"/>.</returns>
    public static T Ten<T>() where T : INumber<T> =>
        T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One;

    /// <summary>
    /// Computes the ceiling of log10 for an integer type.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="number">Number whose log10 ceiling to compute.</param>
    /// <returns>Smallest integer exponent such that 10^exponent &gt;= number.</returns>
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

    /// <summary>
    /// Raises 10 to the given power.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="power">Exponent to raise 10 by.</param>
    /// <returns>10 raised to <paramref name="power"/>.</returns>
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

