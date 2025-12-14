using Common;

namespace Common.Tests;

[TestFixture]
public class TabularReportTests
{
    [Test]
    public void AddColumn_ExtendsExistingRowsAndReturnsIndex()
    {
        var table = new TabularReport();

        var firstIndex = table.AddColumn("Name");
        var rowIndex = table.AddRow("Alice");
        var secondIndex = table.AddColumn("Score", ColumnAlignment.Right);

        Assert.Multiple(() =>
        {
            Assert.That(firstIndex, Is.EqualTo(0));
            Assert.That(secondIndex, Is.EqualTo(1));
            Assert.That(table.ColumnCount, Is.EqualTo(2));
            Assert.That(table.Headers, Is.EqualTo(new[] { "Name", "Score" }));
            Assert.That(table.GetCell(rowIndex, secondIndex), Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public void AddColumn_InvalidAlignment_Throws()
    {
        var table = new TabularReport();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.AddColumn("Name", (ColumnAlignment)123));
    }

    [Test]
    public void Alignments_ReturnsDefinedAlignmentsInOrder()
    {
        var table = new TabularReport();
        table.AddColumn("Left", ColumnAlignment.Left);
        table.AddColumn("Right", ColumnAlignment.Right);
        table.AddColumn("Center", ColumnAlignment.Center);

        Assert.That(
            table.Alignments,
            Is.EqualTo(new[] { ColumnAlignment.Left, ColumnAlignment.Right, ColumnAlignment.Center })
        );
    }

    [Test]
    public void AddColumn_EmptyHeader_Throws()
    {
        var table = new TabularReport();

        Assert.Throws<ArgumentException>(() => table.AddColumn(string.Empty));
    }

    [Test]
    public void AddRow_FillsMissingCellsWithEmptyStrings()
    {
        var table = new TabularReport();
        table.AddColumn("Name");
        table.AddColumn("Score");

        var rowIndex = table.AddRow("Alice");

        Assert.Multiple(() =>
        {
            Assert.That(table.RowCount, Is.EqualTo(1));
            Assert.That(table.GetCell(rowIndex, 0), Is.EqualTo("Alice"));
            Assert.That(table.GetCell(rowIndex, 1), Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public void AddRow_EnumerableOverload_Works()
    {
        var table = new TabularReport();
        table.AddColumn("Name");

        var rowIndex = table.AddRow(new[] { "Alice" }.AsEnumerable());

        Assert.That(table.GetCell(rowIndex, 0), Is.EqualTo("Alice"));
    }

    [Test]
    public void AddRow_TooManyCells_Throws()
    {
        var table = new TabularReport();
        table.AddColumn("Name");

        Assert.Throws<ArgumentException>(() => table.AddRow("Alice", "Extra"));
    }

    [Test]
    public void AddRow_NoColumns_Throws()
    {
        var table = new TabularReport();

        Assert.Throws<InvalidOperationException>(() => table.AddRow("Alice"));
    }

    [Test]
    public void AddRow_NullCells_Throws()
    {
        var table = new TabularReport();
        table.AddColumn("Name");

        Assert.Throws<ArgumentNullException>(() => table.AddRow(null!));
    }

    [Test]
    public void SetCell_UpdatesStoredValue()
    {
        var table = new TabularReport();
        table.AddColumn("Name");
        table.AddColumn("Score");
        var rowIndex = table.AddRow("Alice", 1);

        table.SetCell(rowIndex, 1, 42);

        Assert.That(table.GetCell(rowIndex, 1), Is.EqualTo("42"));
    }

    [Test]
    public void SetCell_InvalidRow_Throws()
    {
        var table = new TabularReport();
        table.AddColumn("Only");
        table.AddRow("value");

        Assert.Throws<ArgumentOutOfRangeException>(() => table.SetCell(5, 0, "x"));
    }

    [Test]
    public void RenderLines_FormatsAlignedOutput()
    {
        var table = new TabularReport();
        table.AddColumn("Name");
        table.AddColumn("Score", ColumnAlignment.Right);
        table.AddColumn("Note", ColumnAlignment.Center);
        table.AddRow("Alice", 12, "Hi");
        table.AddRow("Bob", 7, "Longer");

        var lines = table.RenderLines();

        var expected = new[]
        {
            "Name  | Score |  Note ",
            "------+-------+-------",
            "Alice |    12 |   Hi  ",
            "Bob   |     7 | Longer"
        };

        Assert.That(lines, Is.EqualTo(expected));
    }

    [Test]
    public void RenderLines_WithHeaderSeparatorDisabled_OmitsSeparator()
    {
        var table = new TabularReport();
        table.AddColumn("Only");
        table.AddRow("Value");

        var lines = table.RenderLines(includeHeaderSeparator: false);

        Assert.That(lines, Is.EqualTo(new[] { "Only ", "Value" }));
    }

    [Test]
    public void Render_NoColumns_ReturnsEmptyString()
    {
        var table = new TabularReport();

        var text = table.Render();

        Assert.That(text, Is.EqualTo(string.Empty));
    }

    [Test]
    public void RenderLines_NoColumns_ReturnsEmptyArray()
    {
        var table = new TabularReport();

        var lines = table.RenderLines();

        Assert.That(lines, Is.Empty);
    }

    [Test]
    public void Render_JoinsLinesWithEnvironmentNewLine()
    {
        var table = new TabularReport();
        table.AddColumn("Only");
        table.AddRow("Row");

        var expected = string.Join(Environment.NewLine, table.RenderLines());

        Assert.That(table.Render(), Is.EqualTo(expected));
    }

    [Test]
    public void GetCell_InvalidColumn_Throws()
    {
        var table = new TabularReport();
        table.AddColumn("Only");
        table.AddRow("value");

        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetCell(0, 5));
    }

    [Test]
    public void Rows_ReturnsSnapshotIndependentOfTableState()
    {
        var table = new TabularReport();
        table.AddColumn("Name");
        table.AddColumn("Score", ColumnAlignment.Right);
        var rowIndex = table.AddRow("Alice", 10);

        var rows = table.Rows;
        table.SetCell(rowIndex, 0, "Bob");
        table.SetCell(rowIndex, 1, 20);

        Assert.Multiple(() =>
        {
            Assert.That(rows[0], Is.EqualTo(new[] { "Alice", "10" }));
            Assert.That(table.GetCell(rowIndex, 0), Is.EqualTo("Bob"));
            Assert.That(table.GetCell(rowIndex, 1), Is.EqualTo("20"));
            Assert.That(table.Rows, Is.Not.EqualTo(rows));
        });
    }

    [Test]
    public void Render_UsesCustomSeparators()
    {
        var table = new TabularReport();
        table.AddColumn("A");
        table.AddColumn("B");
        table.AddRow("X", "Y");

        var lines = table.RenderLines(columnSeparator: " / ", headerSeparator: "---");

        Assert.That(lines, Is.EqualTo(new[] { "A / B", "-----", "X / Y" }));
    }

    [Test]
    public void RenderLines_NullColumnSeparator_Throws()
    {
        var table = new TabularReport();
        table.AddColumn("A");
        table.AddRow("B");

        Assert.Throws<ArgumentNullException>(() => table.RenderLines(columnSeparator: null!));
    }

    [Test]
    public void RenderLines_NullHeaderSeparator_Throws()
    {
        var table = new TabularReport();
        table.AddColumn("A");
        table.AddRow("B");

        Assert.Throws<ArgumentNullException>(() => table.RenderLines(headerSeparator: null!));
    }

    [Test]
    public void SetCell_NullValue_StoresEmptyString()
    {
        var table = new TabularReport();
        table.AddColumn("Only");
        var rowIndex = table.AddRow("value");

        table.SetCell(rowIndex, 0, null);

        Assert.That(table.GetCell(rowIndex, 0), Is.EqualTo(string.Empty));
    }
}
