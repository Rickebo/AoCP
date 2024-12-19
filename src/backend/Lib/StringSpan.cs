using System.Text;

namespace Lib;

public readonly record struct StringSpan
{
    public string Full { get; }
    public int Start { get; }
    public int Length { get; }
    public int End => Start + Length;

    public StringSpan(string full, int start = 0, int length = -1)
    {
        Full = full;
        Start = start;
        Length = length < 0 ? full.Length - Start : length;
    }

    public StringSpan Substring(int start, int length = -1)
    {
        var newStart = Start + start;

        if (newStart >= Length)
            newStart = Length;

        if (length < 0)
            length = Length - start;

        if (start + length > Length)
            length = Math.Max(0, Length - start);

        var res = new StringSpan(Full, newStart, length);

        if (res.ToString() != Full.Substring(newStart, length))
            throw new Exception();

        return res;
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
            if (this[i] != text[i])
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

        if (!Full.Substring(Start, Length).StartsWith(span.Full))
            throw new Exception();
        
        return true;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Length; i++)
            sb.Append(this[i]);
        return sb.ToString();
    }
}