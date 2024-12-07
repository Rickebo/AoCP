using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Lib;

[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct Color32 : ISerializable
{
    [FieldOffset(0x0)]
    public byte R;

    [FieldOffset(0x1)]
    public byte G;

    [FieldOffset(0x2)]
    public byte B;

    [FieldOffset(0x3)]
    public byte Transparency;

    #region Constructors

    public Color32(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;

        Transparency = 0;
    }

    public Color32(SerializationInfo info, StreamingContext context)
    {
        R = info.GetByte("r");
        G = info.GetByte("g");
        B = info.GetByte("b");

        Transparency = 0;
        A = info.GetByte("a");
    }

    public Color32(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;

        Transparency = 0;
        A = a;
    }

    public Color32(double r, double g, double b)
    {
        R = ToByte(r);
        G = ToByte(g);
        B = ToByte(b);

        Transparency = 0;
    }

    public Color32(double r, double g, double b, double a)
    {
        R = ToByte(r);
        G = ToByte(g);
        B = ToByte(b);

        Transparency = 0;
        AF = a;
    }

    #endregion

    #region Get/set

    public byte A
    {
        get => (byte)(byte.MaxValue - Transparency);
        set => Transparency = (byte)(byte.MaxValue - value);
    }

    public byte Red
    {
        get => R;
        set => R = value;
    }

    public byte Green
    {
        get => G;
        set => G = value;
    }

    public byte Blue
    {
        get => B;
        set => B = value;
    }

    public byte Alpha
    {
        get => A;
        set => A = value;
    }

    public double TransparencyF
    {
        get => ToDouble(Transparency);
        set => Transparency = ToByte(value);
    }

    public double RF
    {
        get => ToDouble(R);
        set => R = ToByte(value);
    }

    public double GF
    {
        get => ToDouble(G);
        set => G = ToByte(value);
    }

    public double BF
    {
        get => ToDouble(B);
        set => B = ToByte(value);
    }

    public double AF
    {
        get => ToDouble(A);
        set => A = ToByte(value);
    }

    public double RedF
    {
        get => RF;
        set => RF = value;
    }

    public double GreenF
    {
        get => GF;
        set => GF = value;
    }

    public double BlueF
    {
        get => BF;
        set => BF = value;
    }

    public double AlphaF
    {
        get => AF;
        set => AF = value;
    }

    #endregion

    #region Static conversions

    public static double ToDouble(byte value) => value / (double)byte.MaxValue;

    public static byte ToByte(double value)
    {
        if (value > 1)
            return byte.MaxValue;
        if (value < 0)
            return byte.MinValue;

        return (byte)(value * byte.MaxValue);
    }

    /// <summary>
    /// Converts a hex value to a 32 bit color. 
    /// </summary>
    /// <param name="hex">The hex value, specified as A, Rgb or Argb</param>
    /// <returns></returns>
    public static Color32 FromArgb(string hex)
    {
        hex = hex.TrimStart('#');

        var sa = "FF";
        var sr = "FF";
        var sg = "FF";
        var sb = "FF";

        switch (hex.Length)
        {
            case 1:
            case 2:
            {
                sa += hex[0];
                break;
            }
            case 3:
            {
                // One "letter" per color
                sr += hex[0];
                sg += hex[1];
                sb += hex[2];
                break;
            }
            case 4:
            case 5:
            {
                sa += hex[0];
                sr += hex[1];
                sg += hex[2];
                sb += hex[3];

                break;
            }
            case 6:
            case 7:
            {
                sr = hex.Substring(0, 2);
                sg = hex.Substring(2, 2);
                sb = hex.Substring(4, 2);
                break;
            }
            case 8:
            {
                sa = hex.Substring(0, 2);
                sr = hex.Substring(2, 2);
                sg = hex.Substring(4, 2);
                sb = hex.Substring(6, 2);
                break;
            }
        }

        var r = MathExtensions.HexToFloat(sr);
        var g = MathExtensions.HexToFloat(sg);
        var b = MathExtensions.HexToFloat(sb);
        var a = MathExtensions.HexToFloat(sa);

        return new Color32(r, g, b, a);
    }

    public static unsafe Color32 FromArgb(int color)
    {
        var ptr = (byte*)&color;
        var i = 1;
        return new Color32(ptr[i++], ptr[i++], ptr[i], ptr[0]);
    }

    public static unsafe Color32 FromRgba(int color)
    {
        var ptr = (byte*)&color;
        var i = 0;
        return new Color32(ptr[i++], ptr[i++], ptr[i++], ptr[i]);
    }

    #endregion

    #region Color manipulation

    /// <summary>
    /// Modifies the red, green, blue or alpha value of a color. Practical usage example: color.Where(alpha: 100)
    /// </summary>
    /// <param name="red">The new red value. If left as null, no changes will be made to the red value.</param>
    /// <param name="green">The new green value. If left as null, no changes will be made to the green value.</param>
    /// <param name="blue">The new blue value.  If left as null, no changes will be made to the blue value.</param>
    /// <param name="alpha">The new alpha value. If left as null, no changes will be made to the alpha value.</param>
    /// <param name="transparency">The new transparency value. If alpha value is set, this parameter is ignored. If left as null, no changes will be made to the transparency value.</param>
    /// <returns>Returns the result of the modification</returns>
    public Color32 Where(
        byte? red = null,
        byte? green = null,
        byte? blue = null,
        byte? alpha = null,
        byte? transparency = null
    ) =>
        new Color32(
            red ?? R,
            green ?? G,
            blue ?? B,
            alpha ?? (transparency.HasValue ? (byte)(byte.MaxValue - transparency) : A)
        );

    /// <summary>
    /// Modifies the red, green, blue or alpha value of a color as a floating point number. Practical usage example: color.Where(alpha: 0.5)
    /// </summary>
    /// <param name="red">The new red value, specified as a floating point number (in the range 0-1). If left as null, no changes will be made to the red value.</param>
    /// <param name="green">The new green value, specified as a floating point number (in the range 0-1). If left as null, no changes will be made to the green value.</param>
    /// <param name="blue">The new blue value, specified as a floating point number (in the range 0-1). If left as null, no changes will be made to the blue value.</param>
    /// <param name="alpha">The new alpha value, specified as a floating point number (in the range 0-1). If left as null, no changes will be made to the alpha value.</param>
    /// <param name="transparency">The new transparency value, specified as a floating point number (in the range 0-1). If alpha value is set, this parameter is ignored. If left as null, no changes will be made to the transparency value.</param>
    /// <returns>Returns the result of the modification</returns>
    public Color32 Where(
        double? red = null,
        double? green = null,
        double? blue = null,
        double? alpha = null,
        double? transparency = null
    ) =>
        new Color32(
            red ?? RF,
            green ?? GF,
            blue ?? BF,
            alpha ?? (1 - transparency ?? AF)
        );

    /// <summary>
    /// Inverts the colors red, green and blue values by using the xor operator
    /// </summary>
    /// <returns>The result of the color inversion</returns>
    public Color32 Invert() => new(
        R ^= byte.MaxValue,
        G ^= byte.MaxValue,
        B ^= byte.MaxValue
    );

    /// <summary>
    /// Inverts the colors red, green, blue and alpha value by using the xor operator
    /// </summary>
    /// <returns>The result of the color inversion</returns>
    public Color32 InvertA() => Invert().Where(alpha: A ^ byte.MaxValue);

    /// <summary>
    /// Blends two colors by averaging their red, green and blue values.
    /// </summary>
    /// <param name="color">The color to blend with.</param>
    /// <returns>The result of the color blend</returns>
    public Color32 Blend(Color32 color) => new(
        (byte)((R + color.R) / 2),
        (byte)((G + color.G) / 2),
        (byte)((B + color.B) / 2)
    );

    /// <summary>
    /// Blends two colors by averaging their red, green, blue and alpha values.
    /// </summary>
    /// <param name="color">The color to blend with.</param>
    /// <returns>The result of the color blend</returns>
    public Color32 BlendA(Color32 color) =>
        Blend(color).Where(alpha: (byte)((color.A + A) / 2));

    /// <summary>
    /// Blends the color with its inverse color
    /// </summary>
    /// <returns>The result of the color blend</returns>
    public Color32 BlendInverse() => Blend(Invert());

    #endregion

    #region Operators

    public static Color32 operator *(Color32 color, double d) => new Color32(
        color.RF * d,
        color.GF * d,
        color.BF * d,
        color.AF * d
    );

    public static Color32 operator /(Color32 color, double d) => new Color32(
        color.RF / d,
        color.GF / d,
        color.BF / d,
        color.AF / d
    );

    public static Color32 operator +(Color32 a, Color32 b) => new Color32(
        a.R + b.R,
        a.G + b.G,
        a.B + b.B,
        a.A + b.A
    );

    public static Color32 operator -(Color32 left, Color32 right) => new Color32(
        left.R - right.R,
        left.G - right.G,
        left.B - right.B,
        left.A - right.A
    );

    public static Color32 operator +(Color32 color, byte v) => new Color32(
        color.R + v,
        color.G + v,
        color.B + v,
        color.A + v
    );

    public static Color32 operator -(Color32 color, byte v) => new Color32(
        color.R - v,
        color.G - v,
        color.B - v,
        color.A - v
    );

    public static Color32 operator +(byte v, Color32 color) => color + v;

    public static Color32 operator -(byte v, Color32 color) => new Color32(
        v - color.R,
        v - color.G,
        v - color.B,
        v - color.A
    );

    #endregion

    #region Misc

    public override string ToString() => $"#{A:X2}{R:X2}{G:X2}{B:X2}";

    public override bool Equals(object obj)
    {
        if (obj?.GetType() != typeof(Color32))
            return false;

        var tObj = (Color32)obj;
        return R == tObj.R && G == tObj.G && B == tObj.B && A == tObj.A;
    }

    public override int GetHashCode() => R << 24 | G << 16 | B << 8 | A;

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("r", R);
        info.AddValue("g", G);
        info.AddValue("b", B);
        info.AddValue("a", A);
    }

    public static bool operator ==(Color32 left, Color32 right) => left.Equals(right);

    public static bool operator !=(Color32 left, Color32 right) => !(left == right);

    #endregion
}