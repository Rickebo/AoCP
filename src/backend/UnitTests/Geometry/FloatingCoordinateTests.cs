using Lib.Geometry;

namespace Lib.Geometry.Tests;

public class FloatingCoordinateTests
{
    [Test]
    public void Length_ComputesEuclideanDistance()
    {
        var coord = new FloatingCoordinate<double>(3, 4);

        Assert.That(coord.Length, Is.EqualTo(5));
    }

    [Test]
    public void ComponentHelpers_WorkWithFloatingPoint()
    {
        var coord = new FloatingCoordinate<double>(-1.5, 2.5);
        var other = new FloatingCoordinate<double>(1, -2);

        Assert.Multiple(() =>
        {
            Assert.That(coord.Min(other), Is.EqualTo(new FloatingCoordinate<double>(-1.5, -2)));
            Assert.That(coord.Max(other), Is.EqualTo(new FloatingCoordinate<double>(1, 2.5)));
            Assert.That(coord.Clamp(new FloatingCoordinate<double>(-1, -1), new FloatingCoordinate<double>(2, 2)), Is.EqualTo(new FloatingCoordinate<double>(-1, 2)));
            Assert.That(coord.CopySign(other), Is.EqualTo(new FloatingCoordinate<double>(1.5, -2.5)));
            Assert.That(coord.Abs(), Is.EqualTo(new FloatingCoordinate<double>(1.5, 2.5)));
        });
    }

    [Test]
    public void Operators_HandleArithmetic()
    {
        var coord = new FloatingCoordinate<double>(1, 2);
        var other = new FloatingCoordinate<double>(0.5, 1.5);

        Assert.Multiple(() =>
        {
            Assert.That(-coord, Is.EqualTo(new FloatingCoordinate<double>(-1, -2)));
            Assert.That(coord + other, Is.EqualTo(new FloatingCoordinate<double>(1.5, 3.5)));
            Assert.That(coord - other, Is.EqualTo(new FloatingCoordinate<double>(0.5, 0.5)));
            Assert.That(coord * 2, Is.EqualTo(new FloatingCoordinate<double>(2, 4)));
            Assert.That(coord / 2, Is.EqualTo(new FloatingCoordinate<double>(0.5, 1)));
            Assert.That(coord * other, Is.EqualTo(new FloatingCoordinate<double>(0.5, 3)));
            Assert.That(coord / other, Is.EqualTo(new FloatingCoordinate<double>(2, 1.3333333333333333)));
        });
    }

    [Test]
    public void StringCoordinate_ExposesComponents()
    {
        var coord = new FloatingCoordinate<double>(1.25, 2.5);

        Assert.Multiple(() =>
        {
            Assert.That(coord.GetStringX(), Is.EqualTo(coord.X.ToString()));
            Assert.That(coord.GetStringY(), Is.EqualTo(coord.Y.ToString()));
        });
    }
}

