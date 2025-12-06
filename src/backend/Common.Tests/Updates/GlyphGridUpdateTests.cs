using Common.Updates;
using Lib.Color;
using Lib.Geometry;
using Lib.Grids;

namespace Common.Tests.Updates;

[TestFixture]
public class GlyphGridUpdateTests
{
    [Test]
    public void FromCharGrid_WithColorStrings_BuildsCells()
    {
        var grid = new CharGrid(1, 1, 'A');
        var update = GlyphGridUpdate.FromCharGrid(grid, "#111111", "#222222");

        Assert.Multiple(() =>
        {
            Assert.That(update.Width, Is.EqualTo(1));
            Assert.That(update.Height, Is.EqualTo(1));
            Assert.That(update.Rows["0"]["0"].Char, Is.EqualTo("A"));
            Assert.That(update.Rows["0"]["0"].Fg, Is.EqualTo("#111111"));
            Assert.That(update.Rows["0"]["0"].Bg, Is.EqualTo("#222222"));
        });
    }

    [Test]
    public void Builder_WithGlyphEntry_BuildsRows()
    {
        var update = GlyphGridUpdate.Builder()
            .WithWidth(2)
            .WithHeight(2)
            .WithEntry(
                b => b
                    .WithCoordinate(new IntegerCoordinate<int>(1, 1))
                    .WithGlyph('>')
                    .WithForeground("#FF00FF")
            )
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(update.Rows["1"]["1"].Glyph, Is.EqualTo(">"));
            Assert.That(update.Rows["1"]["1"].Fg, Is.EqualTo("#FF00FF"));
            Assert.That(update.Clear, Is.Null.Or.False);
        });
    }

    [Test]
    public void Builder_WithPath_PopulatesAllCoordinates()
    {
        var coordinates = new List<IntegerCoordinate<int>>
        {
            new(0, 0),
            new(1, 0),
            new(1, 1)
        };

        var update = GlyphGridUpdate.Builder()
            .WithPath(coordinates, foreground: new Color(0x00FF00FF))
            .Build();

        Assert.That(update.Rows.SelectMany(r => r.Value).Count(), Is.EqualTo(coordinates.Count));
    }

    [Test]
    public void Build_WithoutGlyphOrCharacter_Throws()
    {
        var builder = GlyphGridUpdate.Builder()
            .WithEntry(b => b.WithCoordinate(new IntegerCoordinate<int>(0, 0)));

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public void FromGrid_UsesCellConverter()
    {
        var grid = new ArrayGrid<int>(1, 2);
        grid[0, 0] = 5;
        grid[0, 1] = 6;

        var update = GlyphGridUpdate.FromGrid(
            grid,
            cell => new GlyphCell { Glyph = cell.ToString() }
        );

        Assert.Multiple(() =>
        {
            Assert.That(update.Width, Is.EqualTo(1));
            Assert.That(update.Height, Is.EqualTo(2));
            Assert.That(update.Rows["0"]["0"].Glyph, Is.EqualTo("5"));
            Assert.That(update.Rows["1"]["0"].Glyph, Is.EqualTo("6"));
        });
    }

    [Test]
    public void FromCharGrid_WithStringColors_SetsCharAndColors()
    {
        var grid = new CharGrid(1, 1, 'Z');

        var update = GlyphGridUpdate.FromCharGrid(grid, "#FF0000", "#00FF00");

        Assert.Multiple(() =>
        {
            Assert.That(update.Rows["0"]["0"].Char, Is.EqualTo("Z"));
            Assert.That(update.Rows["0"]["0"].Fg, Is.EqualTo("#FF0000"));
            Assert.That(update.Rows["0"]["0"].Bg, Is.EqualTo("#00FF00"));
        });
    }

    [Test]
    public void FromCharGrid_WithColorStruct_SerializesColors()
    {
        var grid = new CharGrid(1, 1, 'A');

        var update = GlyphGridUpdate.FromCharGrid(grid, Color.White, Color.Black);

        Assert.Multiple(() =>
        {
            Assert.That(update.Rows["0"]["0"].Char, Is.EqualTo("A"));
            Assert.That(update.Rows["0"]["0"].Fg, Is.EqualTo(Color.White.ToString()));
            Assert.That(update.Rows["0"]["0"].Bg, Is.EqualTo(Color.Black.ToString()));
        });
    }

    [Test]
    public void Builder_CreatesCellsWithCoordinatesAndDimensions()
    {
        var update = GlyphGridUpdate.Builder()
            .WithWidth(2)
            .WithHeight(2)
            .WithClear()
            .WithEntry(
                builder => builder
                    .WithCoordinate(new IntegerCoordinate<int>(0, 0))
                    .WithGlyph('G')
                    .WithBackground("#123")
            )
            .WithEntry(
                builder => builder
                    .WithCoordinate(new IntegerCoordinate<int>(1, 1))
                    .WithChar('C')
                    .WithForeground(Color.White)
            )
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(update.Clear, Is.True);
            Assert.That(update.Width, Is.EqualTo(2));
            Assert.That(update.Height, Is.EqualTo(2));
            Assert.That(update.Rows["0"]["0"].Glyph, Is.EqualTo("G"));
            Assert.That(update.Rows["0"]["0"].Bg, Is.EqualTo("#123"));
            Assert.That(update.Rows["1"]["1"].Char, Is.EqualTo("C"));
            Assert.That(update.Rows["1"]["1"].Fg, Is.EqualTo(Color.White.ToRgbaString()));
        });
    }

    [Test]
    public void Builder_MissingCoordinate_Throws()
    {
        var builder = GlyphGridUpdate.Builder()
            .WithEntry(entry => entry.WithGlyph('X'));

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public void Builder_MissingGlyphAndChar_Throws()
    {
        var builder = GlyphGridUpdate.Builder()
            .WithEntry(entry => entry.WithCoordinate(new IntegerCoordinate<int>(0, 0)));

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public void Builder_WhitespaceCoordinateComponent_Throws()
    {
        var builder = GlyphGridUpdate.Builder()
            .WithEntry(
                entry => entry.WithCoordinate(new TestStringCoordinate("1", " "))
                    .WithGlyph('A')
            );

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public void Builder_WithPath_AddsGlyphsWithNeighborsAndColors()
    {
        var path = new[]
        {
            new IntegerCoordinate<int>(0, 0),
            new IntegerCoordinate<int>(1, 0),
            new IntegerCoordinate<int>(1, 1)
        };

        var update = GlyphGridUpdate.Builder()
            .WithPath(path, Color.White, Color.Black)
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(update.Rows["0"]["0"].Glyph, Is.EqualTo("E"));
            Assert.That(update.Rows["0"]["1"].Glyph, Is.EqualTo("7"));
            Assert.That(update.Rows["1"]["1"].Glyph, Is.EqualTo("N"));
            Assert.That(update.Rows["0"]["0"].Fg, Is.EqualTo(Color.White.ToRgbaString()));
            Assert.That(update.Rows["0"]["1"].Bg, Is.EqualTo(Color.Black.ToRgbaString()));
            Assert.That(update.Rows["1"]["1"].Fg, Is.EqualTo(Color.White.ToRgbaString()));
        });
    }
}
