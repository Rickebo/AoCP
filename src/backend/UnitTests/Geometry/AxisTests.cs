using Lib.Enums;

namespace Lib.UnitTests.Geometry;

public class AxisTests
{
    [Test]
    public void AxisEnum_DefinesFlags()
    {
        var both = Axis.X | Axis.Y;

        Assert.Multiple(() =>
        {
            Assert.That(both.HasFlag(Axis.X), Is.True);
            Assert.That(both.HasFlag(Axis.Y), Is.True);
            Assert.That(((int)Axis.None), Is.EqualTo(0));
        });
    }
}
