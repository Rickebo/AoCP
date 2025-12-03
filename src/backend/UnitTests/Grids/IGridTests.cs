using Lib.Geometry;
using Lib.Grids;

namespace Lib.Grids.Tests;

public class IGridTests
{
    [Test]
    public void Indexer_AllowsGetAndSet()
    {
        IGrid<int, IntegerCoordinate<int>, int> grid = new ArrayGrid<int>(1, 1);
        var coord = new IntegerCoordinate<int>(0, 0);

        grid[coord] = 42;

        Assert.That(grid[coord], Is.EqualTo(42));
    }
}

