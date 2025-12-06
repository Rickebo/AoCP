namespace Common.Updates;

public class TextProblemUpdate : OngoingProblemUpdate
{
    public override string Type => "text";
    
    public string[]? Lines { get; set; }

    public string? Text { get; set; }

    public static TextProblemUpdate FromLine(IFormattable line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return new TextProblemUpdate
        {
            Lines = [line.ToString() ?? ""],
            Text = null
        };
    }

    public static TextProblemUpdate FromLine(string line) =>
        new()
        {
            Lines = [line ?? string.Empty],
            Text = null
        };

    public static TextProblemUpdate FromLines(IFormattable[] lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        return new TextProblemUpdate
        {
            Lines = [.. lines.Select(x => x.ToString() ?? "")],
            Text = null
        };
    }

    public static TextProblemUpdate FromLines(string[] lines) =>
        new()
        {
            Lines = lines ?? [],
            Text = null
        };

    public static TextProblemUpdate FromLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        return new TextProblemUpdate
        {
            Lines = [.. lines],
            Text = null
        };
    }

    public static TextProblemUpdate FromText(string text) =>
        new()
        {
            Lines = null,
            Text = text ?? string.Empty
        };

    public static TextProblemUpdate FromText(IFormattable text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return new TextProblemUpdate
        {
            Lines = null,
            Text = text.ToString() ?? ""
        };
    }
}

