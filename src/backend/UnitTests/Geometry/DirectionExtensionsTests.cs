namespace Lib.Geometry.Tests;

public class DirectionExtensionsTests
{
    [Test]
    public void HorizontalAndVerticalCollections_AreExpected()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DirectionExtensions.Horizontal, Is.EqualTo(new[] { Direction.West, Direction.East }).AsCollection);
            Assert.That(DirectionExtensions.Vertical, Is.EqualTo(new[] { Direction.North, Direction.South }).AsCollection);
        });
    }

    [Test]
    public void ToGlyph_MapsKnownDirections()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.ToGlyph(), Is.EqualTo('N'));
            Assert.That(Direction.NorthWest.ToGlyph(), Is.EqualTo('J'));
            Assert.That((Direction.East | Direction.West).ToGlyph(), Is.EqualTo('-'));
        });
    }

    [Test]
    public void Max_ReturnsLargestDefinedValue()
    {
        Assert.That(DirectionExtensions.Max, Is.EqualTo(Direction.SouthWest));
    }

    [Test]
    public void Opposite_ReturnsInvertedDirection()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.Opposite(), Is.EqualTo(Direction.South));
            Assert.That(Direction.NorthEast.Opposite(), Is.EqualTo(Direction.SouthWest));
            Assert.That((Direction.North | Direction.South | Direction.East).Opposite(),
                Is.EqualTo(Direction.North | Direction.South | Direction.West));
        });
    }

    [Test]
    public void Parse_ConvertsFromGlyphs()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DirectionExtensions.Parse('>'), Is.EqualTo(Direction.East));
            Assert.That(DirectionExtensions.Parse('|'), Is.EqualTo(Direction.North | Direction.South));
            Assert.That(DirectionExtensions.Parse(' '), Is.EqualTo(Direction.None));
        });
    }

    [Test]
    public void FlipXAndFlipY_SwapAxes()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.East.FlipX(), Is.EqualTo(Direction.West));
            Assert.That(Direction.North.FlipY(), Is.EqualTo(Direction.South));
            Assert.That((Direction.North | Direction.South).FlipY(), Is.EqualTo(Direction.North | Direction.South));
        });
    }

    [Test]
    public void All_ReturnsAllEnumValuesIncludingAliases()
    {
        var distinct = DirectionExtensions.All().Distinct().ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(DirectionExtensions.All(), Has.Length.GreaterThan(8));
            Assert.That(distinct, Has.Length.EqualTo(9)); // unique flag combinations
        });
    }

    [Test]
    public void RotationHelpers_TranslateCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.Right(), Is.EqualTo(Direction.East));
            Assert.That(Direction.East.Left(), Is.EqualTo(Direction.North));
            Assert.That(Direction.North.ToAngle(), Is.EqualTo(Angle.None));
            Assert.That((Angle.HalfTurn | Angle.QuarterTurn).ToDirection(), Is.EqualTo(Direction.West));
        });
    }

    [Test]
    public void Rotate_UsesRotationDirectionAndAmount()
    {
        var rotated = Direction.North.Rotate(Rotation.Clockwise, Angle.QuarterTurn);
        var rotatedBack = rotated.Rotate(Rotation.CounterClockwise, Angle.QuarterTurn);

        Assert.Multiple(() =>
        {
            Assert.That(rotated, Is.EqualTo(Direction.East));
            Assert.That(rotatedBack, Is.EqualTo(Direction.North));
        });
    }

    [Test]
    public void Neighbours_ReturnsLeftSelfRight()
    {
        var neighbours = Direction.East.Neighbours().ToArray();

        Assert.That(neighbours, Is.EqualTo(new[] { Direction.North, Direction.East, Direction.South }).AsCollection);
    }

    [Test]
    public void Has_ChecksIfDirectionContainsComponent()
    {
        Assert.Multiple(() =>
        {
            Assert.That((Direction.NorthEast).Has(Direction.North), Is.True);
            Assert.That(Direction.West.Has(Direction.East), Is.False);
        });
    }

    [Test]
    public void DirectionFromOrigin_DerivesFromCoordinateSigns()
    {
        var coord = new IntegerCoordinate<int>(-1, 2);
        var direction = coord.DirectionFromOrigin();

        Assert.That(direction, Is.EqualTo(Direction.North | Direction.West));
    }

    [Test]
    public void ToCoordinate_MapsDirectionToUnitVector()
    {
        var coord = Direction.SouthWest.ToCoordinate<int>();

        Assert.Multiple(() =>
        {
            Assert.That(coord.X, Is.EqualTo(-1));
            Assert.That(coord.Y, Is.EqualTo(-1));
        });
    }

    [Test]
    public void OrdinalAndArrow_MatchDirection()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.Ordinal(), Is.EqualTo(0));
            Assert.That(Direction.South.Arrow(), Is.EqualTo('v'));
            Assert.That(DirectionExtensions.FromOrdinal(2), Is.EqualTo(Direction.South));
        });
    }

    [Test]
    public void OrientationChecks_RequireSubset()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.IsVertical(), Is.True);
            Assert.That(Direction.NorthEast.IsVertical(), Is.False);
            Assert.That(Direction.West.IsHorizontal(), Is.True);
            Assert.That(Direction.NorthWest.IsHorizontal(), Is.False);
        });
    }
}

