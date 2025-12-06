namespace Common.Updates;

/// <summary>
/// Conveys textual output produced while a problem is running.
/// </summary>
public class TextProblemUpdate : OngoingProblemUpdate
{
    /// <inheritdoc />
    public override string Type => "text";
    
    /// <summary>
    /// Gets or sets the lines of text to append to the log.
    /// </summary>
    public string[]? Lines { get; set; }

    /// <summary>
    /// Gets or sets a block of text to append to the log.
    /// </summary>
    public string? Text { get; set; }

    private static string[] NormalizeLines(IEnumerable<string> lines) =>
        lines.Select(line => line ?? string.Empty).ToArray();

    private static string Format(IFormattable value) => value.ToString() ?? string.Empty;

    /// <summary>
    /// Creates an update containing a single formatted line.
    /// </summary>
    /// <param name="line">Line to emit.</param>
    /// <returns>A <see cref="TextProblemUpdate"/> with one line.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="line"/> is null.</exception>
    public static TextProblemUpdate FromLine(IFormattable line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return new TextProblemUpdate
        {
            Lines = [Format(line)],
            Text = null
        };
    }

    /// <summary>
    /// Creates an update containing a single line.
    /// </summary>
    /// <param name="line">Line to emit.</param>
    /// <returns>A <see cref="TextProblemUpdate"/> with one line.</returns>
    public static TextProblemUpdate FromLine(string line) =>
        new()
        {
            Lines = [line ?? string.Empty],
            Text = null
        };

    /// <summary>
    /// Creates an update containing multiple formatted lines.
    /// </summary>
    /// <param name="lines">Lines to emit.</param>
    /// <returns>A <see cref="TextProblemUpdate"/> with the given lines.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="lines"/> is null.</exception>
    public static TextProblemUpdate FromLines(IFormattable[] lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        return new TextProblemUpdate
        {
            Lines = NormalizeLines(lines.Select(Format)),
            Text = null
        };
    }

    /// <summary>
    /// Creates an update containing multiple lines.
    /// </summary>
    /// <param name="lines">Lines to emit.</param>
    /// <returns>A <see cref="TextProblemUpdate"/> with the given lines.</returns>
    public static TextProblemUpdate FromLines(string[] lines) =>
        new()
        {
            Lines = lines == null ? [] : NormalizeLines(lines),
            Text = null
        };

    /// <summary>
    /// Creates an update containing multiple lines.
    /// </summary>
    /// <param name="lines">Lines to emit.</param>
    /// <returns>A <see cref="TextProblemUpdate"/> with the given lines.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="lines"/> is null.</exception>
    public static TextProblemUpdate FromLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        return new TextProblemUpdate
        {
            Lines = NormalizeLines(lines),
            Text = null
        };
    }

    /// <summary>
    /// Creates an update containing a text block.
    /// </summary>
    /// <param name="text">Text to emit.</param>
    /// <returns>A <see cref="TextProblemUpdate"/> with the given text.</returns>
    public static TextProblemUpdate FromText(string text) =>
        new()
        {
            Lines = null,
            Text = text ?? string.Empty
        };

    /// <summary>
    /// Creates an update containing a formatted text block.
    /// </summary>
    /// <param name="text">Text to emit.</param>
    /// <returns>A <see cref="TextProblemUpdate"/> with the given text.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
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

