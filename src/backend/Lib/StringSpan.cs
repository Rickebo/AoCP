using System.Text;

namespace Lib;

public readonly struct StringSpan : IEquatable<StringSpan>
{
    public string Full { get; }
    public int Start { get; }
    public int Length { get; }
    public int End => Start + Length;
    private readonly int _hash;

    public StringSpan(string full, int start = 0, int length = -1)
    {
        Full = full;
        Start = start;

        var remaining = Full.Length - start;
        Length = length < 0 ? remaining : Math.Min(remaining, length);

        _hash = HashCode.Combine(start, Length);
        for (var i = 0; i < Length; i++)
            _hash ^= Full[Start + i].GetHashCode();
    }

    public StringSpan Substring(int start, int length = -1)
    {
        var newStart = Start + start;

        return new StringSpan(Full, newStart, length);
    }

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

    public override bool Equals(object? other) =>
        other is StringSpan span && Equals(span);

    public bool Equals(StringSpan other)
    {
        if (other.Length != Length)
            return false;
        
        for (var i = 0; i < Length; i++)
            if (this[i] != other[i])
                return false;

        return true;
    }

    public override int GetHashCode() => _hash;
}