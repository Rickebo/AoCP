namespace Lib;

public static class MathExtensions
{
    public const string HexadecimalCharacters = "0123456789ABCDEF";
    
    public static int Remainder(int x, int divisor) =>
        (x % divisor + divisor) % divisor;

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
            if (!HexadecimalCharacters.Contains(c))
                throw new ArgumentException(
                    "Cannot convert hex to float: Input string is not a hexadecimal number."
                );

        var val = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        var max = (int)Math.Pow(2, 4 * hex.Length) - 1;
        return ((float)val) / max;
    }
}