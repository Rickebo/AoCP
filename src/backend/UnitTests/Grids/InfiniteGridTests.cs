using Lib.Geometry;
using Lib.Grids;

namespace Lib.Grids.Tests;

public class InfiniteGridTests
{
    [Test]
    public void IndexerStoresValuesAndTracksBounds()
    {
        var grid = new InfiniteGrid<char, int>();
        var coord = new IntegerCoordinate<int>(-1, 2);

        grid[coord] = 'x';

        Assert.Multiple(() =>
        {
            Assert.That(grid.Contains(coord), Is.True);
            Assert.That(grid.TryGetValue(coord, out var value), Is.True);
            Assert.That(value, Is.EqualTo('x'));
            Assert.That(grid.Min, Is.EqualTo(new IntegerCoordinate<int>(-1, 0)));
            Assert.That(grid.Max, Is.EqualTo(new IntegerCoordinate<int>(0, 2)));
        });
    }
}

