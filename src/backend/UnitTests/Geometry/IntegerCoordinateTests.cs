using Lib.Geometry;
using Lib.Geometry;

namespace Lib.Geometry.Tests;

public class IntegerCoordinateTests
{
    [Test]
    public void StaticCoordinates_AreDefined()
    {
        Assert.Multiple(() =>
        {
            Assert.That(IntegerCoordinate<int>.Zero, Is.EqualTo(new IntegerCoordinate<int>(0, 0)));
            Assert.That(IntegerCoordinate<int>.One, Is.EqualTo(new IntegerCoordinate<int>(1, 1)));
            Assert.That(IntegerCoordinate<int>.UnitX, Is.EqualTo(new IntegerCoordinate<int>(1, 0)));
            Assert.That(IntegerCoordinate<int>.UnitY, Is.EqualTo(new IntegerCoordinate<int>(0, 1)));
        });
    }

    [Test]
    public void NeighbourEnumerations_FollowCardinalsAndAxes()
    {
        var origin = new IntegerCoordinate<int>(0, 0);

        CollectionAssert.AreEquivalent(
            new[]
            {
                new IntegerCoordinate<int>(0, 1),
                new IntegerCoordinate<int>(1, 0),
                new IntegerCoordinate<int>(-1, 0),
                new IntegerCoordinate<int>(0, -1),
            },
            origin.Neighbours);

        CollectionAssert.AreEquivalent(
            new[]
            {
                new IntegerCoordinate<int>(-1, 0),
                new IntegerCoordinate<int>(1, 0)
            },
            origin.HorizontalNeighbours);

        CollectionAssert.AreEquivalent(
            new[]
            {
                new IntegerCoordinate<int>(0, 1),
                new IntegerCoordinate<int>(0, -1)
            },
            origin.VerticalNeighbours);
    }

    [Test]
    public void DistanceAndMoveHelpers_Work()
    {
        var origin = new IntegerCoordinate<int>(1, 1);
        var other = new IntegerCoordinate<int>(3, 4);

        Assert.Multiple(() =>
        {
            Assert.That(origin.ManhattanLength(), Is.EqualTo(2));
            Assert.That(origin.Distance(other), Is.EqualTo(new Distance<int>(2, 3)));
            Assert.That(origin.Move(new Distance<int>(2, -1)), Is.EqualTo(new IntegerCoordinate<int>(3, 0)));
            Assert.That(origin.Move(Direction.North), Is.EqualTo(new IntegerCoordinate<int>(1, 2)));
        });

        Assert.Multiple(() =>
        {
            Assert.That(origin.DirectionOf(other), Is.EqualTo(new IntegerCoordinate<int>(1, 1)));
            Assert.That(origin.MoveTo(other).Last(), Is.EqualTo(new IntegerCoordinate<int>(3, 3)));
        });
    }

    [Test]
    public void ModuloAndBitwiseOperators_AreAppliedComponentWise()
    {
        var coord = new IntegerCoordinate<int>(5, -1);
        var other = new IntegerCoordinate<int>(3, 3);

        Assert.Multiple(() =>
        {
            Assert.That(coord.Modulo(other), Is.EqualTo(new IntegerCoordinate<int>(2, 2)));
            Assert.That(coord % other, Is.EqualTo(new IntegerCoordinate<int>(2, -1)));
            Assert.That(-coord, Is.EqualTo(new IntegerCoordinate<int>(-5, 1)));
            Assert.That(coord & other, Is.EqualTo(new IntegerCoordinate<int>(5 & 3, -1 & 3)));
            Assert.That(coord | other, Is.EqualTo(new IntegerCoordinate<int>(5 | 3, -1 | 3)));
            Assert.That(coord ^ other, Is.EqualTo(new IntegerCoordinate<int>(5 ^ 3, -1 ^ 3)));
            Assert.That(~coord, Is.EqualTo(new IntegerCoordinate<int>(~5, ~-1)));
        });
    }

    [Test]
    public void ArithmeticOperators_CombineCoordinatesAndScalars()
    {
        var coord = new IntegerCoordinate<int>(4, 6);
        var other = new IntegerCoordinate<int>(2, 3);

        Assert.Multiple(() =>
        {
            Assert.That(coord + other, Is.EqualTo(new IntegerCoordinate<int>(6, 9)));
            Assert.That(coord - other, Is.EqualTo(new IntegerCoordinate<int>(2, 3)));
            Assert.That(coord * 2, Is.EqualTo(new IntegerCoordinate<int>(8, 12)));
            Assert.That(coord / 2, Is.EqualTo(new IntegerCoordinate<int>(2, 3)));
            Assert.That(coord * other, Is.EqualTo(new IntegerCoordinate<int>(8, 18)));
            Assert.That(coord / other, Is.EqualTo(new IntegerCoordinate<int>(2, 2)));
            Assert.That(coord == new IntegerCoordinate<int>(4, 6), Is.True);
            Assert.That(coord != other, Is.True);
        });
    }

    [Test]
    public void StringRepresentations_ExposeValues()
    {
        var coord = new IntegerCoordinate<int>(7, 8);

        Assert.Multiple(() =>
        {
            Assert.That(coord.GetStringX(), Is.EqualTo("7"));
            Assert.That(coord.GetStringY(), Is.EqualTo("8"));
            Assert.That(coord.ToString(), Is.EqualTo("<7 8>"));
        });
    }
}

