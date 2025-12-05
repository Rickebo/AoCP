namespace Lib.Geometry.Tests;

public class Coordinate3DTests
{
    [Test]
    public void StaticCoordinates_AreDefined()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Coordinate3D<int>.Zero, Is.EqualTo(new Coordinate3D<int>(0, 0, 0)));
            Assert.That(Coordinate3D<int>.UnitX, Is.EqualTo(new Coordinate3D<int>(1, 0, 0)));
            Assert.That(Coordinate3D<int>.UnitY, Is.EqualTo(new Coordinate3D<int>(0, 1, 0)));
            Assert.That(Coordinate3D<int>.UnitZ, Is.EqualTo(new Coordinate3D<int>(0, 0, 1)));
        });
    }

    [Test]
    public void ComponentHelpers_HandleSignsAndBounds()
    {
        var coord = new Coordinate3D<int>(1, -2, 3);
        var other = new Coordinate3D<int>(-5, 4, -6);

        Assert.Multiple(() =>
        {
            Assert.That(coord.CopySign(other), Is.EqualTo(new Coordinate3D<int>(-1, 2, -3)));
            Assert.That(coord.Abs(), Is.EqualTo(new Coordinate3D<int>(1, 2, 3)));
            Assert.That(coord.ManhattanLength(), Is.EqualTo(6));
            Assert.That(coord.Min(other), Is.EqualTo(new Coordinate3D<int>(-5, -2, -6)));
            Assert.That(coord.Max(other), Is.EqualTo(new Coordinate3D<int>(1, 4, 3)));
            Assert.That(coord.Clamp(new Coordinate3D<int>(0, -1, 2), new Coordinate3D<int>(2, 3, 4)), Is.EqualTo(new Coordinate3D<int>(1, -1, 3)));
        });
    }

    [Test]
    public void Operators_HandleArithmetic()
    {
        var coord = new Coordinate3D<int>(1, 2, 3);
        var other = new Coordinate3D<int>(3, 2, 1);

        Assert.Multiple(() =>
        {
            Assert.That(coord + other, Is.EqualTo(new Coordinate3D<int>(4, 4, 4)));
            Assert.That(coord - other, Is.EqualTo(new Coordinate3D<int>(-2, 0, 2)));
            Assert.That(coord * 2, Is.EqualTo(new Coordinate3D<int>(2, 4, 6)));
            Assert.That(coord / 2, Is.EqualTo(new Coordinate3D<int>(0, 1, 1)));
            Assert.That(coord.Equals(new Coordinate3D<int>(1, 2, 3)), Is.True);
        });
    }

    [Test]
    public void StringsExposeCoordinateComponents()
    {
        var coord = new Coordinate3D<int>(9, 8, 7);

        Assert.Multiple(() =>
        {
            Assert.That(coord.GetStringX(), Is.EqualTo("9"));
            Assert.That(coord.GetStringY(), Is.EqualTo("8"));
            Assert.That(coord.GetStringZ(), Is.EqualTo("7"));
        });
    }
}

