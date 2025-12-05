using Lib.Geometry;
using Lib.Grids;

namespace Lib.Tests.Grids;

public class GridAlgorithmsTests
{
    [Test]
    public void FloodFill_FindsReachableCells()
    {
        var grid = new ArrayGrid<char>(3, 3, '.');
        grid[1, 1] = '#';

        var start = new IntegerCoordinate<int>(0, 0);
        var filled = GridAlgorithms.FloodFill(grid, start, c => c == '.');

        Assert.That(
            filled,
            Is.EquivalentTo(
            [
                new IntegerCoordinate<int>(0, 0),
                new IntegerCoordinate<int>(1, 0),
                new IntegerCoordinate<int>(2, 0),
                new IntegerCoordinate<int>(0, 1),
                new IntegerCoordinate<int>(2, 1),
                new IntegerCoordinate<int>(0, 2),
                new IntegerCoordinate<int>(1, 2),
                new IntegerCoordinate<int>(2, 2),
            ]));
    }
}


