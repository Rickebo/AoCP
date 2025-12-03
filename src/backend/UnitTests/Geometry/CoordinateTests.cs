using Lib.Coordinate;

namespace Lib.UnitTests.Geometry;

public class CoordinateTests
{
    [Test]
    public void StaticCoordinates_AreCorrect()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Coordinate<int>.Zero, Is.EqualTo(new Coordinate<int>(0, 0)));
            Assert.That(Coordinate<int>.One, Is.EqualTo(new Coordinate<int>(1, 1)));
            Assert.That(Coordinate<int>.UnitX, Is.EqualTo(new Coordinate<int>(1, 0)));
            Assert.That(Coordinate<int>.UnitY, Is.EqualTo(new Coordinate<int>(0, 1)));
        });
    }

    [Test]
    public void MinMaxClamp_CopySignAndAbs_Work()
    {
        var coord = new Coordinate<int>(2, -3);
        var other = new Coordinate<int>(-1, 5);

        Assert.Multiple(() =>
        {
            Assert.That(coord.Min(other), Is.EqualTo(new Coordinate<int>(-1, -3)));
            Assert.That(coord.Max(other), Is.EqualTo(new Coordinate<int>(2, 5)));
            Assert.That(coord.Clamp(new Coordinate<int>(0, 0), new Coordinate<int>(2, 2)), Is.EqualTo(new Coordinate<int>(2, 0)));
            Assert.That(coord.CopySign(other), Is.EqualTo(new Coordinate<int>(-2, 3)));
            Assert.That(coord.Abs(), Is.EqualTo(new Coordinate<int>(2, 3)));
        });
    }

    [Test]
    public void Operators_CombineCoordinatesAndScalars()
    {
        var coord = new Coordinate<int>(2, 4);
        var other = new Coordinate<int>(1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(coord + other, Is.EqualTo(new Coordinate<int>(3, 5)));
            Assert.That(coord - other, Is.EqualTo(new Coordinate<int>(1, 3)));
            Assert.That(coord * 2, Is.EqualTo(new Coordinate<int>(4, 8)));
            Assert.That(coord / 2, Is.EqualTo(new Coordinate<int>(1, 2)));
            Assert.That(coord * other, Is.EqualTo(new Coordinate<int>(2, 4)));
            Assert.That(coord / other, Is.EqualTo(new Coordinate<int>(2, 4)));
            Assert.That(coord == new Coordinate<int>(2, 4), Is.True);
            Assert.That(coord != other, Is.True);
        });
    }

    [Test]
    public void StringCoordinate_ExposesComponents()
    {
        var coord = new Coordinate<int>(7, 8);

        Assert.Multiple(() =>
        {
            Assert.That(coord.GetStringX(), Is.EqualTo("7"));
            Assert.That(coord.GetStringY(), Is.EqualTo("8"));
        });
    }
}
