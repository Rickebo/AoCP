using Lib.Grid;

namespace Lib.UnitTests.Grids;

public class GridFactoryTests
{
    [Test]
    public void ParseCharGrid_ValidatesDimensions()
    {
        var lines = new[] { "ab", "cd" };
        var grid = GridFactory.ParseCharGrid(lines);

        Assert.Multiple(() =>
        {
            Assert.That(grid.Width, Is.EqualTo(2));
            Assert.That(grid.Height, Is.EqualTo(2));
            Assert.That(grid[0, 1], Is.EqualTo('c'));
        });

        Assert.Throws<ArgumentException>(() => GridFactory.ParseCharGrid(Array.Empty<string>()));
        Assert.Throws<ArgumentException>(() => GridFactory.ParseCharGrid(new[] { "a", "bc" }));
    }

    [Test]
    public void ParseIntGrid_ParsesDigitsAndThrowsOnInvalid()
    {
        var lines = new[] { "12", "34" };
        var grid = GridFactory.ParseIntGrid(lines);

        Assert.Multiple(() =>
        {
            Assert.That(grid.Width, Is.EqualTo(2));
            Assert.That(grid.Height, Is.EqualTo(2));
            Assert.That(grid[1, 1], Is.EqualTo(4));
        });

        Assert.Throws<ArgumentException>(() => GridFactory.ParseIntGrid(new[] { "1x" }));
    }
}
