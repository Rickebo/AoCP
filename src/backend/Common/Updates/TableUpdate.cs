namespace Common.Updates;

/// <summary>
/// Conveys a tabular payload to be rendered by the client.
/// </summary>
public sealed class TableUpdate : OngoingProblemUpdate
{
    /// <inheritdoc />
    public override string Type => "table";

    /// <summary>
    /// Gets or sets the column definitions for the table in display order.
    /// </summary>
    public IReadOnlyList<TableColumn> Columns { get; set; } = [];

    /// <summary>
    /// Gets or sets the table rows, aligned with <see cref="Columns"/> by index.
    /// </summary>
    public IReadOnlyList<string[]> Rows { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the client should clear any existing table before applying rows.
    /// Reporters typically set this to true when the table layout changes.
    /// </summary>
    public bool Reset { get; set; }
}

/// <summary>
/// Describes a column in a <see cref="TableUpdate"/>.
/// </summary>
/// <param name="Header">Column header text displayed to the user.</param>
/// <param name="Alignment">
/// Horizontal alignment keyword understood by the client (e.g. "left", "center", "right").
/// </param>
public sealed record TableColumn(string Header, string Alignment);
