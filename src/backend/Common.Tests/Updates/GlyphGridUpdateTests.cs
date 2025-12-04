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
}
