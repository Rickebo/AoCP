using System.Text;

namespace Lib.Text;

/// <summary>
/// Lightweight view over a substring without allocations.
/// </summary>
public readonly struct StringSpan : IEquatable<StringSpan>
{
    /// <summary>
    /// Full source string.
    /// </summary>
    public string Full { get; }

    /// <summary>
    /// Starting index within <see cref="Full"/>.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// Length of the span.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Index immediately after the last character in the span.
    /// </summary>
    public int End => Start + Length;

    private readonly int _hash;

    /// <summary>
    /// Creates a new span over a string.
    /// </summary>
    /// <param name="full">Underlying string.</param>
    /// <param name="start">Start index.</param>
    /// <param name="length">Length of the span.</param>
    public StringSpan(string full, int start = 0, int length = int.MaxValue)
    {
        Full = full;
        Start = System.Math.Clamp(start, 0, System.Math.Max(Full.Length - 1, 0));
        Length = System.Math.Clamp(length, 0, System.Math.Max(Full.Length - start, 0));
        _hash = HashCode.Combine(start, Length);
        for (var i = 0; i < Length; i++)
            _hash ^= Full[Start + i].GetHashCode();
    }

    /// <summary>
    /// Creates a new span relative to the current one.
    /// </summary>
    public StringSpan Substring(int start = 0, int length = int.MaxValue)
        => new(Full, Start + start, length);

    /// <summary>
    /// Gets the character at the given relative index.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">Thrown when index is outside the span.</exception>
    public char this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException();

            return Full[Start + index];
        }
    }

    /// <summary>
    /// Determines whether the span starts with the provided text.
    /// </summary>
    public bool StartsWith(string text)
    {
        if (text.Length > Length)
            return false;

        for (var i = 0; i < text.Length; i++)
            if (Full[Start + i] != text[i])
                return false;

        return true;
    }

    /// <summary>
    /// Determines whether the span starts with another span.
    /// </summary>
    public bool StartsWith(StringSpan span)
    {
        if (span.Length > Length)
            return false;

        for (var i = 0; i < span.Length; i++)
            if (this[i] != span[i])
                return false;

        return true;
    }

    /// <summary>
    /// Materializes the span into a new string.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Length; i++)
            sb.Append(this[i]);
        return sb.ToString();
    }

    /// <inheritdoc />
    public override int GetHashCode() => _hash;

    /// <summary>
    /// Determines whether two spans are equal.
    /// </summary>
    public static bool operator ==(StringSpan left, StringSpan right) => left.Equals(right);

    /// <summary>
    /// Determines whether two spans differ.
    /// </summary>
    public static bool operator !=(StringSpan left, StringSpan right) => !(left == right);

    /// <inheritdoc />
    public override bool Equals(object? other) => other is StringSpan span && Equals(span);

    /// <inheritdoc />
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

