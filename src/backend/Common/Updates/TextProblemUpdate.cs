namespace Common.Updates;

public class TextProblemUpdate : OngoingProblemUpdate
{
    public override string Type => "text";
    
    public string[]? Lines { get; set; }

    public string? Text { get; set; }

    private static string[] NormalizeLines(IEnumerable<string> lines) =>
        lines.Select(line => line ?? string.Empty).ToArray();

    private static string Format(IFormattable value) => value.ToString() ?? string.Empty;

    public static TextProblemUpdate FromLine(IFormattable line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return new TextProblemUpdate
        {
            Lines = [Format(line)],
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
            Lines = NormalizeLines(lines.Select(Format)),
            Text = null
        };
    }

    public static TextProblemUpdate FromLines(string[] lines) =>
        new()
        {
            Lines = lines == null ? [] : NormalizeLines(lines),
            Text = null
        };

    public static TextProblemUpdate FromLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        return new TextProblemUpdate
        {
            Lines = NormalizeLines(lines),
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
            Text = Format(text)
        };
    }
}

