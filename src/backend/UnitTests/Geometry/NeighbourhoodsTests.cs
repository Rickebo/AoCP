namespace Lib.Geometry.Tests;

public class NeighbourhoodsTests
{
    [Test]
    public void Orthogonal_ReturnsFourNeighbours()
    {
        var origin = new IntegerCoordinate<int>(2, 2);
        var neighbours = Neighbourhoods.Orthogonal(origin).ToArray();

        Assert.That(
            neighbours,
            Is.EquivalentTo(
            [
                new IntegerCoordinate<int>(3, 2),
                new IntegerCoordinate<int>(1, 2),
                new IntegerCoordinate<int>(2, 3),
                new IntegerCoordinate<int>(2, 1)
            ]));
    }

    [Test]
    public void All2DNeighbours_ReturnsEightNeighbours()
    {
        var origin = new IntegerCoordinate<int>(0, 0);
        var neighbours = Neighbourhoods.All2DNeighbours(origin).ToArray();

        Assert.That(neighbours, Has.Length.EqualTo(8));
        Assert.That(neighbours, Has.No.Member(origin));
    }

    [Test]
    public void Neighbours_CanFilterDirections()
    {
        var origin = new IntegerCoordinate<int>(1, 1);

        var neighbours = Neighbourhoods.Neighbours(origin, Neighbourhood.All, Direction.North | Direction.West).ToArray();

        Assert.That(
            neighbours,
            Is.EquivalentTo(
            [
                new IntegerCoordinate<int>(2, 1),
                new IntegerCoordinate<int>(2, 0),
                new IntegerCoordinate<int>(1, 0)
            ]));
    }

    [Test]
    public void Neighbours_CanReturnDiagonalOnly()
    {
        var origin = new IntegerCoordinate<int>(2, 2);

        var neighbours = Neighbourhoods.Neighbours(origin, Neighbourhood.Diagonal).ToArray();

        Assert.That(
            neighbours,
            Is.EquivalentTo(
            [
                new IntegerCoordinate<int>(3, 3),
                new IntegerCoordinate<int>(3, 1),
                new IntegerCoordinate<int>(1, 1),
                new IntegerCoordinate<int>(1, 3)
            ]));
    }

    [Test]
    public void Orthogonal3DNeighbours_ReturnsSixNeighbours()
    {
        var origin = new Coordinate3D<int>(0, 0, 0);
        var neighbours = Neighbourhoods.Orthogonal3DNeighbours(origin).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(neighbours, Has.Length.EqualTo(6));
            Assert.That(neighbours, Has.Member(new Coordinate3D<int>(1, 0, 0)));
            Assert.That(neighbours, Has.Member(new Coordinate3D<int>(0, 0, -1)));
        });
    }
}

