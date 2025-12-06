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

    [Test]
    public void FromColorGrid_ConvertsCellsToRgbaStrings()
    {
        var grid = new ArrayGrid<Color>(2, 1);
        grid[0, 0] = Color.FromRgba(0x11223344);
        grid[1, 0] = Color.White;

        var update = StringGridUpdate.FromColorGrid(grid);

        Assert.Multiple(() =>
        {
            Assert.That(update.Width, Is.EqualTo(2));
            Assert.That(update.Height, Is.EqualTo(1));
            Assert.That(update.Rows["0"]["0"], Is.EqualTo("#11223344"));
            Assert.That(update.Rows["0"]["1"], Is.EqualTo(Color.White.ToRgbaString()));
        });
    }

    [Test]
    public void FromStringGrid_HandlesNullCells()
    {
        var grid = new ArrayGrid<string>(2, 1);
        grid[1, 0] = "text";

        var update = StringGridUpdate.FromStringGrid(grid);

        Assert.Multiple(() =>
        {
            Assert.That(update.Rows["0"]["0"], Is.EqualTo(string.Empty));
            Assert.That(update.Rows["0"]["1"], Is.EqualTo("text"));
        });
    }

    [Test]
    public void FromCharGrid_ConvertsCharsToStrings()
    {
        var grid = new ArrayGrid<char>(1, 2);
        grid[0, 0] = 'x';
        grid[0, 1] = 'y';

        var update = StringGridUpdate.FromCharGrid(grid);

        Assert.Multiple(() =>
        {
            Assert.That(update.Rows["0"]["0"], Is.EqualTo("x"));
            Assert.That(update.Rows["1"]["0"], Is.EqualTo("y"));
        });
    }

    [Test]
    public void FromRect_BuildsExpectedRows()
    {
        var origin = new IntegerCoordinate<int>(5, 10);

        var update = StringGridUpdate.FromRect(origin, 2, 2, "#ABCDEF");

        Assert.Multiple(() =>
        {
            Assert.That(update.Width, Is.Null);
            Assert.That(update.Height, Is.Null);
            Assert.That(update.Rows["10"]["5"], Is.EqualTo("#ABCDEF"));
            Assert.That(update.Rows["10"]["6"], Is.EqualTo("#ABCDEF"));
            Assert.That(update.Rows["11"]["5"], Is.EqualTo("#ABCDEF"));
            Assert.That(update.Rows["11"]["6"], Is.EqualTo("#ABCDEF"));
        });
    }

    [Test]
    public void FromRect_WithNonPositiveDimensions_Throws()
    {
        var origin = new IntegerCoordinate<int>(0, 0);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => StringGridUpdate.FromRect(origin, 0, 1, "#FFF")
        );
        Assert.Throws<ArgumentOutOfRangeException>(
            () => StringGridUpdate.FromRect(origin, 1, 0, "#FFF")
        );
    }

    [Test]
    public void FromRect_WithInvalidColor_Throws()
    {
        var origin = new IntegerCoordinate<int>(0, 0);

        Assert.Throws<ArgumentException>(
            () => StringGridUpdate.FromRect(origin, 1, 1, " ")
        );
    }

    [Test]
    public void Builder_PopulatesMetadataAndRows()
    {
        var update = StringGridUpdate.Builder()
            .WithWidth(3)
            .WithHeight(2)
            .WithClear()
            .WithEntry(
                builder => builder
                    .WithCoordinate(new IntegerCoordinate<int>(1, 1))
                    .WithText("A")
            )
            .WithEntry(
                builder => builder
                    .WithCoordinate(new IntegerCoordinate<int>(0, 0))
                    .WithColor(Color.Black)
            )
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(update.Clear, Is.True);
            Assert.That(update.Width, Is.EqualTo(3));
            Assert.That(update.Height, Is.EqualTo(2));
            Assert.That(update.Rows["1"]["1"], Is.EqualTo("A"));
            Assert.That(update.Rows["0"]["0"], Is.EqualTo(Color.Black.ToRgbaString()));
        });
    }

    [Test]
    public void Builder_MissingCoordinate_Throws()
    {
        var builder = StringGridUpdate.Builder()
            .WithEntry(entry => entry.WithText("value"));

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public void Builder_WhitespaceCoordinateComponent_Throws()
    {
        var builder = StringGridUpdate.Builder()
            .WithEntry(entry => entry.WithCoordinate(new TestStringCoordinate(" ", "1")));

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }
}
