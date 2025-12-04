using System.Text;

namespace Lib.Text;

/// <summary>
/// Lightweight view over a substring without allocating new strings.
/// </summary>
public readonly struct StringSpan : IEquatable<StringSpan>
{
    /// <summary>
    /// Gets the underlying source string.
    /// </summary>
    public string Full { get; }

    /// <summary>
    /// Gets the starting offset within <see cref="Full"/>.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// Gets the length of the span.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets the index immediately after the last character in the span.
    /// </summary>
    public int End => Start + Length;

    private readonly int _hash;

    /// <summary>
    /// Initializes a new <see cref="StringSpan"/>.
    /// </summary>
    /// <param name="full">Source string.</param>
    /// <param name="start">Start index within the source string.</param>
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
    /// Creates a subspan relative to the current span.
    /// </summary>
    /// <param name="start">Start offset relative to this span.</param>
    /// <param name="length">Length of the new span.</param>
    /// <returns>A new <see cref="StringSpan"/>.</returns>
    public StringSpan Substring(int start = 0, int length = int.MaxValue)
        => new(Full, Start + start, length);

    /// <summary>
    /// Gets the character at the specified index within the span.
    /// </summary>
    /// <param name="index">Index into the span.</param>
    /// <returns>Character at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index lies outside the span.</exception>
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
    /// Determines whether the span starts with the specified string.
    /// </summary>
    /// <param name="text">String to compare against.</param>
    /// <returns><c>true</c> when the prefix matches; otherwise <c>false</c>.</returns>
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
    /// Determines whether the span starts with the specified span.
    /// </summary>
    /// <param name="span">Span to compare against.</param>
    /// <returns><c>true</c> when the prefix matches; otherwise <c>false</c>.</returns>
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
    /// Returns the string represented by this span.
    /// </summary>
    /// <returns>String content of the span.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Length; i++)
            sb.Append(this[i]);
        return sb.ToString();
    }

    /// <summary>
    /// Returns the cached hash code for the span.
    /// </summary>
    /// <returns>Hash code value.</returns>
    public override int GetHashCode() => _hash;

    /// <summary>
    /// Compares two spans for equality.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><c>true</c> when the spans contain identical text.</returns>
    public static bool operator ==(StringSpan left, StringSpan right) => left.Equals(right);

    /// <summary>
    /// Compares two spans for inequality.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><c>true</c> when the spans contain different text.</returns>
    public static bool operator !=(StringSpan left, StringSpan right) => !(left == right);

    /// <summary>
    /// Determines equality with another object.
    /// </summary>
    /// <param name="other">Object to compare.</param>
    /// <returns><c>true</c> when the other object is an equal <see cref="StringSpan"/>.</returns>
    public override bool Equals(object? other) => other is StringSpan span && Equals(span);

    /// <summary>
    /// Determines equality with another span by comparing character content.
    /// </summary>
    /// <param name="other">Span to compare.</param>
    /// <returns><c>true</c> when the spans contain the same text.</returns>
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

