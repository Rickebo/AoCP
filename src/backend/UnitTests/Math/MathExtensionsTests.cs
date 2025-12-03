
namespace Lib.Math.Tests;

public class MathExtensionsTests
{
    [Test]
    public void Remainder_IsAlwaysNonNegative()
    {
        Assert.Multiple(() =>
        {
            Assert.That(MathExtensions.Remainder(5, 3), Is.EqualTo(2));
            Assert.That(MathExtensions.Remainder(-1, 3), Is.EqualTo(2));
        });
    }

    [Test]
    public void Modulo_HandlesNegativeNumbers()
    {
        Assert.Multiple(() =>
        {
            Assert.That(MathExtensions.Modulo(-5, 3), Is.EqualTo(1));
            Assert.That(MathExtensions.Modulo(5, -3), Is.EqualTo(-1));
        });
    }

    [Test]
    public void HexToFloat_ValidatesInput()
    {
        Assert.That(MathExtensions.HexToFloat("FF"), Is.EqualTo(1f));
        Assert.Throws<ArgumentException>(() => MathExtensions.HexToFloat("zz"));
        Assert.Throws<ArgumentException>(() => MathExtensions.HexToFloat(""));
        Assert.Throws<NullReferenceException>(() => MathExtensions.HexToFloat(null!));
    }

    [Test]
    public void Ten_CeilLog10_And_Pow10_Work()
    {
        Assert.Multiple(() =>
        {
            Assert.That(MathExtensions.Ten<int>(), Is.EqualTo(10));
            Assert.That(MathExtensions.CeilLog10(999), Is.EqualTo(3));
            Assert.That(MathExtensions.Pow10(3), Is.EqualTo(1000));
        });
    }
}

