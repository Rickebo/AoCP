namespace Common;

/// <summary>
/// Defines the supported horizontal alignments for tabular columns.
/// </summary>
public enum ColumnAlignment
{
    /// <summary>
    /// Aligns content to the left edge of the column.
    /// </summary>
    Left,

    /// <summary>
    /// Centers content within the column width.
    /// </summary>
    Center,

    /// <summary>
    /// Aligns content to the right edge of the column.
    /// </summary>
    Right
}

/// <summary>
/// Builds a text-based table with configurable columns, alignment, and rendering options.
/// </summary>
public sealed class TabularReport
{
    /// <summary>
    /// Column metadata used while rendering.
    /// </summary>
    /// <param name="Header">Header text displayed for the column.</param>
    /// <param name="Alignment">Alignment applied to all cells in the column.</param>
    private sealed record Column(string Header, ColumnAlignment Alignment);

    private readonly List<Column> _columns = [];
    private readonly List<string[]> _rows = [];

    /// <summary>
    /// Gets the number of columns defined for the report.
    /// </summary>
    public int ColumnCount => _columns.Count;

    /// <summary>
    /// Gets the number of rows currently stored in the report.
    /// </summary>
    public int RowCount => _rows.Count;

    /// <summary>
    /// Gets the header labels for the defined columns.
    /// </summary>
    public IReadOnlyList<string> Headers => [.. _columns.Select(c => c.Header)];

    /// <summary>
    /// Gets the alignment values for the defined columns.
    /// </summary>
    public IReadOnlyList<ColumnAlignment> Alignments => [.. _columns.Select(c => c.Alignment)];

    /// <summary>
    /// Gets the rows currently stored in the report.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>> Rows => [.. _rows.Select(r => (IReadOnlyList<string>)[.. r])];

    /// <summary>
    /// Adds a new column to the report and expands existing rows with an empty cell.
    /// </summary>
    /// <param name="header">The header text to display for the column.</param>
    /// <param name="alignment">Alignment applied to all cells in the column.</param>
    /// <returns>The zero-based index of the newly added column.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="header"/> is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="alignment"/> is not a defined <see cref="ColumnAlignment"/> value.
    /// </exception>
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
    /// Adds a row of cells to the report using a parameter array of values.
    /// </summary>
    /// <param name="cells">Cell values; null entries become empty strings.</param>
    /// <returns>The zero-based index of the added row.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cells"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when called before any columns are added.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the number of provided cells exceeds the defined column count.
    /// </exception>
    public int AddRow(params object?[] cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        return AddRow(cells.AsEnumerable());
    }

    /// <summary>
    /// Adds a row of cells to the report from an enumerable sequence.
    /// </summary>
    /// <param name="cells">Sequence of cell values; null entries become empty strings.</param>
    /// <returns>The zero-based index of the added row.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cells"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when called before any columns are added.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the number of provided cells exceeds the defined column count.
    /// </exception>
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
    /// Updates the value of a specific cell.
    /// </summary>
    /// <param name="rowIndex">Zero-based index of the row to update.</param>
    /// <param name="columnIndex">Zero-based index of the column to update.</param>
    /// <param name="value">The new value; null is stored as an empty string.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="rowIndex"/> or <paramref name="columnIndex"/> is outside the valid range.
    /// </exception>
    public void SetCell(int rowIndex, int columnIndex, object? value)
    {
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        _rows[rowIndex][columnIndex] = value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Retrieves the string value stored at the specified row and column.
    /// </summary>
    /// <param name="rowIndex">Zero-based index of the row.</param>
    /// <param name="columnIndex">Zero-based index of the column.</param>
    /// <returns>The cell contents as a string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="rowIndex"/> or <paramref name="columnIndex"/> is outside the valid range.
    /// </exception>
    public string GetCell(int rowIndex, int columnIndex)
    {
        ValidateRowIndex(rowIndex);
        ValidateColumnIndex(columnIndex);

        return _rows[rowIndex][columnIndex];
    }

    /// <summary>
    /// Renders the table as a collection of text lines.
    /// </summary>
    /// <param name="includeHeaderSeparator">
    /// Whether to add a separator line between the headers and the data rows.
    /// </param>
    /// <param name="columnSeparator">String inserted between formatted columns.</param>
    /// <param name="headerSeparator">
    /// Separator string inserted between header underline segments when <paramref name="includeHeaderSeparator"/> is true.
    /// </param>
    /// <returns>An array of formatted lines; empty when no columns are defined.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="columnSeparator"/> is null or when <paramref name="headerSeparator"/> is null while
    /// <paramref name="includeHeaderSeparator"/> is true.
    /// </exception>
    public string[] RenderLines(
        bool includeHeaderSeparator = true,
        string columnSeparator = " | ",
        string headerSeparator = "-+-")
    {
        ArgumentNullException.ThrowIfNull(columnSeparator);
        if (includeHeaderSeparator)
            ArgumentNullException.ThrowIfNull(headerSeparator);

        if (_columns.Count == 0)
            return [];

        var widths = CalculateWidths();
        var alignments = _columns.Select(c => c.Alignment).ToArray();
        var lines = new List<string>(_rows.Count + 2)
        {
            FormatRow([.. _columns.Select(c => c.Header)], widths, alignments, columnSeparator)
        };

        if (includeHeaderSeparator)
            lines.Add(string.Join(headerSeparator, widths.Select(w => new string('-', w))));

        foreach (var row in _rows)
            lines.Add(FormatRow(row, widths, alignments, columnSeparator));

        return [.. lines];
    }

    /// <summary>
    /// Renders the table as a single string joined by <see cref="Environment.NewLine"/>.
    /// </summary>
    /// <param name="includeHeaderSeparator">
    /// Whether to add a separator line between the headers and the data rows.
    /// </param>
    /// <param name="columnSeparator">String inserted between formatted columns.</param>
    /// <param name="headerSeparator">
    /// Separator string inserted between header underline segments when <paramref name="includeHeaderSeparator"/> is true.
    /// </param>
    /// <returns>The formatted table, or an empty string when no columns are defined.</returns>
    public string Render(
        bool includeHeaderSeparator = true,
        string columnSeparator = " | ",
        string headerSeparator = "-+-") =>
        string.Join(Environment.NewLine, RenderLines(includeHeaderSeparator, columnSeparator, headerSeparator));

    /// <summary>
    /// Calculates the width required for each column based on headers and cell contents.
    /// </summary>
    /// <returns>An array containing the width for each column.</returns>
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

    /// <summary>
    /// Formats a row of cells using the provided widths and alignment rules.
    /// </summary>
    /// <param name="cells">The cell contents to format.</param>
    /// <param name="widths">Calculated widths for each column.</param>
    /// <param name="alignments">Alignment for each column.</param>
    /// <param name="columnSeparator">String inserted between formatted columns.</param>
    /// <returns>The formatted row string.</returns>
    private static string FormatRow(
        string[] cells,
        int[] widths,
        ColumnAlignment[] alignments,
        string columnSeparator)
    {
        var formatted = new string[cells.Length];
        for (var i = 0; i < cells.Length; i++)
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

    /// <summary>
    /// Centers a string within the specified width by padding with spaces.
    /// </summary>
    /// <param name="value">The value to center.</param>
    /// <param name="width">Total width to pad to.</param>
    /// <returns>The centered string.</returns>
    private static string PadCenter(string value, int width)
    {
        if (value.Length >= width)
            return value;

        var pad = width - value.Length;
        var left = pad / 2;
        var right = pad - left;
        return new string(' ', left) + value + new string(' ', right);
    }

    /// <summary>
    /// Validates that the provided row index references an existing row.
    /// </summary>
    /// <param name="rowIndex">Zero-based row index to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the range of existing rows.</exception>
    private void ValidateRowIndex(int rowIndex)
    {
        if ((uint)rowIndex >= (uint)_rows.Count)
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
    }

    /// <summary>
    /// Validates that the provided column index references an existing column.
    /// </summary>
    /// <param name="columnIndex">Zero-based column index to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the range of defined columns.</exception>
    private void ValidateColumnIndex(int columnIndex)
    {
        if ((uint)columnIndex >= (uint)_columns.Count)
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
    }
}
