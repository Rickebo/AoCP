using Lib.Geometry;

namespace Lib.Tests.Geometry;

public class AngleTests
{
    [Test]
    public void AngleEnum_HasExpectedFlagValues()
    {
        Assert.Multiple(() =>
        {
            Assert.That((int)Angle.None, Is.EqualTo(0));
            Assert.That((int)Angle.EighthTurn, Is.EqualTo(1));
            Assert.That((int)Angle.QuarterTurn, Is.EqualTo(2));
            Assert.That((int)Angle.HalfTurn, Is.EqualTo(4));
        });

        Assert.That(Angle.EighthTurn | Angle.QuarterTurn, Is.EqualTo(Angle.QuarterTurn | Angle.EighthTurn));
    }
}


