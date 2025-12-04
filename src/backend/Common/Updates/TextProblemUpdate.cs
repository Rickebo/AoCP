namespace Common.Updates;

/// <summary>
/// Text-based update emitted while solving a problem.
/// </summary>
public class TextProblemUpdate : OngoingProblemUpdate
{
    /// <summary>
    /// Gets the update type string used for text payloads.
    /// </summary>
    public override string Type => "text";
    
    /// <summary>
    /// Gets or sets the lines included in the update.
    /// </summary>
    public string[]? Lines { get; set; }

    /// <summary>
    /// Gets or sets a freeform text payload.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Creates a text update from a single formatted line.
    /// </summary>
    /// <param name="line">Line to include.</param>
    /// <returns>A configured <see cref="TextProblemUpdate"/>.</returns>
    public static TextProblemUpdate FromLine(IFormattable line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return new TextProblemUpdate
        {
            Lines = [line.ToString() ?? ""],
            Text = null
        };
    }

    /// <summary>
    /// Creates a text update from a single line.
    /// </summary>
    /// <param name="line">Line to include.</param>
    /// <returns>A configured <see cref="TextProblemUpdate"/>.</returns>
    public static TextProblemUpdate FromLine(string line) =>
        new()
        {
            Lines = [line ?? string.Empty],
            Text = null
        };

    /// <summary>
    /// Creates a text update from formatted lines.
    /// </summary>
    /// <param name="lines">Lines to include.</param>
    /// <returns>A configured <see cref="TextProblemUpdate"/>.</returns>
    public static TextProblemUpdate FromLines(IFormattable[] lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        return new TextProblemUpdate
        {
            Lines = [.. lines.Select(x => x.ToString() ?? "")],
            Text = null
        };
    }

    /// <summary>
    /// Creates a text update from string lines.
    /// </summary>
    /// <param name="lines">Lines to include.</param>
    /// <returns>A configured <see cref="TextProblemUpdate"/>.</returns>
    public static TextProblemUpdate FromLines(string[] lines) =>
        new()
        {
            Lines = lines ?? [],
            Text = null
        };

    /// <summary>
    /// Creates a text update from a general enumerable of lines.
    /// </summary>
    /// <param name="lines">Lines to include.</param>
    /// <returns>A configured <see cref="TextProblemUpdate"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="lines"/> is null.</exception>
    public static TextProblemUpdate FromLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        return new TextProblemUpdate
        {
            Lines = [.. lines],
            Text = null
        };
    }

    /// <summary>
    /// Creates a text update from a freeform text payload.
    /// </summary>
    /// <param name="text">Text content.</param>
    /// <returns>A configured <see cref="TextProblemUpdate"/>.</returns>
    public static TextProblemUpdate FromText(string text) =>
        new()
        {
            Lines = null,
            Text = text ?? string.Empty
        };

    /// <summary>
    /// Creates a text update from a formatted payload.
    /// </summary>
    /// <param name="text">Text to format.</param>
    /// <returns>A configured <see cref="TextProblemUpdate"/>.</returns>
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

