namespace Lib.Grids.Tests;

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

        Assert.Throws<ArgumentException>(() => GridFactory.ParseCharGrid([]));
        Assert.Throws<ArgumentException>(() => GridFactory.ParseCharGrid(["a", "bc"]));
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

        Assert.Throws<ArgumentException>(() => GridFactory.ParseIntGrid(["1x"]));
    }
}

