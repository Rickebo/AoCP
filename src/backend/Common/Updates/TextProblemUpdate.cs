namespace Common.Updates;

public class TextProblemUpdate : OngoingProblemUpdate
{
    public override string Type => "text";
    
    public string[]? Lines { get; set; }
    public string? Text { get; set; }

    public static TextProblemUpdate FromLine(IFormattable line) =>
        new()
        {
            Lines = [line.ToString() ?? ""],
            Text = null
        };

    public static TextProblemUpdate FromLine(string line) =>
        new()
        {
            Lines = [line],
            Text = null
        };

    public static TextProblemUpdate FromLines(IFormattable[] lines) =>
        new()
        {
            Lines = lines.Select(x => x.ToString() ?? "").ToArray(),
            Text = null
        };

    public static TextProblemUpdate FromLines(string[] lines) =>
        new()
        {
            Lines = lines,
            Text = null
        };

    public static TextProblemUpdate FromText(string text) =>
        new()
        {
            Lines = null,
            Text = text.ToString() ?? ""
        };
}

