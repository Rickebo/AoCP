using Lib.Enums;

namespace Lib.UnitTests.Geometry;

public class RotationTests
{
    [Test]
    public void RotationEnum_HasExpectedValues()
    {
        Assert.Multiple(() =>
        {
            Assert.That((int)Rotation.None, Is.EqualTo(0));
            Assert.That((int)Rotation.Clockwise, Is.EqualTo(1));
            Assert.That((int)Rotation.CounterClockwise, Is.EqualTo(2));
        });
    }
}
