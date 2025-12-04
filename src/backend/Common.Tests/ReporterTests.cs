using Common;
using Common.Updates;
using Lib.Geometry;

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
}
