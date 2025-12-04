using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lib.Grids;
using Lib.Math;

namespace Lib.Color;

public class ColorJsonConverter : JsonConverter<Color>
{
    /// <summary>
    /// Reads a color value from a JSON string using <see cref="Color.Parse(string)"/>.
    /// </summary>
    /// <param name="reader">JSON reader.</param>
    /// <param name="typeToConvert">Type being converted.</param>
    /// <param name="options">Serializer options.</param>
    /// <returns>Parsed <see cref="Color"/> value.</returns>
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        Color.Parse(reader.GetString() ?? "#000000FF");

    /// <summary>
    /// Writes the color as a hexadecimal RGBA string.
    /// </summary>
    /// <param name="writer">JSON writer.</param>
    /// <param name="value">Color to write.</param>
    /// <param name="options">Serializer options.</param>
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

/// <summary>
/// Represents a 32-bit RGBA color with helper operations.
/// </summary>
public readonly struct Color(uint value) : IEquatable<Color>
{
    private static readonly int BaseHashCode = typeof(Color).GetHashCode();

    /// <summary>
    /// Gets an opaque black color.
    /// </summary>
    public static readonly Color Black = new(0x000000FF);

    /// <summary>
    /// Gets an opaque white color.
    /// </summary>
    public static readonly Color White = new(0xFFFFFFFF);

    /// <summary>
    /// Gets a fully transparent black color.
    /// </summary>
    public static readonly Color TransparentBlack = new(0x00000000);

    /// <summary>
    /// Gets a fully transparent white color.
    /// </summary>
    public static readonly Color TransparentWhite = new(0xFFFFFF00);

    /// <summary>
    /// Gets the packed RGBA value.
    /// </summary>
    public uint Value => value;

    /// <summary>
    /// Bitmask for the red component within the packed value.
    /// </summary>
    public const uint RedMask = 0xFFu << RedShift;

    /// <summary>
    /// Bitmask for the green component within the packed value.
    /// </summary>
    public const uint GreenMask = 0xFFu << GreenShift;

    /// <summary>
    /// Bitmask for the blue component within the packed value.
    /// </summary>
    public const uint BlueMask = 0xFFu << BlueShift;

    /// <summary>
    /// Bitmask for the alpha component within the packed value.
    /// </summary>
    public const uint AlphaMask = 0xFFu << AlphaShift;

    /// <summary>
    /// Bit shift for the red component.
    /// </summary>
    public const int RedShift = 8 * 3;

    /// <summary>
    /// Bit shift for the green component.
    /// </summary>
    public const int GreenShift = 8 * 2;

    /// <summary>
    /// Bit shift for the blue component.
    /// </summary>
    public const int BlueShift = 8 * 1;

    /// <summary>
    /// Bit shift for the alpha component.
    /// </summary>
    public const int AlphaShift = 8 * 0;

    /// <summary>
    /// Gets the red byte component (0-255).
    /// </summary>
    public uint R => (Value & RedMask) >> RedShift;

    /// <summary>
    /// Gets the green byte component (0-255).
    /// </summary>
    public uint G => (Value & GreenMask) >> GreenShift;

    /// <summary>
    /// Gets the blue byte component (0-255).
    /// </summary>
    public uint B => (Value & BlueMask) >> BlueShift;

    /// <summary>
    /// Gets the alpha byte component (0-255).
    /// </summary>
    public uint A => (Value & AlphaMask) >> AlphaShift;

    /// <summary>
    /// Gets the red component normalized to the range [0,1].
    /// </summary>
    public double Red => R / 255d;

    /// <summary>
    /// Gets the green component normalized to the range [0,1].
    /// </summary>
    public double Green => G / 255d;

    /// <summary>
    /// Gets the blue component normalized to the range [0,1].
    /// </summary>
    public double Blue => B / 255d;

    /// <summary>
    /// Gets the alpha component normalized to the range [0,1].
    /// </summary>
    public double Alpha => A / 255d;

    /// <summary>
    /// Creates a color from a packed RGBA value.
    /// </summary>
    /// <param name="rgba">Packed color value in RGBA order.</param>
    /// <returns>A new <see cref="Color"/> instance.</returns>
    public static Color FromRgba(uint rgba) => new(rgba);

    /// <summary>
    /// Creates a color from a packed ARGB value.
    /// </summary>
    /// <param name="argb">Packed color value in ARGB order.</param>
    /// <returns>A new <see cref="Color"/> instance.</returns>
    public static Color FromArgb(uint argb) => new(argb << 8 | argb >> 24);

    /// <summary>
    /// Converts a normalized component value to a byte.
    /// </summary>
    /// <param name="v">Component value in the range [0,1].</param>
    /// <returns>Converted byte or <c>null</c> when <paramref name="v"/> is <c>null</c>.</returns>
    public static byte? FromRange(double? v) => v != null ? (byte)(v.Value.Clamp(0, 1) * 255) : null;

    /// <summary>
    /// Parses a single hexadecimal nibble.
    /// </summary>
    /// <param name="ch">Character to parse.</param>
    /// <returns>Numeric value of the nibble.</returns>
    /// <exception cref="ArgumentException">Thrown when the character is not hexadecimal.</exception>
    public static uint ParseNibble(char ch)
    {
        if (char.IsDigit(ch))
            return (uint)(ch - '0');

        ch = char.ToLower(ch);
        var ordinal = 10u + ch - 'a';
        if (ordinal is < 10 or > 15)
            throw new ArgumentException("Cannot parse hexadecimal character '" + ch + "'");

        return ordinal;
    }

    /// <summary>
    /// Parse a Color from a hex string on the format of "#RRGGBBAA", "#RRGGBB", "#RGBA", "#RGB". 
    /// If components are specified as a single digit, such as for "#RGB", then each value is shifted 
    /// to represent the corresponding components most significant bits. Alpha component defaults to 255. 
    /// </summary>
    /// <param name="text">The text to parse as a color</param>
    /// <returns>The parsed color</returns>
    public static Color Parse(string text)
    {
        // # optional
        if (text.StartsWith('#'))
            text = text[1..];

        // Retrieve nibbles
        var nibbles = text.Select(ParseNibble).ToArray();

        // Short forms: #RGB or #RGBA
        if (nibbles.Length is 3 or 4)
        {
            // Create components
            var shortR = nibbles[0] << 4;
            var shortG = nibbles[1] << 4;
            var shortB = nibbles[2] << 4;
            var shortA = nibbles.Length == 4 ? nibbles[3] << 4 : 255;

            return new(shortR << RedShift | shortG << GreenShift | shortB << BlueShift | shortA << AlphaShift);
        }

        // Long forms: #RRGGBB or #RRGGBBAA
        if (nibbles.Length is not 6 and not 8)
            throw new ArgumentException($"Hex string {text} must be 3, 4, 6, or 8 hex digits long.");

        // Combine nibbles into bytes correctly: (high << 4) | low
        var longR = (nibbles[0] << 4) | nibbles[1];
        var longG = (nibbles[2] << 4) | nibbles[3];
        var longB = (nibbles[4] << 4) | nibbles[5];
        var longA = nibbles.Length == 8 ? (nibbles[6] << 4) | nibbles[7] : 255;

        return new(longR << RedShift | longG << GreenShift | longB << BlueShift | longA << AlphaShift);
    }

    /// <summary>
    /// Creates a color from optional byte or normalized component values.
    /// </summary>
    /// <param name="r">Red byte component.</param>
    /// <param name="g">Green byte component.</param>
    /// <param name="b">Blue byte component.</param>
    /// <param name="a">Alpha byte component.</param>
    /// <param name="red">Normalized red component.</param>
    /// <param name="green">Normalized green component.</param>
    /// <param name="blue">Normalized blue component.</param>
    /// <param name="alpha">Normalized alpha component.</param>
    /// <returns>New <see cref="Color"/> with provided components.</returns>
    public static Color From(
        byte? r = null,
        byte? g = null,
        byte? b = null,
        byte? a = null,
        double? red = null,
        double? green = null,
        double? blue = null,
        double? alpha = null
    )
    {
        var rv = (uint)(r ?? FromRange(red) ?? 0x00);
        var gv = (uint)(g ?? FromRange(green) ?? 0x00);
        var bv = (uint)(b ?? FromRange(blue) ?? 0x00);
        var av = (uint)(a ?? FromRange(alpha) ?? 0xFF);

        return new(rv << RedShift | gv << GreenShift | bv << BlueShift | av << AlphaShift);
    }

    /// <summary>
    /// Returns a copy of this color with overridden byte or normalized component values.
    /// </summary>
    /// <param name="r">Override red byte component.</param>
    /// <param name="g">Override green byte component.</param>
    /// <param name="b">Override blue byte component.</param>
    /// <param name="a">Override alpha byte component.</param>
    /// <param name="red">Override normalized red component.</param>
    /// <param name="green">Override normalized green component.</param>
    /// <param name="blue">Override normalized blue component.</param>
    /// <param name="alpha">Override normalized alpha component.</param>
    /// <returns>New <see cref="Color"/> with updated components.</returns>
    public Color With(
        byte? r = null,
        byte? g = null,
        byte? b = null,
        byte? a = null,
        double? red = null,
        double? green = null,
        double? blue = null,
        double? alpha = null
    ) => new(
        ((r ?? FromRange(red) ?? R) << RedShift) |
        ((g ?? FromRange(green) ?? G) << GreenShift) |
        ((b ?? FromRange(blue) ?? B) << BlueShift) |
        ((a ?? FromRange(alpha) ?? A) << AlphaShift)
    );

    /// <summary>
    /// Multiplies normalized components by the provided factors.
    /// </summary>
    /// <param name="red">Red multiplier.</param>
    /// <param name="green">Green multiplier.</param>
    /// <param name="blue">Blue multiplier.</param>
    /// <param name="alpha">Alpha multiplier.</param>
    /// <returns>New <see cref="Color"/> with scaled components.</returns>
    public Color Multiply(double? red = null, double? green = null, double? blue = null, double? alpha = null) =>
        With(
            red: red != null ? Red * red.Value : null,
            green: green != null ? Green * green.Value : null,
            blue: blue != null ? Blue * blue.Value : null,
            alpha: alpha != null ? Alpha * alpha.Value : null
        );

    /// <summary>
    /// Adds offsets to normalized components.
    /// </summary>
    /// <param name="red">Red offset.</param>
    /// <param name="green">Green offset.</param>
    /// <param name="blue">Blue offset.</param>
    /// <param name="alpha">Alpha offset.</param>
    /// <returns>New <see cref="Color"/> with adjusted components.</returns>
    public Color Add(double? red = null, double? green = null, double? blue = null, double? alpha = null) =>
        With(
            red: red != null ? Red + red.Value : null,
            green: green != null ? Green + green.Value : null,
            blue: blue != null ? Blue + blue.Value : null,
            alpha: alpha != null ? Alpha + alpha.Value : null
        );

    /// <summary>
    /// Helper for converting HSL values to RGB components.
    /// </summary>
    /// <param name="p">Lower bound component.</param>
    /// <param name="q">Upper bound component.</param>
    /// <param name="h">Hue value.</param>
    /// <returns>Computed color channel.</returns>
    private static double GetHue(double p, double q, double h)
    {
        if (h < 0) h++;
        if (h > 1) h--;

        return h switch
        {
            < 1.0 / 6 => p + (q - p) * 6 * h,
            < 1.0 / 2 => q,
            < 2.0 / 3 => p + (q - p) * (2.0 / 3 - h) * 6,
            _ => p
        };
    }

    /// <summary>
    /// Creates a color from HSL components in the range [0,1].
    /// </summary>
    /// <param name="h">Hue.</param>
    /// <param name="s">Saturation.</param>
    /// <param name="l">Lightness.</param>
    /// <returns>New <see cref="Color"/> corresponding to the HSL values.</returns>
    public static Color FromHsl(double h, double s, double l)
    {
        double r, g, b;

        var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
        var p = 2 * l - q;

        if (l == 0)
        {
            r = 0;
            g = 0;
            b = 0;
        }
        else if (s != 0)
        {
            r = GetHue(p, q, h + 1.0 / 3);
            g = GetHue(p, q, h);
            b = GetHue(p, q, h - 1.0 / 3);
        }
        else
        {
            r = l;
            g = l;
            b = l;
        }

        return From(red: r, green: g, blue: b);
    }

    /// <summary>
    /// Creates a color from HSV components in the range [0,1].
    /// </summary>
    /// <param name="h">Hue.</param>
    /// <param name="s">Saturation.</param>
    /// <param name="v">Value component.</param>
    /// <returns>New <see cref="Color"/> corresponding to the HSV values.</returns>
    public static Color FromHsv(double h, double s, double v)
    {
        var l = v * (1 - s / 2);

        return FromHsl(h, l is 0 or 1 ? 0 : (v - l) / System.Math.Min(l, 1 - l), l);
    }

    /// <summary>
    /// Generates a sequence of distinct colors using a random starting hue.
    /// </summary>
    /// <param name="count">Number of colors to generate.</param>
    /// <param name="saturation">Saturation component for each color.</param>
    /// <param name="vibrancy">Value component for each color.</param>
    /// <returns>Sequence of colors.</returns>
    public static IEnumerable<Color> Generate(int count, double saturation = 0.5, double vibrancy = 0.9) =>
        Generate(Random.Shared.NextDouble(), count, saturation, vibrancy);

    /// <summary>
    /// Generates a sequence of distinct colors using the golden ratio for hue spacing.
    /// </summary>
    /// <param name="seed">Starting hue seed.</param>
    /// <param name="count">Number of colors to generate.</param>
    /// <param name="saturation">Saturation component for each color.</param>
    /// <param name="vibrancy">Value component for each color.</param>
    /// <returns>Sequence of colors.</returns>
    public static IEnumerable<Color> Generate(double seed, int count, double saturation = 0.5, double vibrancy = 0.9)
    {
        var goldenRatio = (1 + System.Math.Sqrt(5)) / 2 - 1;
        var h = seed;
        for (var i = 0; i < count; i++)
        {
            h = (h + goldenRatio) % 1;
            yield return FromHsv(h, saturation, vibrancy);
        }
    }

    /// <summary>
    /// Converts a numeric grid into a color heatmap between two colors.
    /// </summary>
    /// <typeparam name="TCell">Numeric cell type.</typeparam>
    /// <param name="sourceGrid">Grid containing numeric values.</param>
    /// <param name="from">Color representing the minimum value.</param>
    /// <param name="to">Color representing the maximum value.</param>
    /// <param name="minClamp">Optional minimum clamp value.</param>
    /// <param name="maxClamp">Optional maximum clamp value.</param>
    /// <returns>A grid of colors interpolated between <paramref name="from"/> and <paramref name="to"/>.</returns>
    public static ArrayGrid<Color> Heatmap<TCell>(
        ArrayGrid<TCell> sourceGrid,
        Color from,
        Color to,
        TCell? minClamp = null,
        TCell? maxClamp = null
    ) where TCell : struct, INumber<TCell>, IMinMaxValue<TCell>
    {
        // Check if clamping has been provided
        bool clampMin = minClamp != null;
        bool clampMax = maxClamp != null;

        // Find out global min and max values if necessary
        if (!clampMin || !clampMax)
        {
            foreach (var coord in sourceGrid.Coordinates)
            {
                TCell value = sourceGrid[coord];
                if (!clampMin && (minClamp == null || value < minClamp))
                    minClamp = value;
                if (!clampMax && (maxClamp == null || value > maxClamp))
                    maxClamp = value;
            }
        }

        // Create clamping min max
        TCell min = minClamp ?? TCell.MinValue;
        TCell max = maxClamp ?? TCell.MaxValue;

        // Create color grid
        ArrayGrid<Color> colorGrid = new(sourceGrid.Width, sourceGrid.Height);

        // Create range
        double range = double.CreateSaturating(max - min);

        // Interpolate each cell value
        foreach (var coord in sourceGrid.Coordinates)
        {
            // Clamp
            TCell clamped = sourceGrid[coord].Clamp(min, max);

            // Interpolate
            double percent = double.CreateSaturating(clamped - min) / range;
            colorGrid[coord.X, coord.Y] = Between(from, to, percent);
        }

        return colorGrid;
    }

    /// <summary>
    /// Linearly interpolates between two colors by the given percentage.
    /// </summary>
    /// <param name="from">Start color.</param>
    /// <param name="to">End color.</param>
    /// <param name="percent">Interpolation factor in the range [0,1].</param>
    /// <returns>Interpolated color.</returns>
    public static Color Between(Color from, Color to, double percent)
    {
        // Clamp percent between 0 and 1
        percent = percent.Clamp(0.0, 1.0);

        // Interpolate R G B A values
        uint r = (uint)System.Math.Round(from.R + to.R * percent - from.R * percent);
        uint g = (uint)System.Math.Round(from.G + to.G * percent - from.G * percent);
        uint b = (uint)System.Math.Round(from.B + to.B * percent - from.B * percent);
        uint a = (uint)System.Math.Round(from.A + to.A * percent - from.A * percent);

        return new(r << RedShift | g << GreenShift | b << BlueShift | a << AlphaShift);
    }

    /// <summary>
    /// Linearly interpolates between two colors based on a value within a numeric range.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="from">Start color.</param>
    /// <param name="to">End color.</param>
    /// <param name="curr">Current value.</param>
    /// <param name="min">Minimum bound.</param>
    /// <param name="max">Maximum bound.</param>
    /// <returns>Interpolated color corresponding to <paramref name="curr"/>.</returns>
    public static Color Between<T>(Color from, Color to, T curr, T min, T max) where T : INumber<T>
    {
        var currD = double.CreateSaturating(curr);
        var minD = double.CreateSaturating(min);
        var maxD = double.CreateSaturating(max);
        return Between(from, to, (currD - minD) / (maxD - minD));
    }

    /// <summary>
    /// Returns the RGBA string representation of the color.
    /// </summary>
    /// <returns>String in <c>#RRGGBBAA</c> format.</returns>
    public new string ToString() => ToRgbaString();

    /// <summary>
    /// Returns the RGBA string representation of the color.
    /// </summary>
    /// <returns>String in <c>#RRGGBBAA</c> format.</returns>
    public string ToRgbaString() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

    /// <summary>
    /// Returns the RGB string representation of the color, excluding alpha.
    /// </summary>
    /// <returns>String in <c>#RRGGBB</c> format.</returns>
    public string ToRgbString() => $"#{R:X2}{G:X2}{B:X2}";

    /// <summary>
    /// Returns the ARGB string representation of the color.
    /// </summary>
    /// <returns>String in <c>#AARRGGBB</c> format.</returns>
    public string ToArgbString() => $"#{A:X2}{R:X2}{G:X2}{B:X2}";

    /// <summary>
    /// Determines equality with another object based on the packed color value.
    /// </summary>
    /// <param name="other">Object to compare.</param>
    /// <returns><c>true</c> when the object represents the same color.</returns>
    public override bool Equals(object? other) => other is Color color && color.Value == Value;

    /// <summary>
    /// Determines equality with another color based on the packed value.
    /// </summary>
    /// <param name="other">Color to compare.</param>
    /// <returns><c>true</c> when the colors are identical.</returns>
    public bool Equals(Color other) => other.Value == Value;

    /// <summary>
    /// Generates a hash code for the color.
    /// </summary>
    /// <returns>Hash code value.</returns>
    public override int GetHashCode() => BaseHashCode ^ Value.GetHashCode();

    /// <summary>
    /// Compares two colors for equality.
    /// </summary>
    public static bool operator ==(Color left, Color right) => left.Value == right.Value;

    /// <summary>
    /// Compares two colors for inequality.
    /// </summary>
    public static bool operator !=(Color left, Color right) => left.Value != right.Value;

    /// <summary>
    /// Applies a binary operation component-wise between two colors.
    /// </summary>
    /// <param name="left">Left color.</param>
    /// <param name="right">Right color.</param>
    /// <param name="op">Operator to apply.</param>
    /// <returns>Resulting color.</returns>
    private static Color ApplyOperator(Color left, Color right, Func<double, double, double> op) =>
        left.With(
            red: op(left.Red, right.Red),
            green: op(left.Green, right.Green),
            blue: op(left.Blue, right.Blue),
            alpha: op(left.Alpha, right.Alpha)
        );

    /// <summary>
    /// Applies a binary operation between a color and a scalar across RGB components.
    /// </summary>
    /// <param name="left">Color operand.</param>
    /// <param name="right">Scalar operand.</param>
    /// <param name="op">Operator to apply.</param>
    /// <returns>Resulting color.</returns>
    private static Color ApplyOperator(Color left, double right, Func<double, double, double> op) =>
        left.With(
            red: op(left.Red, right),
            green: op(left.Green, right),
            blue: op(left.Blue, right),
            alpha: left.Alpha
        );

    /// <summary>
    /// Applies a binary operation between a scalar and a color across RGB components.
    /// </summary>
    /// <param name="left">Scalar operand.</param>
    /// <param name="right">Color operand.</param>
    /// <param name="op">Operator to apply.</param>
    /// <returns>Resulting color.</returns>
    private static Color ApplyOperator(double left, Color right, Func<double, double, double> op) =>
        right.With(
            red: op(left, right.Red),
            green: op(left, right.Green),
            blue: op(left, right.Blue),
            alpha: right.Alpha
        );

    /// <summary>
    /// Adds a scalar to each RGB component of the color.
    /// </summary>
    public static Color operator +(Color left, double right) => ApplyOperator(left, right, (a, b) => a + b);

    /// <summary>
    /// Subtracts a scalar from each RGB component of the color.
    /// </summary>
    public static Color operator -(Color left, double right) => ApplyOperator(left, right, (a, b) => a - b);

    /// <summary>
    /// Multiplies each RGB component of the color by a scalar.
    /// </summary>
    public static Color operator *(Color left, double right) => ApplyOperator(left, right, (a, b) => a * b);

    /// <summary>
    /// Divides each RGB component of the color by a scalar.
    /// </summary>
    public static Color operator /(Color left, double right) => ApplyOperator(left, right, (a, b) => a / b);

    /// <summary>
    /// Adds each RGB component of the color to a scalar.
    /// </summary>
    public static Color operator +(double left, Color right) => ApplyOperator(left, right, (a, b) => a + b);

    /// <summary>
    /// Subtracts each RGB component of the color from a scalar.
    /// </summary>
    public static Color operator -(double left, Color right) => ApplyOperator(left, right, (a, b) => a - b);

    /// <summary>
    /// Multiplies a scalar by each RGB component of the color.
    /// </summary>
    public static Color operator *(double left, Color right) => ApplyOperator(left, right, (a, b) => a * b);

    /// <summary>
    /// Divides a scalar by each RGB component of the color.
    /// </summary>
    public static Color operator /(double left, Color right) => ApplyOperator(left, right, (a, b) => a / b);

    /// <summary>
    /// Adds two colors component-wise.
    /// </summary>
    public static Color operator +(Color left, Color right) => ApplyOperator(left, right, (a, b) => a + b);

    /// <summary>
    /// Subtracts two colors component-wise.
    /// </summary>
    public static Color operator -(Color left, Color right) => ApplyOperator(left, right, (a, b) => a - b);

    /// <summary>
    /// Multiplies two colors component-wise.
    /// </summary>
    public static Color operator *(Color left, Color right) => ApplyOperator(left, right, (a, b) => a * b);

    /// <summary>
    /// Divides two colors component-wise.
    /// </summary>
    public static Color operator /(Color left, Color right) => ApplyOperator(left, right, (a, b) => a / b);

    /// <summary>
    /// Performs a bitwise OR on the packed color values.
    /// </summary>
    public static Color operator |(Color left, Color right) => new(left.Value | right.Value);

    /// <summary>
    /// Performs a bitwise AND on the packed color values.
    /// </summary>
    public static Color operator &(Color left, Color right) => new(left.Value & right.Value);

    /// <summary>
    /// Performs a bitwise XOR on the packed color values.
    /// </summary>
    public static Color operator ^(Color left, Color right) => new(left.Value ^ right.Value);

    /// <summary>
    /// Performs a bitwise NOT on the packed color value.
    /// </summary>
    public static Color operator ~(Color value) => new(~value.Value);
}


