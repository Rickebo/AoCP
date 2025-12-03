using Lib.Geometry;
using Lib.Geometry;
using Lib.Grids;

namespace Lib.Grids.Tests;

public class CharGridTests
{
    [Test]
    public void ConstructorFromString_MapsOriginToBottomLeft()
    {
        var grid = new CharGrid("ab\ncd");

        Assert.Multiple(() =>
        {
            Assert.That(grid.Width, Is.EqualTo(2));
            Assert.That(grid.Height, Is.EqualTo(2));
            Assert.That(grid[0, 0], Is.EqualTo('c'));
            Assert.That(grid[1, 1], Is.EqualTo('b'));
        });
    }

    [Test]
    public void CountRepeating_WalksInDirection()
    {
        var grid = new CharGrid(4, 1, 'x');
        grid[2, 0] = 'y';

        var count = grid.CountRepeating(new IntegerCoordinate<int>(0, 0), Direction.East);

        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public void Flip_ReturnsNewCharGrid()
    {
        var grid = new CharGrid("12\n34");
        var flipped = grid.Flip(Axis.X);

        Assert.Multiple(() =>
        {
            Assert.That(flipped[0, 0], Is.EqualTo('4'));
            Assert.That(grid[0, 0], Is.EqualTo('3')); // original unchanged
        });
    }
}

