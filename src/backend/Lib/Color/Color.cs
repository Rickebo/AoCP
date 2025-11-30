using Lib.Extensions;
using Lib.Grid;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lib.Color;

public class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        Color.Parse(reader.GetString() ?? "#000000FF");

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

public readonly struct Color(uint value) : IEquatable<Color>
{
    private static readonly int BaseHashCode = typeof(Color).GetHashCode();
    public static readonly Color Black = new(0x000000FF);
    public static readonly Color White = new(0xFFFFFFFF);
    public static readonly Color TransparentBlack = new(0x00000000);
    public static readonly Color TransparentWhite = new(0xFFFFFF00);

    public uint Value => value;

    public const uint RedMask = 0xFFu << RedShift;
    public const uint GreenMask = 0xFFu << GreenShift;
    public const uint BlueMask = 0xFFu << BlueShift;
    public const uint AlphaMask = 0xFFu << AlphaShift;

    public const int RedShift = 8 * 3;
    public const int GreenShift = 8 * 2;
    public const int BlueShift = 8 * 1;
    public const int AlphaShift = 8 * 0;

    public uint R => (Value & RedMask) >> RedShift;
    public uint G => (Value & GreenMask) >> GreenShift;
    public uint B => (Value & BlueMask) >> BlueShift;
    public uint A => (Value & AlphaMask) >> AlphaShift;

    public double Red => R / 255d;
    public double Green => G / 255d;
    public double Blue => B / 255d;
    public double Alpha => A / 255d;

    public static Color FromRgba(uint rgba) => new(rgba);
    public static Color FromArgb(uint argb) => new(argb << 8 | argb >> 24);

    public static byte? FromRange(double? v) => v != null ? (byte)(v.Value.Clamp(0, 1) * 255) : null;

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

    public Color Multiply(double? red = null, double? green = null, double? blue = null, double? alpha = null) =>
        With(
            red: red != null ? Red * red.Value : null,
            green: green != null ? Green * green.Value : null,
            blue: blue != null ? Blue * blue.Value : null,
            alpha: alpha != null ? Alpha * alpha.Value : null
        );

    public Color Add(double? red = null, double? green = null, double? blue = null, double? alpha = null) =>
        With(
            red: red != null ? Red + red.Value : null,
            green: green != null ? Green + green.Value : null,
            blue: blue != null ? Blue + blue.Value : null,
            alpha: alpha != null ? Alpha + alpha.Value : null
        );

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

    public static Color FromHsv(double h, double s, double v)
    {
        var l = v * (1 - s / 2);

        return FromHsl(h, l is 0 or 1 ? 0 : (v - l) / Math.Min(l, 1 - l), l);
    }

    public static IEnumerable<Color> Generate(int count, double saturation = 0.5, double vibrancy = 0.9) =>
        Generate(Random.Shared.NextDouble(), count, saturation, vibrancy);

    public static IEnumerable<Color> Generate(double seed, int count, double saturation = 0.5, double vibrancy = 0.9)
    {
        var goldenRatio = (1 + Math.Sqrt(5)) / 2 - 1;
        var h = seed;
        for (var i = 0; i < count; i++)
        {
            h = (h + goldenRatio) % 1;
            yield return FromHsv(h, saturation, vibrancy);
        }
    }

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

    public static Color Between(Color from, Color to, double percent)
    {
        // Clamp percent between 0 and 1
        percent = percent.Clamp(0.0, 1.0);

        // Interpolate R G B A values
        uint r = (uint)Math.Round(from.R + to.R * percent - from.R * percent);
        uint g = (uint)Math.Round(from.G + to.G * percent - from.G * percent);
        uint b = (uint)Math.Round(from.B + to.B * percent - from.B * percent);
        uint a = (uint)Math.Round(from.A + to.A * percent - from.A * percent);

        return new(r << RedShift | g << GreenShift | b << BlueShift | a << AlphaShift);
    }

    public static Color Between<T>(Color from, Color to, T curr, T min, T max) where T : INumber<T>
    {
        var currD = double.CreateSaturating(curr);
        var minD = double.CreateSaturating(min);
        var maxD = double.CreateSaturating(max);
        return Between(from, to, (currD - minD) / (maxD - minD));
    }

    public new string ToString() => ToRgbaString();
    public string ToRgbaString() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";
    public string ToRgbString() => $"#{R:X2}{G:X2}{B:X2}";
    public string ToArgbString() => $"#{A:X2}{R:X2}{G:X2}{B:X2}";

    public override bool Equals(object? other) => other is Color color && color.Value == Value;

    public bool Equals(Color other) => other.Value == Value;

    public override int GetHashCode() => BaseHashCode ^ Value.GetHashCode();

    public static bool operator ==(Color left, Color right) => left.Value == right.Value;
    public static bool operator !=(Color left, Color right) => left.Value != right.Value;

    private static Color ApplyOperator(Color left, Color right, Func<double, double, double> op) =>
        left.With(
            red: op(left.Red, right.Red),
            green: op(left.Green, right.Green),
            blue: op(left.Blue, right.Blue),
            alpha: op(left.Alpha, right.Alpha)
        );

    private static Color ApplyOperator(Color left, double right, Func<double, double, double> op) =>
        left.With(
            red: op(left.Red, right),
            green: op(left.Green, right),
            blue: op(left.Blue, right),
            alpha: left.Alpha
        );

    private static Color ApplyOperator(double left, Color right, Func<double, double, double> op) =>
        right.With(
            red: op(left, right.Red),
            green: op(left, right.Green),
            blue: op(left, right.Blue),
            alpha: right.Alpha
        );

    public static Color operator +(Color left, double right) => ApplyOperator(left, right, (a, b) => a + b);
    public static Color operator -(Color left, double right) => ApplyOperator(left, right, (a, b) => a - b);
    public static Color operator *(Color left, double right) => ApplyOperator(left, right, (a, b) => a * b);
    public static Color operator /(Color left, double right) => ApplyOperator(left, right, (a, b) => a / b);

    public static Color operator +(double left, Color right) => ApplyOperator(left, right, (a, b) => a + b);
    public static Color operator -(double left, Color right) => ApplyOperator(left, right, (a, b) => a - b);
    public static Color operator *(double left, Color right) => ApplyOperator(left, right, (a, b) => a * b);
    public static Color operator /(double left, Color right) => ApplyOperator(left, right, (a, b) => a / b);

    public static Color operator +(Color left, Color right) => ApplyOperator(left, right, (a, b) => a + b);
    public static Color operator -(Color left, Color right) => ApplyOperator(left, right, (a, b) => a - b);
    public static Color operator *(Color left, Color right) => ApplyOperator(left, right, (a, b) => a * b);
    public static Color operator /(Color left, Color right) => ApplyOperator(left, right, (a, b) => a / b);

    public static Color operator |(Color left, Color right) => new(left.Value | right.Value);
    public static Color operator &(Color left, Color right) => new(left.Value & right.Value);
    public static Color operator ^(Color left, Color right) => new(left.Value ^ right.Value);
    public static Color operator ~(Color value) => new(~value.Value);
}