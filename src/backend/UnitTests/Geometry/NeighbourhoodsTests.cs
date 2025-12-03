using Lib.Geometry;
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

