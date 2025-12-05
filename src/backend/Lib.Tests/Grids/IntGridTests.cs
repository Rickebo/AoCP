using Lib.Geometry;

namespace Lib.Grids.Tests;

public class IntGridTests
{
    [Test]
    public void ConstructorFromString_ParsesDigits()
    {
        var grid = new IntGrid("12\n34");

        Assert.Multiple(() =>
        {
            Assert.That(grid.Width, Is.EqualTo(2));
            Assert.That(grid.Height, Is.EqualTo(2));
            Assert.That(grid[0, 0], Is.EqualTo(3));
            Assert.That(grid[1, 1], Is.EqualTo(2));
        });
    }

    [Test]
    public void ConstructorFromString_UsesDefaultValueForNonDigits()
    {
        var grid = new IntGrid("1a\n23", defaultValue: 9);

        Assert.That(grid[1, 1], Is.EqualTo(9));
        Assert.Throws<ArgumentException>(() => new IntGrid("1a\n23"));
    }

    [Test]
    public void CountRepeating_WalksUntilDifferentNumber()
    {
        var grid = new IntGrid(4, 1, 1);
        grid[2, 0] = 2;

        var count = grid.CountRepeating(new IntegerCoordinate<int>(0, 0), Direction.East);

        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public void Flip_ReturnsNewGridWithValuesFlipped()
    {
        var grid = new IntGrid(new[,] { { 1, 2 }, { 3, 4 } });
        var flipped = grid.Flip(Axis.Y);

        Assert.Multiple(() =>
        {
            Assert.That(flipped[0, 0], Is.EqualTo(2));
            Assert.That(grid[0, 0], Is.EqualTo(1));
        });
    }
}

