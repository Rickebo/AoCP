using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lib.Printing;

public class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Color.Parse(reader.GetString() ?? "#000000FF");
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
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

    public static byte? FromRange(double? v) => v != null ? (byte)(Math.Clamp(v.Value, 0, 1) * 255) : null;

    private static uint Parse(char ch)
    {
        if (char.IsDigit(ch))
            return (uint)(ch - '0');

        ch = char.ToLower(ch);
        var ordinal = 10u + ch - 'a';
        if (ordinal is < 10 or > 15)
            throw new Exception("Cannot parse hexadecimal character '" + ch + "'");

        return ordinal;
    }

    /// <summary>
    /// Parse a Color from a hex string on the format of "#AABBCCDD", "#AABBCC", "#ABCD", "#ABC" where A is the R
    /// component, B is the B component, C is the G component and D is the A component. If components are specified as a
    /// single digit, such as for "#ABC", then each value is shifted to represent the corresponding components most
    /// significant bits. 
    /// </summary>
    /// <param name="text">The text to parse as a color</param>
    /// <returns>The parsed color</returns>
    public static Color Parse(string text)
    {
        if (text.StartsWith('#'))
            text = text[1..];

        var digits = text.Select(Parse).ToArray();

        if (digits.Length is 3 or 4)
        {
            var value = digits[0] << (RedShift + 4) | digits[1] << (GreenShift + 4) | digits[2] << (BlueShift + 4);

            if (digits.Length is 4)
                value = value << 8 | (digits[3] << (AlphaShift + 4));

            return new Color(value);
        }

        var r = digits[0] << 8 | digits[1];
        var g = digits[2] << 8 | digits[3];
        var b = digits[4] << 8 | digits[5];
        var a = digits.Length >= 8 ? digits[6] << 8 | digits[7] : 0;

        return new Color(r << RedShift | g << GreenShift | b << BlueShift | a << AlphaShift);
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

        return new Color((rv << RedShift) | (gv << GreenShift) | (bv << BlueShift) | (av << AlphaShift));
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

public static class ColorTable
{
    public static string Transparent => "#FFFFFF";
    public static string AliceBlue => "#F0F8FF";
    public static string AntiqueWhite => "#FAEBD7";
    public static string Aqua => "#00FFFF";
    public static string Aquamarine => "#7FFFD4";
    public static string Azure => "#F0FFFF";
    public static string Beige => "#F5F5DC";
    public static string Bisque => "#FFE4C4";
    public static string Black => "#000000";
    public static string BlanchedAlmond => "#FFEBCD";
    public static string Blue => "#0000FF";
    public static string BlueViolet => "#8A2BE2";
    public static string Brown => "#A52A2A";
    public static string BurlyWood => "#DEB887";
    public static string CadetBlue => "#5F9EA0";
    public static string Chartreuse => "#7FFF00";
    public static string Chocolate => "#D2691E";
    public static string Coral => "#FF7F50";
    public static string CornflowerBlue => "#6495ED";
    public static string Cornsilk => "#FFF8DC";
    public static string Crimson => "#DC143C";
    public static string Cyan => "#00FFFF";
    public static string DarkBlue => "#00008B";
    public static string DarkCyan => "#008B8B";
    public static string DarkGoldenrod => "#B8860B";
    public static string DarkGray => "#A9A9A9";
    public static string DarkGreen => "#006400";
    public static string DarkKhaki => "#BDB76B";
    public static string DarkMagenta => "#8B008B";
    public static string DarkOliveGreen => "#556B2F";
    public static string DarkOrange => "#FF8C00";
    public static string DarkOrchid => "#9932CC";
    public static string DarkRed => "#8B0000";
    public static string DarkSalmon => "#E9967A";
    public static string DarkSeaGreen => "#8FBC8B";
    public static string DarkSlateBlue => "#483D8B";
    public static string DarkSlateGray => "#2F4F4F";
    public static string DarkTurquoise => "#00CED1";
    public static string DarkViolet => "#9400D3";
    public static string DeepPink => "#FF1493";
    public static string DeepSkyBlue => "#00BFFF";
    public static string DimGray => "#696969";
    public static string DodgerBlue => "#1E90FF";
    public static string Firebrick => "#B22222";
    public static string FloralWhite => "#FFFAF0";
    public static string ForestGreen => "#228B22";
    public static string Fuchsia => "#FF00FF";
    public static string Gainsboro => "#DCDCDC";
    public static string GhostWhite => "#F8F8FF";
    public static string Gold => "#FFD700";
    public static string Goldenrod => "#DAA520";
    public static string Gray => "#808080";
    public static string Green => "#008000";
    public static string GreenYellow => "#ADFF2F";
    public static string Honeydew => "#F0FFF0";
    public static string HotPink => "#FF69B4";
    public static string IndianRed => "#CD5C5C";
    public static string Indigo => "#4B0082";
    public static string Ivory => "#FFFFF0";
    public static string Khaki => "#F0E68C";
    public static string Lavender => "#E6E6FA";
    public static string LavenderBlush => "#FFF0F5";
    public static string LawnGreen => "#7CFC00";
    public static string LemonChiffon => "#FFFACD";
    public static string LightBlue => "#ADD8E6";
    public static string LightCoral => "#F08080";
    public static string LightCyan => "#E0FFFF";
    public static string LightGoldenrodYellow => "#FAFAD2";
    public static string LightGray => "#D3D3D3";
    public static string LightGreen => "#90EE90";
    public static string LightPink => "#FFB6C1";
    public static string LightSalmon => "#FFA07A";
    public static string LightSeaGreen => "#20B2AA";
    public static string LightSkyBlue => "#87CEFA";
    public static string LightSlateGray => "#778899";
    public static string LightSteelBlue => "#B0C4DE";
    public static string LightYellow => "#FFFFE0";
    public static string Lime => "#00FF00";
    public static string LimeGreen => "#32CD32";
    public static string Linen => "#FAF0E6";
    public static string Magenta => "#FF00FF";
    public static string Maroon => "#800000";
    public static string MediumAquamarine => "#66CDAA";
    public static string MediumBlue => "#0000CD";
    public static string MediumOrchid => "#BA55D3";
    public static string MediumPurple => "#9370DB";
    public static string MediumSeaGreen => "#3CB371";
    public static string MediumSlateBlue => "#7B68EE";
    public static string MediumSpringGreen => "#00FA9A";
    public static string MediumTurquoise => "#48D1CC";
    public static string MediumVioletRed => "#C71585";
    public static string MidnightBlue => "#191970";
    public static string MintCream => "#F5FFFA";
    public static string MistyRose => "#FFE4E1";
    public static string Moccasin => "#FFE4B5";
    public static string NavajoWhite => "#FFDEAD";
    public static string Navy => "#000080";
    public static string OldLace => "#FDF5E6";
    public static string Olive => "#808000";
    public static string OliveDrab => "#6B8E23";
    public static string Orange => "#FFA500";
    public static string OrangeRed => "#FF4500";
    public static string Orchid => "#DA70D6";
    public static string PaleGoldenrod => "#EEE8AA";
    public static string PaleGreen => "#98FB98";
    public static string PaleTurquoise => "#AFEEEE";
    public static string PaleVioletRed => "#DB7093";
    public static string PapayaWhip => "#FFEFD5";
    public static string PeachPuff => "#FFDAB9";
    public static string Peru => "#CD853F";
    public static string Pink => "#FFC0CB";
    public static string Plum => "#DDA0DD";
    public static string PowderBlue => "#B0E0E6";
    public static string Purple => "#800080";
    public static string Red => "#FF0000";
    public static string RosyBrown => "#BC8F8F";
    public static string RoyalBlue => "#4169E1";
    public static string SaddleBrown => "#8B4513";
    public static string Salmon => "#FA8072";
    public static string SandyBrown => "#F4A460";
    public static string SeaGreen => "#2E8B57";
    public static string SeaShell => "#FFF5EE";
    public static string Sienna => "#A0522D";
    public static string Silver => "#C0C0C0";
    public static string SkyBlue => "#87CEEB";
    public static string SlateBlue => "#6A5ACD";
    public static string SlateGray => "#708090";
    public static string Snow => "#FFFAFA";
    public static string SpringGreen => "#00FF7F";
    public static string SteelBlue => "#4682B4";
    public static string Tan => "#D2B48C";
    public static string Teal => "#008080";
    public static string Thistle => "#D8BFD8";
    public static string Tomato => "#FF6347";
    public static string Turquoise => "#40E0D0";
    public static string Violet => "#EE82EE";
    public static string Wheat => "#F5DEB3";
    public static string White => "#FFFFFF";
    public static string WhiteSmoke => "#F5F5F5";
    public static string Yellow => "#FFFF00";
    public static string YellowGreen => "#9ACD32";
}