using Lib.Geometry;
using Lib.Grids;

namespace Lib.Tests.Grids;

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
            Assert.That(grid.Min, Is.EqualTo(coord));
            Assert.That(grid.Max, Is.EqualTo(coord));
        });
    }
}


