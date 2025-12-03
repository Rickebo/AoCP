
namespace Lib.Math.Tests;

public class NumberExtensionsTests
{
    [Test]
    public void Clamp_WithExplicitRange()
    {
        Assert.Multiple(() =>
        {
            Assert.That(5.Clamp(0, 3), Is.EqualTo(3));
            Assert.That((-1).Clamp(0, 3), Is.EqualTo(0));
        });
    }

    [Test]
    public void Clamp_WithOptionalMinMaxUsesTypeBounds()
    {
        Assert.Multiple(() =>
        {
            Assert.That(5.Clamp(min: 1), Is.EqualTo(5));
            Assert.That((-10).Clamp<int>(min: 0), Is.EqualTo(0));
        });
    }
}

