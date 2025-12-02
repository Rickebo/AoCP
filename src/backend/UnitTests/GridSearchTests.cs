using Lib.Coordinate;
using Lib.Grid;
using Lib.Grids;
using Lib.Search;

namespace UnitTests;

[TestFixture]
internal class GridSearchTests
{
    [Test]
    public void AsSearchSource_FiltersNonWalkableCells()
    {
        var grid = new ArrayGrid<int>(3, 3, 0);
        grid[1, 1] = -1;

        var source = grid.AsSearchSource<int>(
            isWalkable: (_, value) => value >= 0
        );

        var origin = source.ToElement(new IntegerCoordinate<int>(1, 0));
        var neighbours = source
            .GetNeighbours(origin)
            .Select(n => n.Element.Coordinate)
            .ToArray();

        var expected = new[]
        {
            new IntegerCoordinate<int>(0, 0),
            new IntegerCoordinate<int>(2, 0)
        };

        Assert.That(neighbours, Is.EquivalentTo(expected));
    }

    [Test]
    public void DijkstraSearch_FindsLowestCostPath_OnArrayGrid()
    {
        var grid = CreateWeightedGrid();

        var source = grid.AsSearchSource<int>(
            costSelector: (_, _, _, destination) => destination
        );

        var start = source.ToElement(new IntegerCoordinate<int>(0, 0));
        var goal = source.ToElement(new IntegerCoordinate<int>(2, 2));

        var search = new DijkstraSearch<GridSearchSource<int, int>, GridSearchElement<int, int>, int>(source);
        var result = search.Find(start, goal);

        if (result is DijkstraSearch<GridSearchSource<int, int>, GridSearchElement<int, int>, int>.SuccessfulResult success)
        {
            Assert.That(success.Cost, Is.EqualTo(4));
            var coordinates = success.Path.Select(e => e.Coordinate).ToArray();

            Assert.That(coordinates, Is.EqualTo(new[]
            {
                new IntegerCoordinate<int>(0, 0),
                new IntegerCoordinate<int>(0, 1),
                new IntegerCoordinate<int>(0, 2),
                new IntegerCoordinate<int>(1, 2),
                new IntegerCoordinate<int>(2, 2)
            }));
        }
        else
        {
            Assert.Fail("Dijkstra search did not find a path to the goal coordinate.");
        }
    }

    [Test]
    public void AStarSearch_UsesHeuristic_OnArrayGrid()
    {
        var grid = CreateWeightedGrid();

        var source = grid.AsSearchSource<int>(
            costSelector: (_, _, _, destination) => destination
        );

        var start = source.ToElement(new IntegerCoordinate<int>(0, 0));
        var goal = source.ToElement(new IntegerCoordinate<int>(2, 2));

        var search = new AStarSearch<GridSearchSource<int, int>, GridSearchElement<int, int>, int>(
            source,
            (current, target) =>
            {
                var dx = Math.Abs(target.Coordinate.X - current.Coordinate.X);
                var dy = Math.Abs(target.Coordinate.Y - current.Coordinate.Y);
                return dx + dy;
            });

        var result = search.Find(start, goal);

        if (result is AStarSearch<GridSearchSource<int, int>, GridSearchElement<int, int>, int>.SuccessfulResult success)
        {
            Assert.That(success.Cost, Is.EqualTo(4));
            var coordinates = success.Path.Select(e => e.Coordinate).ToArray();

            Assert.That(coordinates, Is.EqualTo(new[]
            {
                new IntegerCoordinate<int>(0, 0),
                new IntegerCoordinate<int>(0, 1),
                new IntegerCoordinate<int>(0, 2),
                new IntegerCoordinate<int>(1, 2),
                new IntegerCoordinate<int>(2, 2)
            }));
        }
        else
        {
            Assert.Fail("A* search did not find a path to the goal coordinate.");
        }
    }

    private static ArrayGrid<int> CreateWeightedGrid()
    {
        var grid = new ArrayGrid<int>(3, 3, 1);
        grid[1, 0] = 5;
        grid[1, 1] = 5;
        return grid;
    }
}
