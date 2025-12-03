using Lib.Coordinate;

namespace Lib.UnitTests.Geometry;

public class DistanceTests
{
    [Test]
    public void ManhattanAndAbs_Work()
    {
        var distance = new Distance<int>(-3, 4);

        Assert.Multiple(() =>
        {
            Assert.That(distance.Manhattan(), Is.EqualTo(7));
            Assert.That(distance.Abs(), Is.EqualTo(new Distance<int>(3, 4)));
        });
    }

    [Test]
    public void Operators_HandleArithmetic()
    {
        var a = new Distance<int>(1, 2);
        var b = new Distance<int>(3, 4);

        Assert.Multiple(() =>
        {
            Assert.That(a + b, Is.EqualTo(new Distance<int>(4, 6)));
            Assert.That(b - a, Is.EqualTo(new Distance<int>(2, 2)));
            Assert.That(a * 2, Is.EqualTo(new Distance<int>(2, 4)));
            Assert.That(b / 2, Is.EqualTo(new Distance<int>(1, 2)));
            Assert.That(a * b, Is.EqualTo(new Distance<int>(3, 8)));
            Assert.That(b / a, Is.EqualTo(new Distance<int>(3, 2)));
            Assert.That(a == new Distance<int>(1, 2), Is.True);
            Assert.That(a != b, Is.True);
        });
    }
}
