using Lib.Coordinate;
using Lib.Grid;
using Lib.Grids;

namespace Lib.UnitTests.Grids;

public class GridSearchSourceTests
{
    [Test]
    public void ToElement_WrapsCoordinateAndValue()
    {
        var grid = new ArrayGrid<int>(2, 1);
        grid[1, 0] = 5;
        var source = new GridSearchSource<int, int>(grid, (_, v) => v >= 0, (_, _, _, dest) => dest);

        var element = source.ToElement(new IntegerCoordinate<int>(1, 0));

        Assert.That(element.Value, Is.EqualTo(5));
    }

    [Test]
    public void GetNeighbours_RespectsWalkableAndDiagonals()
    {
        var grid = new ArrayGrid<int>(2, 2);
        grid[0, 0] = 1;
        grid[1, 0] = -1; // blocked
        grid[0, 1] = 2;
        grid[1, 1] = 3;

        var source = new GridSearchSource<int, int>(
            grid,
            (_, value) => value >= 0,
            (_, _, _, dest) => dest,
            includeDiagonals: true);

        var start = source.ToElement(new IntegerCoordinate<int>(0, 0));
        var neighbours = source.GetNeighbours(start).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(neighbours.Length, Is.EqualTo(2));
            CollectionAssert.DoesNotContain(neighbours.Select(n => n.Element.Coordinate), new IntegerCoordinate<int>(1, 0));
            Assert.That(neighbours.Any(n => n.Cost == 3), Is.True);
        });
    }
}
