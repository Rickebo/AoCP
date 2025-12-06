using System.Numerics;

namespace Lib.Math;

/// <summary>
/// General numeric helper methods.
/// </summary>
public static class MathExtensions
{
    /// <summary>
    /// Characters used in hexadecimal parsing/formatting.
    /// </summary>
    public const string HexadecimalCharacters = "0123456789ABCDEF";

    /// <summary>
    /// Computes a mathematical remainder that is always non-negative.
    /// </summary>
    public static int Remainder(int x, int divisor) =>
        (x % divisor + divisor) % divisor;

    /// <summary>
    /// Computes a modulus that respects the sign of the divisor.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="left">Dividend.</param>
    /// <param name="right">Divisor.</param>
    /// <returns>Result of modulo operation.</returns>
    /// <exception cref="DivideByZeroException">Thrown when <paramref name="right"/> is zero.</exception>
    public static T Modulo<T>(T left, T right) where T : INumber<T>, IBinaryInteger<T>
    {
        if (right == T.Zero)
            throw new DivideByZeroException("Modulo by zero is undefined.");

        var result = left % right;
        if ((result < T.Zero && right > T.Zero) || (result > T.Zero && right < T.Zero))
            result += right;

        return result;
    }
    
    /// <summary>
    /// Converts a hexadecimal string to a floating point value normalized to [0,1].
    /// </summary>
    /// <param name="hex">Hex string.</param>
    /// <returns>Value between 0 and 1.</returns>
    /// <exception cref="NullReferenceException">Thrown when input is null.</exception>
    /// <exception cref="ArgumentException">Thrown when input is empty or not hexadecimal.</exception>
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
    /// Convenience method returning the numeric value ten for any <see cref="INumber{TSelf}"/>.
    /// </summary>
    public static T Ten<T>() where T : INumber<T> =>
        T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One;

    /// <summary>
    /// Computes the ceiling of log base 10 for an integer-like value.
    /// </summary>
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
    /// Raises 10 to the specified power.
    /// </summary>
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

