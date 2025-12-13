namespace Common;

/// <summary>
/// Specifies how cells in a column should be aligned when rendered.
/// </summary>
public enum ColumnAlignment
{
    /// <summary>
    /// Aligns content to the left edge of the column.
    /// </summary>
    Left,

    /// <summary>
    /// Centers content within the column.
    /// </summary>
    Center,

    /// <summary>
    /// Aligns content to the right edge of the column.
    /// </summary>
    Right
}

/// <summary>
/// Collects tabular data that can be mutated during problem execution and rendered as text once complete.
/// </summary>
public sealed class TabularReport
{
    private sealed record Column(string Header, ColumnAlignment Alignment);

    private readonly List<Column> _columns = [];
    private readonly List<string[]> _rows = [];

    /// <summary>
    /// Gets the number of columns currently defined.
    /// </summary>
    public int ColumnCount => _columns.Count;

    /// <summary>
    /// Gets the number of rows currently stored.
    /// </summary>
    public int RowCount => _rows.Count;

    /// <summary>
    /// Gets the column headers in the order they were added.
    /// </summary>
    public IReadOnlyList<string> Headers => _columns.Select(c => c.Header).ToArray();

    /// <summary>
    /// Adds a column to the table and extends all existing rows with an empty cell.
    /// </summary>
    /// <param name="header">Header text for the new column.</param>
    /// <param name="alignment">Alignment to apply when rendering this column.</param>
    /// <returns>The zero-based index of the added column.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="header"/> is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="alignment"/> is not defined.</exception>
    public int AddColumn(string header, ColumnAlignment alignment = ColumnAlignment.Left)
    {
        ArgumentException.ThrowIfNullOrEmpty(header);

        if (!Enum.IsDefined(typeof(ColumnAlignment), alignment))
            throw new ArgumentOutOfRangeException(nameof(alignment));

        _columns.Add(new Column(header, alignment));

        var newIndex = _columns.Count - 1;
        for (var i = 0; i < _rows.Count; i++)
        {
            var row = _rows[i];
            Array.Resize(ref row, _columns.Count);
            row[newIndex] = string.Empty;
            _rows[i] = row;
        }

        return _columns.Count - 1;
    }

    /// <summary>
    /// Adds a row populated with the provided cells.
    /// </summary>
    /// <param name="cells">Cell values to place in the row. Extra cells beyond the column count are not allowed.</param>
    /// <returns>The zero-based index of the added row.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cells"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when more cells are supplied than there are columns.</exception>
    /// <exception cref="InvalidOperationException">Thrown when attempting to add a row before defining any columns.</exception>
    public int AddRow(params object?[] cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        return AddRow(cells.AsEnumerable());
    }

    /// <summary>
    /// Adds a row populated with the provided cells.
    /// </summary>
    /// <param name="cells">Cell values to place in the row. Extra cells beyond the column count are not allowed.</param>
    /// <returns>The zero-based index of the added row.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cells"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when more cells are supplied than there are columns.</exception>
    /// <exception cref="InvalidOperationException">Thrown when attempting to add a row before defining any columns.</exception>
    public int AddRow(IEnumerable<object?> cells)
    {
        ArgumentNullException.ThrowIfNull(cells);

        if (_columns.Count == 0)
            throw new InvalidOperationException("Add at least one column before adding rows.");

        var materialized = cells.ToArray();
        if (materialized.Length > _columns.Count)
            throw new ArgumentException("Number of cells exceeds the number of columns.", nameof(cells));

        var row = new string[_columns.Count];
        for (var i = 0; i < _columns.Count; i++)
            row[i] = i < materialized.Length ? materialized[i]?.ToString() ?? string.Empty : string.Empty;

        _rows.Add(row);
        return _rows.Count - 1;
    }

    /// <summary>
    /// Updates a specific cell with a new value.
    /// </summary>
    /// <param name="rowIndex">Zero-based row index.</param>
    /// <param name="columnIndex">Zero-based column index.</param>
    /// <param name="value">Value to assign to the cell.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="rowIndex"/> or <paramref name="columnIndex"/> is out of range.</exception>
    public void SetCell(int rowIndex, int columnIndex, object? value)
    {
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        _rows[rowIndex][columnIndex] = value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Retrieves the value currently stored in a cell.
    /// </summary>
    /// <param name="rowIndex">Zero-based row index.</param>
    /// <param name="columnIndex">Zero-based column index.</param>
    /// <returns>The cell value; never null.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="rowIndex"/> or <paramref name="columnIndex"/> is out of range.</exception>
    public string GetCell(int rowIndex, int columnIndex)
    {
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        return _rows[rowIndex][columnIndex];
    }

    /// <summary>
    /// Renders the table into formatted text lines.
    /// </summary>
    /// <param name="includeHeaderSeparator">Whether to include a separator line beneath the header row.</param>
    /// <param name="columnSeparator">Separator placed between columns.</param>
    /// <param name="headerSeparator">Separator placed between header columns; ignored when <paramref name="includeHeaderSeparator"/> is false.</param>
    /// <returns>An array of formatted lines suitable for reporting.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnSeparator"/> is null or when <paramref name="headerSeparator"/> is null while header separators are requested.</exception>
    public string[] RenderLines(
        bool includeHeaderSeparator = true,
        string columnSeparator = " | ",
        string headerSeparator = "-+-")
    {
        ArgumentNullException.ThrowIfNull(columnSeparator);
        if (includeHeaderSeparator)
            ArgumentNullException.ThrowIfNull(headerSeparator);

        if (_columns.Count == 0)
            return Array.Empty<string>();

        var widths = CalculateWidths();
        var alignments = _columns.Select(c => c.Alignment).ToArray();
        var lines = new List<string>(_rows.Count + 2)
        {
            FormatRow(_columns.Select(c => c.Header).ToArray(), widths, alignments, columnSeparator)
        };

        if (includeHeaderSeparator)
            lines.Add(string.Join(headerSeparator, widths.Select(w => new string('-', w))));

        foreach (var row in _rows)
            lines.Add(FormatRow(row, widths, alignments, columnSeparator));

        return lines.ToArray();
    }

    /// <summary>
    /// Renders the table into a single formatted string joined with newline separators.
    /// </summary>
    /// <param name="includeHeaderSeparator">Whether to include a separator line beneath the header row.</param>
    /// <param name="columnSeparator">Separator placed between columns.</param>
    /// <param name="headerSeparator">Separator placed between header columns; ignored when <paramref name="includeHeaderSeparator"/> is false.</param>
    /// <returns>A formatted string representing the table.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnSeparator"/> is null or when <paramref name="headerSeparator"/> is null while header separators are requested.</exception>
    public string Render(
        bool includeHeaderSeparator = true,
        string columnSeparator = " | ",
        string headerSeparator = "-+-") =>
        string.Join(Environment.NewLine, RenderLines(includeHeaderSeparator, columnSeparator, headerSeparator));

    private int[] CalculateWidths()
    {
        var widths = new int[_columns.Count];
        for (var i = 0; i < _columns.Count; i++)
        {
            widths[i] = _columns[i].Header.Length;
            foreach (var row in _rows)
                widths[i] = Math.Max(widths[i], row[i].Length);
        }

        return widths;
    }

    private static string FormatRow(
        IReadOnlyList<string> cells,
        IReadOnlyList<int> widths,
        IReadOnlyList<ColumnAlignment> alignments,
        string columnSeparator)
    {
        var formatted = new string[cells.Count];
        for (var i = 0; i < cells.Count; i++)
        {
            var cell = cells[i] ?? string.Empty;
            formatted[i] = alignments[i] switch
            {
                ColumnAlignment.Right => cell.PadLeft(widths[i]),
                ColumnAlignment.Center => PadCenter(cell, widths[i]),
                _ => cell.PadRight(widths[i])
            };
        }

        return string.Join(columnSeparator, formatted);
    }

    private static string PadCenter(string value, int width)
    {
        if (value.Length >= width)
            return value;

        var pad = width - value.Length;
        var left = pad / 2;
        var right = pad - left;
        return new string(' ', left) + value + new string(' ', right);
    }

    private void ValidateRowIndex(int rowIndex)
    {
        if ((uint)rowIndex >= (uint)_rows.Count)
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
    }

    private void ValidateColumnIndex(int columnIndex)
    {
        if ((uint)columnIndex >= (uint)_columns.Count)
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
    }
}
