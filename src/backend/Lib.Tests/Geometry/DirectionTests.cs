using Lib.Geometry;

namespace Lib.Tests.Geometry;

public class DirectionTests
{
    [Test]
    public void CompositeDirections_CombineFlags()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.NorthEast, Is.EqualTo(Direction.North | Direction.East));
            Assert.That(Direction.SouthWest, Is.EqualTo(Direction.South | Direction.West));
            Assert.That(Direction.Up, Is.EqualTo(Direction.North));
            Assert.That(Direction.Left, Is.EqualTo(Direction.West));
        });
    }

    [Test]
    public void OrdinalValues_AreSequentialFlags()
    {
        Assert.Multiple(() =>
        {
            Assert.That((int)Direction.North, Is.EqualTo(1));
            Assert.That((int)Direction.East, Is.EqualTo(2));
            Assert.That((int)Direction.South, Is.EqualTo(4));
            Assert.That((int)Direction.West, Is.EqualTo(8));
        });
    }
}


