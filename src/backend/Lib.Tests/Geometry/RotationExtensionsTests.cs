using Lib.Geometry;

namespace Lib.Tests.Geometry;

public class RotationExtensionsTests
{
    [Test]
    public void Sign_ReturnsPositiveNegativeOrZero()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Rotation.Clockwise.Sign(), Is.EqualTo(1));
            Assert.That(Rotation.CounterClockwise.Sign(), Is.EqualTo(-1));
            Assert.That(Rotation.None.Sign(), Is.EqualTo(0));
        });
    }

    [Test]
    public void Invert_SwapsDirectionsAndKeepsNone()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Rotation.Clockwise.Invert(), Is.EqualTo(Rotation.CounterClockwise));
            Assert.That(Rotation.CounterClockwise.Invert(), Is.EqualTo(Rotation.Clockwise));
            Assert.That(Rotation.None.Invert(), Is.EqualTo(Rotation.None));
        });
    }

    [TestCase(1, Rotation.Clockwise)]
    [TestCase(-2, Rotation.CounterClockwise)]
    [TestCase(0, Rotation.None)]
    public void FromSign_MapsNumericSigns(int sign, Rotation expected)
    {
        Assert.That(RotationExtensions.FromSign(sign), Is.EqualTo(expected));
    }

    [Test]
    public void ParseAndToGlyph_RoundTripKnownGlyphs()
    {
        Assert.Multiple(() =>
        {
            Assert.That(RotationExtensions.Parse('R'), Is.EqualTo(Rotation.Clockwise));
            Assert.That(RotationExtensions.Parse('l'), Is.EqualTo(Rotation.CounterClockwise));
            Assert.That(Rotation.Clockwise.ToGlyph(), Is.EqualTo('R'));
            Assert.That(Rotation.None.ToGlyph(), Is.EqualTo('-'));
        });
    }

    [Test]
    public void ApplyTo_RotatesAngleByStep()
    {
        var start = Angle.None;
        var ninety = Rotation.Clockwise.ApplyTo(start);
        var fortyFive = Rotation.Clockwise.ApplyTo(start, Angle.EighthTurn);
        var backToStart = Rotation.CounterClockwise.ApplyTo(ninety);

        Assert.Multiple(() =>
        {
            Assert.That(ninety, Is.EqualTo(Angle.QuarterTurn));
            Assert.That(fortyFive, Is.EqualTo(Angle.EighthTurn));
            Assert.That(backToStart, Is.EqualTo(start));
        });
    }
}
