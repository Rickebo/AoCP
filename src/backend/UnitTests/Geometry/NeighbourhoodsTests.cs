using Lib.Geometry;

namespace Lib.Geometry.Tests;

public class NeighbourhoodsTests
{
    [Test]
    public void Orthogonal_ReturnsFourNeighbours()
    {
        var origin = new IntegerCoordinate<int>(2, 2);
        var neighbours = Neighbourhoods.Orthogonal(origin).ToArray();

        CollectionAssert.AreEquivalent(
            new[]
            {
                new IntegerCoordinate<int>(3, 2),
                new IntegerCoordinate<int>(1, 2),
                new IntegerCoordinate<int>(2, 3),
                new IntegerCoordinate<int>(2, 1)
            },
            neighbours);
    }

    [Test]
    public void All2DNeighbours_ReturnsEightNeighbours()
    {
        var origin = new IntegerCoordinate<int>(0, 0);
        var neighbours = Neighbourhoods.All2DNeighbours(origin).ToArray();

        Assert.That(neighbours, Has.Length.EqualTo(8));
        CollectionAssert.DoesNotContain(neighbours, origin);
    }

    [Test]
    public void Neighbours_CanFilterDirections()
    {
        var origin = new IntegerCoordinate<int>(1, 1);

        var neighbours = Neighbourhoods.Neighbours(origin, Neighbourhood.All, Direction.North | Direction.West).ToArray();

        CollectionAssert.AreEquivalent(
            new[]
            {
                new IntegerCoordinate<int>(2, 1),
                new IntegerCoordinate<int>(2, 0),
                new IntegerCoordinate<int>(1, 0)
            },
            neighbours);
    }

    [Test]
    public void Neighbours_CanReturnDiagonalOnly()
    {
        var origin = new IntegerCoordinate<int>(2, 2);

        var neighbours = Neighbourhoods.Neighbours(origin, Neighbourhood.Diagonal).ToArray();

        CollectionAssert.AreEquivalent(
            new[]
            {
                new IntegerCoordinate<int>(3, 3),
                new IntegerCoordinate<int>(3, 1),
                new IntegerCoordinate<int>(1, 1),
                new IntegerCoordinate<int>(1, 3)
            },
            neighbours);
    }

    [Test]
    public void Orthogonal3DNeighbours_ReturnsSixNeighbours()
    {
        var origin = new Coordinate3D<int>(0, 0, 0);
        var neighbours = Neighbourhoods.Orthogonal3DNeighbours(origin).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(neighbours, Has.Length.EqualTo(6));
            CollectionAssert.Contains(neighbours, new Coordinate3D<int>(1, 0, 0));
            CollectionAssert.Contains(neighbours, new Coordinate3D<int>(0, 0, -1));
        });
    }
}

