using Lib.Extensions;
using System.Text;

namespace Lib.Strings;

public readonly struct StringSpan : IEquatable<StringSpan>
{
    public string Full { get; }
    public int Start { get; }
    public int Length { get; }
    public int End => Start + Length;

    private readonly int _hash;

    public StringSpan(string full, int start = 0, int length = int.MaxValue)
    {
        Full = full;
        Start = start.Clamp(0, Full.Length - 1);
        Length = length.Clamp(0, Full.Length - start);
        _hash = HashCode.Combine(start, Length);
        for (var i = 0; i < Length; i++)
            _hash ^= Full[Start + i].GetHashCode();
    }

    public StringSpan Substring(int start = 0, int length = int.MaxValue)
        => new(Full, Start + start, length);

    public char this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException();

            return Full[Start + index];
        }
    }

    public bool StartsWith(string text)
    {
        if (text.Length > Length)
            return false;

        for (var i = 0; i < text.Length; i++)
            if (Full[Start + i] != text[i])
                return false;

        return true;
    }

    public bool StartsWith(StringSpan span)
    {
        if (span.Length > Length)
            return false;

        for (var i = 0; i < span.Length; i++)
            if (this[i] != span[i])
                return false;

        return true;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Length; i++)
            sb.Append(this[i]);
        return sb.ToString();
    }

    public override int GetHashCode() => _hash;
    public static bool operator ==(StringSpan left, StringSpan right) => left.Equals(right);
    public static bool operator !=(StringSpan left, StringSpan right) => !(left == right);
    public override bool Equals(object? other) => other is StringSpan span && Equals(span);
    public bool Equals(StringSpan other)
    {
        if (other.Length != Length)
            return false;
        
        for (var i = 0; i < Length; i++)
            if (this[i] != other[i])
                return false;

        return true;
    }
}