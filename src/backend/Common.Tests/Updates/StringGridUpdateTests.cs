using Common.Updates;
using Lib.Color;
using Lib.Geometry;
using Lib.Grids;

namespace Common.Tests.Updates;

[TestFixture]
public class StringGridUpdateTests
{
    [Test]
    public void FromRect_FillsExpectedCoordinates()
    {
        var origin = new IntegerCoordinate<int>(1, 2);
        var update = StringGridUpdate.FromRect(origin, 2, 2, "#ABCDEF");

        Assert.Multiple(() =>
        {
            Assert.That(update.Rows["2"]["1"], Is.EqualTo("#ABCDEF"));
            Assert.That(update.Rows["2"]["2"], Is.EqualTo("#ABCDEF"));
            Assert.That(update.Rows["3"]["1"], Is.EqualTo("#ABCDEF"));
            Assert.That(update.Rows["3"]["2"], Is.EqualTo("#ABCDEF"));
        });
    }

    [Test]
    public void FromStringGrid_ConvertsNullCellsToEmptyString()
    {
        var grid = new ArrayGrid<string?>(1, 1);
        var update = StringGridUpdate.FromStringGrid(grid!);

        Assert.That(update.Rows["0"]["0"], Is.EqualTo(string.Empty));
    }

    [Test]
    public void Builder_WithEntryBuildsGridAndClearFlag()
    {
        var update = StringGridUpdate.Builder()
            .WithWidth(2)
            .WithHeight(3)
            .WithClear()
            .WithEntry(
                b => b
                    .WithCoordinate(new IntegerCoordinate<int>(0, 1))
                    .WithText("X")
            )
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(update.Width, Is.EqualTo(2));
            Assert.That(update.Height, Is.EqualTo(3));
            Assert.That(update.Clear, Is.True);
            Assert.That(update.Rows["1"]["0"], Is.EqualTo("X"));
        });
    }

    [Test]
    public void Build_WithoutCoordinate_ThrowsInvalidOperation()
    {
        var builder = StringGridUpdate.Builder()
            .WithEntry(b => b.WithText("X"));

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public void FromColorGrid_UsesColorStringRepresentation()
    {
        var grid = new ArrayGrid<Color>(1, 1, new Color(0xFF0000FF));
        var update = StringGridUpdate.FromColorGrid(grid);

        Assert.That(update.Rows["0"]["0"], Is.EqualTo("#FF0000FF"));
    }
}
