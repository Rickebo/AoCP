using Common;
using Common.Updates;
using Lib.Geometry;
using Lib.Grids;

namespace Common.Tests;

[TestFixture]
public class ReporterTests
{
    private static ProblemId CreateId() =>
        ProblemId.Create(2024, "Source", "Author", "Set", "Problem");

    [Test]
    public void ReportLine_EnqueuesTextUpdateWithSingleLine()
    {
        var reporter = new Reporter();

        reporter.ReportLine("hello");

        var update = reporter.ReadAllCurrent().Single() as TextProblemUpdate;
        Assert.That(update, Is.Not.Null);
        Assert.That(update!.Lines, Is.EqualTo(new[] { "hello" }));
        Assert.That(update.Text, Is.Null);
    }

    [Test]
    public async Task ReadAll_YieldsUntilFinished()
    {
        var reporter = new Reporter();
        var id = CreateId();

        reporter.Report(new StartProblemUpdate { Id = id });
        var finished = FinishedProblemUpdate.FromSolution("done");
        finished.Id = id;
        reporter.Report(finished);

        var updates = new List<ProblemUpdate>();
        await foreach (var update in reporter.ReadAll())
            updates.Add(update);

        Assert.Multiple(() =>
        {
            Assert.That(updates, Has.Exactly(2).Items);
            Assert.That(updates.First(), Is.TypeOf<StartProblemUpdate>());
            Assert.That(updates.Last(), Is.TypeOf<FinishedProblemUpdate>());
            Assert.That(updates.All(u => u.Id == id), Is.True);
        });
    }

    [Test]
    public void ReportSolution_FromFormattable_AddsFinishedUpdate()
    {
        var reporter = new Reporter();
        reporter.ReportSolution(1234);

        var finished = reporter.ReadAllCurrent().Single() as FinishedProblemUpdate;
        Assert.That(finished, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(finished!.Successful, Is.True);
            Assert.That(finished.Solution, Is.EqualTo("1234"));
            Assert.That(finished.Error, Is.Null);
        });
    }

    [Test]
    public void ReportError_AddsFailedFinishedUpdate()
    {
        var reporter = new Reporter();

        reporter.ReportError("oops", "partial");

        var finished = reporter.ReadAllCurrent().Single() as FinishedProblemUpdate;
        Assert.That(finished, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(finished!.Successful, Is.False);
            Assert.That(finished.Error, Is.EqualTo("oops"));
            Assert.That(finished.Solution, Is.EqualTo("partial"));
        });
    }

    [Test]
    public void ReportTable_MapsTabularReportToTableUpdate()
    {
        var reporter = new Reporter();
        var tabular = new TabularReport();
        tabular.AddColumn("Name", ColumnAlignment.Center);
        tabular.AddColumn("Score", ColumnAlignment.Right);
        tabular.AddRow("Alice", 10);
        tabular.AddRow("Bob", 5);

        reporter.ReportTable(tabular, reset: false);

        var update = reporter.ReadAllCurrent().Single() as TableUpdate;
        Assert.That(update, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(update!.Type, Is.EqualTo("table"));
            Assert.That(update.Reset, Is.False);
            Assert.That(update.Columns.Select(c => c.Header), Is.EqualTo(new[] { "Name", "Score" }));
            Assert.That(update.Columns.Select(c => c.Alignment), Is.EqualTo(new[] { "center", "right" }));
            Assert.That(update.Rows[0], Is.EqualTo(new[] { "Alice", "10" }));
            Assert.That(update.Rows[1], Is.EqualTo(new[] { "Bob", "5" }));
        });
    }

    [Test]
    public void ReportTable_DefaultsResetToTrue()
    {
        var reporter = new Reporter();
        var tabular = new TabularReport();
        tabular.AddColumn("Only");
        tabular.AddRow("Row");

        reporter.ReportTable(tabular);

        var update = reporter.ReadAllCurrent().Single() as TableUpdate;
        Assert.That(update, Is.Not.Null);
        Assert.That(update!.Reset, Is.True);
    }

    [Test]
    public void ReportStringGridUpdate_FromCoordinate_AddsEntry()
    {
        var reporter = new Reporter();
        var coordinate = new IntegerCoordinate<int>(1, 2);

        reporter.ReportStringGridUpdate(coordinate, "X");
        var update = reporter.ReadAllCurrent().Single() as StringGridUpdate;

        Assert.That(update, Is.Not.Null);
        Assert.That(update!.Rows["2"]["1"], Is.EqualTo("X"));
    }

    [Test]
    public void ReportGlyphGridUpdate_FromGrid_FiltersPredicate()
    {
        var reporter = new Reporter();
        var grid = new Lib.Grids.ArrayGrid<int>(2, 2);
        grid[0, 0] = 1;
        grid[1, 1] = 2;

        reporter.ReportGlyphGridUpdate(
            grid,
            (builder, coord, value) => builder
                .WithCoordinate(coord)
                .WithGlyph(value.ToString()),
            (coord, value) => value > 1
        );

        var update = reporter.ReadAllCurrent().Single() as GlyphGridUpdate;
        Assert.That(update, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(update!.Rows, Has.Count.EqualTo(1));
            Assert.That(update.Rows["1"]["1"].Glyph, Is.EqualTo("2"));
        });
    }

    [Test]
    public void ReportText_WithNoContent_ThrowsArgumentException()
    {
        var reporter = new Reporter();

        Assert.Throws<ArgumentException>(() => reporter.ReportText());
    }

    [Test]
    public void ReportText_WithTextAndLines_ThrowsArgumentException()
    {
        var reporter = new Reporter();

        Assert.Throws<ArgumentException>(() => reporter.ReportText("text", ["line"]));
    }

    [Test]
    public void ReportLine_NullLine_TreatedAsEmptyString()
    {
        var reporter = new Reporter();

        reporter.ReportLine(null!);

        var update = reporter.ReadAllCurrent().Single() as TextProblemUpdate;
        Assert.That(update, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(update!.Lines, Is.EqualTo(new[] { string.Empty }));
            Assert.That(update.Text, Is.Null);
        });
    }

    [Test]
    public void ReportStringGridUpdate_FromGrid_SetsDimensionsAndEntries()
    {
        var reporter = new Reporter();
        var grid = new ArrayGrid<string>(2, 1);
        grid[0, 0] = "A";
        grid[1, 0] = "B";

        reporter.ReportStringGridUpdate(
            grid,
            (builder, coord, value) => builder
                .WithCoordinate(coord)
                .WithText(value)
        );

        var update = reporter.ReadAllCurrent().Single() as StringGridUpdate;
        Assert.That(update, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(update!.Width, Is.EqualTo(2));
            Assert.That(update.Height, Is.EqualTo(1));
            Assert.That(update.Rows["0"]["0"], Is.EqualTo("A"));
            Assert.That(update.Rows["0"]["1"], Is.EqualTo("B"));
        });
    }

    [Test]
    public void ReportGlyphGridUpdate_WithClearFlag_PreservesMetadata()
    {
        var reporter = new Reporter();

        reporter.ReportGlyphGridUpdate(
            builder => builder
                .WithClear()
                .WithWidth(1)
                .WithHeight(1)
                .WithEntry(
                    glyph => glyph
                        .WithCoordinate(new IntegerCoordinate<int>(0, 0))
                        .WithChar('X')
                )
        );

        var update = reporter.ReadAllCurrent().Single() as GlyphGridUpdate;
        Assert.That(update, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(update!.Clear, Is.True);
            Assert.That(update.Width, Is.EqualTo(1));
            Assert.That(update.Height, Is.EqualTo(1));
            Assert.That(update.Rows["0"]["0"].Char, Is.EqualTo("X"));
        });
    }
}
