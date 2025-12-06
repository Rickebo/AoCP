using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lib.Grids;
using Lib.Math;

namespace Lib.Color;

/// <summary>
/// JSON converter that serializes <see cref="Color"/> values as RGBA strings (e.g. <c>#RRGGBBAA</c>).
/// </summary>
public class ColorJsonConverter : JsonConverter<Color>
{
    /// <inheritdoc />
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        Color.Parse(reader.GetString() ?? "#000000FF");

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

/// <summary>
/// Represents a packed RGBA color stored as a 32-bit unsigned integer.
/// </summary>
/// <param name="value">Packed 0xRRGGBBAA value.</param>
public readonly struct Color(uint value) : IEquatable<Color>
{
    private static readonly int BaseHashCode = typeof(Color).GetHashCode();

    /// <summary>
    /// Opaque black (<c>#000000FF</c>).
    /// </summary>
    public static readonly Color Black = new(0x000000FF);

    /// <summary>
    /// Opaque white (<c>#FFFFFFFF</c>).
    /// </summary>
    public static readonly Color White = new(0xFFFFFFFF);

    /// <summary>
    /// Transparent black (<c>#00000000</c>).
    /// </summary>
    public static readonly Color TransparentBlack = new(0x00000000);

    /// <summary>
    /// Transparent white (<c>#FFFFFF00</c>).
    /// </summary>
    public static readonly Color TransparentWhite = new(0xFFFFFF00);

    /// <summary>
    /// Gets the packed RGBA value for this color.
    /// </summary>
    public uint Value => value;

    /// <summary>
    /// Mask isolating the red byte.
    /// </summary>
    public const uint RedMask = 0xFFu << RedShift;

    /// <summary>
    /// Mask isolating the green byte.
    /// </summary>
    public const uint GreenMask = 0xFFu << GreenShift;

    /// <summary>
    /// Mask isolating the blue byte.
    /// </summary>
    public const uint BlueMask = 0xFFu << BlueShift;

    /// <summary>
    /// Mask isolating the alpha byte.
    /// </summary>
    public const uint AlphaMask = 0xFFu << AlphaShift;

    /// <summary>
    /// Bit shift for the red channel.
    /// </summary>
    public const int RedShift = 8 * 3;

    /// <summary>
    /// Bit shift for the green channel.
    /// </summary>
    public const int GreenShift = 8 * 2;

    /// <summary>
    /// Bit shift for the blue channel.
    /// </summary>
    public const int BlueShift = 8 * 1;

    /// <summary>
    /// Bit shift for the alpha channel.
    /// </summary>
    public const int AlphaShift = 8 * 0;

    /// <summary>
    /// Gets the red channel as a byte (0-255).
    /// </summary>
    public uint R => (Value & RedMask) >> RedShift;

    /// <summary>
    /// Gets the green channel as a byte (0-255).
    /// </summary>
    public uint G => (Value & GreenMask) >> GreenShift;

    /// <summary>
    /// Gets the blue channel as a byte (0-255).
    /// </summary>
    public uint B => (Value & BlueMask) >> BlueShift;

    /// <summary>
    /// Gets the alpha channel as a byte (0-255).
    /// </summary>
    public uint A => (Value & AlphaMask) >> AlphaShift;

    /// <summary>
    /// Gets the red channel normalized to the 0-1 range.
    /// </summary>
    public double Red => R / 255d;

    /// <summary>
    /// Gets the green channel normalized to the 0-1 range.
    /// </summary>
    public double Green => G / 255d;

    /// <summary>
    /// Gets the blue channel normalized to the 0-1 range.
    /// </summary>
    public double Blue => B / 255d;

    /// <summary>
    /// Gets the alpha channel normalized to the 0-1 range.
    /// </summary>
    public double Alpha => A / 255d;

    /// <summary>
    /// Creates a color from a packed RGBA value.
    /// </summary>
    /// <param name="rgba">Packed <c>0xRRGGBBAA</c> value.</param>
    public static Color FromRgba(uint rgba) => new(rgba);

    /// <summary>
    /// Creates a color from a packed ARGB value.
    /// </summary>
    /// <param name="argb">Packed <c>0xAARRGGBB</c> value.</param>
    public static Color FromArgb(uint argb) => new(argb << 8 | argb >> 24);

    /// <summary>
    /// Converts a 0-1 normalized channel to a byte component.
    /// </summary>
    /// <param name="v">Normalized value.</param>
    /// <returns>The channel as a byte or <see langword="null"/> when <paramref name="v"/> is <see langword="null"/>.</returns>
    public static byte? FromRange(double? v) => v != null ? (byte)(v.Value.Clamp(0, 1) * 255) : null;

    /// <summary>
    /// Parses a hexadecimal nibble character.
    /// </summary>
    /// <param name="ch">Character to parse.</param>
    /// <returns>The numeric nibble value.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="ch"/> is not a valid hexadecimal digit.</exception>
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
    /// Parses a hexadecimal color string (with or without leading <c>#</c>) in RGB, RGBA, RRGGBB, or RRGGBBAA format.
    /// </summary>
    /// <param name="text">The color text to parse.</param>
    /// <returns>A <see cref="Color"/> value.</returns>
    /// <exception cref="ArgumentException">Thrown when the input length is invalid or contains non-hex characters.</exception>
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
            var shortR = (nibbles[0] << 4) | nibbles[0];
            var shortG = (nibbles[1] << 4) | nibbles[1];
            var shortB = (nibbles[2] << 4) | nibbles[2];
            var shortA = nibbles.Length == 4 ? (nibbles[3] << 4) | nibbles[3] : 255;

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
    /// Creates a color by specifying optional channel components in byte or normalized form.
    /// Missing components default to zero for RGB and fully opaque for alpha.
    /// </summary>
    /// <param name="r">Red channel (0-255).</param>
    /// <param name="g">Green channel (0-255).</param>
    /// <param name="b">Blue channel (0-255).</param>
    /// <param name="a">Alpha channel (0-255).</param>
    /// <param name="red">Normalized red value (0-1).</param>
    /// <param name="green">Normalized green value (0-1).</param>
    /// <param name="blue">Normalized blue value (0-1).</param>
    /// <param name="alpha">Normalized alpha value (0-1).</param>
    /// <returns>A new <see cref="Color"/>.</returns>
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
    /// Creates a new color by replacing specified channels on the current color.
    /// </summary>
    /// <param name="r">Red channel (0-255).</param>
    /// <param name="g">Green channel (0-255).</param>
    /// <param name="b">Blue channel (0-255).</param>
    /// <param name="a">Alpha channel (0-255).</param>
    /// <param name="red">Normalized red value (0-1).</param>
    /// <param name="green">Normalized green value (0-1).</param>
    /// <param name="blue">Normalized blue value (0-1).</param>
    /// <param name="alpha">Normalized alpha value (0-1).</param>
    /// <returns>A new <see cref="Color"/> with updated channels.</returns>
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
    /// Multiplies the channels by the provided factors.
    /// </summary>
    /// <param name="red">Multiplier for red.</param>
    /// <param name="green">Multiplier for green.</param>
    /// <param name="blue">Multiplier for blue.</param>
    /// <param name="alpha">Multiplier for alpha.</param>
    /// <returns>A new <see cref="Color"/> with scaled channels.</returns>
    public Color Multiply(double? red = null, double? green = null, double? blue = null, double? alpha = null) =>
        With(
            red: red != null ? Red * red.Value : null,
            green: green != null ? Green * green.Value : null,
            blue: blue != null ? Blue * blue.Value : null,
            alpha: alpha != null ? Alpha * alpha.Value : null
        );

    /// <summary>
    /// Adds the provided deltas to each channel, clamping to valid ranges.
    /// </summary>
    /// <param name="red">Delta for red.</param>
    /// <param name="green">Delta for green.</param>
    /// <param name="blue">Delta for blue.</param>
    /// <param name="alpha">Delta for alpha.</param>
    /// <returns>A new <see cref="Color"/> with modified channels.</returns>
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

    /// <summary>
    /// Creates a color from HSL values where hue is in [0,1] and saturation/lightness are [0,1].
    /// </summary>
    /// <param name="h">Hue.</param>
    /// <param name="s">Saturation.</param>
    /// <param name="l">Lightness.</param>
    /// <returns>A new <see cref="Color"/>.</returns>
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
    /// Creates a color from HSV values where hue is in [0,1] and saturation/value are [0,1].
    /// </summary>
    /// <param name="h">Hue.</param>
    /// <param name="s">Saturation.</param>
    /// <param name="v">Value/brightness.</param>
    /// <returns>A new <see cref="Color"/>.</returns>
    public static Color FromHsv(double h, double s, double v)
    {
        var l = v * (1 - s / 2);

        return FromHsl(h, l is 0 or 1 ? 0 : (v - l) / System.Math.Min(l, 1 - l), l);
    }

    /// <summary>
    /// Generates a sequence of visually distinct colors using the golden ratio as a hue step.
    /// </summary>
    /// <param name="count">Number of colors to generate.</param>
    /// <param name="saturation">Saturation to apply to all colors.</param>
    /// <param name="vibrancy">Value/brightness to apply to all colors.</param>
    /// <returns>A sequence of <see cref="Color"/> values.</returns>
    public static IEnumerable<Color> Generate(int count, double saturation = 0.5, double vibrancy = 0.9) =>
        Generate(Random.Shared.NextDouble(), count, saturation, vibrancy);

    /// <summary>
    /// Generates a sequence of visually distinct colors starting from a deterministic hue seed.
    /// </summary>
    /// <param name="seed">Initial hue seed in [0,1].</param>
    /// <param name="count">Number of colors to generate.</param>
    /// <param name="saturation">Saturation to apply to all colors.</param>
    /// <param name="vibrancy">Value/brightness to apply to all colors.</param>
    /// <returns>A sequence of <see cref="Color"/> values.</returns>
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
    /// Creates a heatmap by mapping numeric values to a gradient between two colors.
    /// </summary>
    /// <typeparam name="TCell">Type of the numeric grid cell.</typeparam>
    /// <param name="sourceGrid">Input grid to map.</param>
    /// <param name="from">Color representing the minimum value.</param>
    /// <param name="to">Color representing the maximum value.</param>
    /// <param name="minClamp">Optional explicit minimum clamp.</param>
    /// <param name="maxClamp">Optional explicit maximum clamp.</param>
    /// <returns>An <see cref="ArrayGrid{TValue}"/> of mapped colors.</returns>
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
        var hasRange = range > double.Epsilon;

        // Interpolate each cell value
        foreach (var coord in sourceGrid.Coordinates)
        {
            // Clamp
            TCell clamped = sourceGrid[coord].Clamp(min, max);

            // Interpolate
            double percent = hasRange ? double.CreateSaturating(clamped - min) / range : 0d;
            colorGrid[coord.X, coord.Y] = Between(from, to, percent);
        }

        return colorGrid;
    }

    /// <summary>
    /// Linearly interpolates between two colors by the provided <paramref name="percent"/>.
    /// </summary>
    /// <param name="from">Start color.</param>
    /// <param name="to">End color.</param>
    /// <param name="percent">Interpolation factor between 0 and 1.</param>
    /// <returns>The interpolated <see cref="Color"/>.</returns>
    public static Color Between(Color from, Color to, double percent)
    {
        // Clamp percent between 0 and 1
        percent = percent.Clamp(0.0, 1.0);

        static uint Lerp(double start, double end, double t) =>
            (uint)System.Math.Round(start + (end - start) * t);

        // Interpolate R G B A values
        uint r = Lerp(from.R, to.R, percent);
        uint g = Lerp(from.G, to.G, percent);
        uint b = Lerp(from.B, to.B, percent);
        uint a = Lerp(from.A, to.A, percent);

        return new(r << RedShift | g << GreenShift | b << BlueShift | a << AlphaShift);
    }

    /// <summary>
    /// Interpolates between two colors based on a value relative to a numeric range.
    /// </summary>
    /// <typeparam name="T">Number type used for interpolation.</typeparam>
    /// <param name="from">Start color.</param>
    /// <param name="to">End color.</param>
    /// <param name="curr">Current value.</param>
    /// <param name="min">Minimum of the range.</param>
    /// <param name="max">Maximum of the range.</param>
    /// <returns>The interpolated <see cref="Color"/>.</returns>
    public static Color Between<T>(Color from, Color to, T curr, T min, T max) where T : INumber<T>
    {
        var currD = double.CreateSaturating(curr);
        var minD = double.CreateSaturating(min);
        var maxD = double.CreateSaturating(max);
        var span = maxD - minD;
        if (System.Math.Abs(span) < double.Epsilon)
            return from;

        return Between(from, to, (currD - minD) / span);
    }

    /// <summary>
    /// Returns an RGBA string representation of the color.
    /// </summary>
    public new string ToString() => ToRgbaString();

    /// <summary>
    /// Returns the RGBA string in the format <c>#RRGGBBAA</c>.
    /// </summary>
    /// <returns>The RGBA string.</returns>
    public string ToRgbaString() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

    /// <summary>
    /// Returns the RGB string in the format <c>#RRGGBB</c>.
    /// </summary>
    /// <returns>The RGB string.</returns>
    public string ToRgbString() => $"#{R:X2}{G:X2}{B:X2}";

    /// <summary>
    /// Returns the ARGB string in the format <c>#AARRGGBB</c>.
    /// </summary>
    /// <returns>The ARGB string.</returns>
    public string ToArgbString() => $"#{A:X2}{R:X2}{G:X2}{B:X2}";

    /// <inheritdoc />
    public override bool Equals(object? other) => other is Color color && color.Value == Value;

    /// <inheritdoc />
    public bool Equals(Color other) => other.Value == Value;

    /// <inheritdoc />
    public override int GetHashCode() => BaseHashCode ^ Value.GetHashCode();

    /// <summary>
    /// Determines whether two colors are equal.
    /// </summary>
    public static bool operator ==(Color left, Color right) => left.Value == right.Value;

    /// <summary>
    /// Determines whether two colors are different.
    /// </summary>
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

    /// <summary>
    /// Adds a scalar value to all color channels except alpha.
    /// </summary>
    /// <param name="left">Base color.</param>
    /// <param name="right">Scalar to add.</param>
    /// <returns>The resulting <see cref="Color"/>.</returns>
    public static Color operator +(Color left, double right) => ApplyOperator(left, right, (a, b) => a + b);

    /// <summary>
    /// Subtracts a scalar value from all color channels except alpha.
    /// </summary>
    public static Color operator -(Color left, double right) => ApplyOperator(left, right, (a, b) => a - b);

    /// <summary>
    /// Multiplies all color channels except alpha by a scalar.
    /// </summary>
    public static Color operator *(Color left, double right) => ApplyOperator(left, right, (a, b) => a * b);

    /// <summary>
    /// Divides all color channels except alpha by a scalar.
    /// </summary>
    public static Color operator /(Color left, double right) => ApplyOperator(left, right, (a, b) => a / b);

    /// <summary>
    /// Adds a scalar to all color channels except alpha.
    /// </summary>
    public static Color operator +(double left, Color right) => ApplyOperator(left, right, (a, b) => a + b);

    /// <summary>
    /// Subtracts a color's channels from a scalar.
    /// </summary>
    public static Color operator -(double left, Color right) => ApplyOperator(left, right, (a, b) => a - b);

    /// <summary>
    /// Multiplies all color channels except alpha by a scalar.
    /// </summary>
    public static Color operator *(double left, Color right) => ApplyOperator(left, right, (a, b) => a * b);

    /// <summary>
    /// Divides a scalar by the color channels.
    /// </summary>
    public static Color operator /(double left, Color right) => ApplyOperator(left, right, (a, b) => a / b);

    /// <summary>
    /// Adds channel values component-wise.
    /// </summary>
    public static Color operator +(Color left, Color right) => ApplyOperator(left, right, (a, b) => a + b);

    /// <summary>
    /// Subtracts channel values component-wise.
    /// </summary>
    public static Color operator -(Color left, Color right) => ApplyOperator(left, right, (a, b) => a - b);

    /// <summary>
    /// Multiplies channel values component-wise.
    /// </summary>
    public static Color operator *(Color left, Color right) => ApplyOperator(left, right, (a, b) => a * b);

    /// <summary>
    /// Divides channel values component-wise.
    /// </summary>
    public static Color operator /(Color left, Color right) => ApplyOperator(left, right, (a, b) => a / b);

    /// <summary>
    /// Performs a bitwise OR of the packed color values.
    /// </summary>
    public static Color operator |(Color left, Color right) => new(left.Value | right.Value);

    /// <summary>
    /// Performs a bitwise AND of the packed color values.
    /// </summary>
    public static Color operator &(Color left, Color right) => new(left.Value & right.Value);

    /// <summary>
    /// Performs a bitwise XOR of the packed color values.
    /// </summary>
    public static Color operator ^(Color left, Color right) => new(left.Value ^ right.Value);

    /// <summary>
    /// Performs a bitwise NOT of the packed color value.
    /// </summary>
    public static Color operator ~(Color value) => new(~value.Value);
}


