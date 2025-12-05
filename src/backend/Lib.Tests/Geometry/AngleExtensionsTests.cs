using Lib.Math;
using Lib.Geometry;

namespace Lib.Tests.Geometry;

public class AngleExtensionsTests
{
    [TestCase(0)]
    [TestCase(45)]
    [TestCase(90)]
    [TestCase(180)]
    [TestCase(315)]
    [TestCase(720)]
    [TestCase(-45)]
    public void ToAngleAndToDegrees_RoundTrip(int degrees)
    {
        var normalized = degrees.ToAngle().ToDegrees();
        Assert.That(normalized, Is.EqualTo(MathExtensions.Modulo(degrees, 360)));
    }

    [Test]
    public void ArithmeticOperations_NormalizeBeyondFullTurn()
    {
        var ninety = Angle.QuarterTurn;
        var result = ninety.Add(Angle.HalfTurn | Angle.QuarterTurn);
        var subtract = ninety.Subtract(Angle.HalfTurn);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Angle.None));
            Assert.That(subtract, Is.EqualTo(Angle.QuarterTurn | Angle.HalfTurn));
        });
    }

    [Test]
    public void RotationHelpers_InvokeAddOrSubtract()
    {
        var angle = Angle.EighthTurn;

        Assert.Multiple(() =>
        {
            Assert.That(angle.RotateBy(Angle.QuarterTurn), Is.EqualTo(Angle.QuarterTurn | Angle.EighthTurn));
            Assert.That(angle.RotateClockwise(Angle.QuarterTurn), Is.EqualTo(Angle.QuarterTurn | Angle.EighthTurn));
            Assert.That(angle.RotateCounterClockwise(Angle.QuarterTurn), Is.EqualTo(Angle.HalfTurn | Angle.QuarterTurn | Angle.EighthTurn));
        });
    }

    [Test]
    public void OppositeAndNegate_FlipOrientation()
    {
        var angle = Angle.QuarterTurn;

        Assert.Multiple(() =>
        {
            Assert.That(angle.Opposite(), Is.EqualTo(Angle.QuarterTurn | Angle.HalfTurn));
            Assert.That(angle.Negate(), Is.EqualTo(Angle.QuarterTurn | Angle.HalfTurn));
            Assert.That(angle.Normalize(), Is.EqualTo(angle));
        });
    }

    [Test]
    public void ClassificationMethods_WorkForCardinalAndDiagonal()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Angle.None.IsCardinal(), Is.True);
            Assert.That((Angle.EighthTurn | Angle.QuarterTurn).IsDiagonal(), Is.True);
            Assert.That(Angle.None.IsHorizontal(), Is.True);
            Assert.That(Angle.QuarterTurn.IsVertical(), Is.True);
            Assert.That(Angle.None.IsPerpendicularTo(Angle.QuarterTurn), Is.True);
            Assert.That(Angle.HalfTurn.IsParallelTo(Angle.None), Is.True);
        });
    }

    [Test]
    public void QuadrantAndSigns_AreCalculatedCorrectly()
    {
        var angle = Angle.EighthTurn | Angle.QuarterTurn;
        var oppositeDiagonal = Angle.HalfTurn | Angle.EighthTurn;

        Assert.Multiple(() =>
        {
            Assert.That(angle.GetQuadrant(), Is.EqualTo(2));
            Assert.That(angle.HorizontalSign(), Is.EqualTo(-1));
            Assert.That(angle.VerticalSign(), Is.EqualTo(1));
            Assert.That(oppositeDiagonal.VerticalSign(), Is.EqualTo(-1));
        });
    }

    [Test]
    public void DistanceAndDirection_CaptureShortestRotation()
    {
        var from = Angle.None;
        var to = Angle.HalfTurn;

        Assert.Multiple(() =>
        {
            Assert.That(from.ShortestDistanceTo(to), Is.EqualTo(180));
            Assert.That(to.ShortestDistanceTo(from), Is.EqualTo(-180));
            Assert.That(from.GetRotationDirection(Angle.QuarterTurn), Is.EqualTo(1));
        });
    }

    [Test]
    public void LerpAndStepToward_MoveAlongShortestPath()
    {
        var start = Angle.None;
        var end = Angle.HalfTurn | Angle.QuarterTurn; // 270

        var halfway = start.Lerp(end, 0.5f);
        var stepped = start.StepToward(end, 90);

        Assert.Multiple(() =>
        {
            Assert.That(halfway.ToDegrees(), Is.EqualTo(315));
            Assert.That(stepped, Is.EqualTo(Angle.QuarterTurn | Angle.HalfTurn));
        });
    }

    [Test]
    public void SnapHelpers_RoundToClosestIncrement()
    {
        Assert.Multiple(() =>
        {
            Assert.That(AngleExtensions.SnapToNearest45(10).ToDegrees(), Is.EqualTo(0));
            Assert.That(AngleExtensions.SnapToNearest90(200).ToDegrees(), Is.EqualTo(180));
        });
    }

    [Test]
    public void Enumerations_ReturnExpectedAngles()
    {
        var allAngles = AngleExtensions.GetAllAngles();
        var cardinals = AngleExtensions.GetCardinalAngles();
        var diagonals = AngleExtensions.GetDiagonalAngles();

        Assert.Multiple(() =>
        {
            Assert.That(allAngles, Has.Length.EqualTo(8));
            Assert.That(cardinals.All(a => a.IsCardinal()), Is.True);
            Assert.That(diagonals.All(a => a.IsDiagonal()), Is.True);
        });
    }

    [Test]
    public void TryGetExactAngle_ValidatesMultiplesOfFortyFive()
    {
        Assert.That(AngleExtensions.TryGetExactAngle(135, out var angle), Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(angle, Is.EqualTo(Angle.EighthTurn | Angle.QuarterTurn));
            Assert.That(AngleExtensions.TryGetExactAngle(17, out _), Is.False);
        });
    }
}


