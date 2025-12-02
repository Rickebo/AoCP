using Lib.Coordinate;
using Lib.Enums;
using Lib.Extensions;

namespace UnitTests;

[TestFixture]
public class DirectionTests
{
    [Test]
    public void Direction_BaseValues_AreExpected()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.None, Is.EqualTo((Direction)0));
            Assert.That(Direction.North, Is.EqualTo((Direction)1));
            Assert.That(Direction.East, Is.EqualTo((Direction)2));
            Assert.That(Direction.South, Is.EqualTo((Direction)4));
            Assert.That(Direction.West, Is.EqualTo((Direction)8));
        });
    }

    [Test]
    public void Direction_CompositeValues_AreBitwiseCombinations()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.NorthEast, Is.EqualTo(Direction.North | Direction.East));
            Assert.That(Direction.NorthWest, Is.EqualTo(Direction.North | Direction.West));
            Assert.That(Direction.SouthEast, Is.EqualTo(Direction.South | Direction.East));
            Assert.That(Direction.SouthWest, Is.EqualTo(Direction.South | Direction.West));
        });
    }

    [Test]
    public void Direction_Aliases_MapToUnderlyingValues()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.Up, Is.EqualTo(Direction.North));
            Assert.That(Direction.Right, Is.EqualTo(Direction.East));
            Assert.That(Direction.Down, Is.EqualTo(Direction.South));
            Assert.That(Direction.Left, Is.EqualTo(Direction.West));
            Assert.That(Direction.UpRight, Is.EqualTo(Direction.NorthEast));
            Assert.That(Direction.UpLeft, Is.EqualTo(Direction.NorthWest));
            Assert.That(Direction.DownRight, Is.EqualTo(Direction.SouthEast));
            Assert.That(Direction.DownLeft, Is.EqualTo(Direction.SouthWest));
        });
    }

    [Test]
    public void Direction_StaticArrays_ContainExpectedMembers()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DirectionExtensions.Horizontal,
                Is.EqualTo(new[] { Direction.West, Direction.East }));
            Assert.That(DirectionExtensions.Vertical,
                Is.EqualTo(new[] { Direction.North, Direction.South }));
            Assert.That(DirectionExtensions.Cardinals,
                Is.EqualTo(new[] { Direction.North, Direction.East, Direction.South, Direction.West }));
            Assert.That(DirectionExtensions.Diagonals,
                Is.EqualTo(new[] { Direction.NorthEast, Direction.SouthEast, Direction.SouthWest, Direction.NorthWest }));
        });
    }

    [Test]
    public void Direction_All_ReturnsAllEnumValuesInOrder()
    {
        var expected = Enum.GetValues<Direction>();
        Assert.That(DirectionExtensions.All(), Is.EqualTo(expected));
    }

    [Test]
    public void Max_IsLargestDefinedDirectionValue()
    {
        Assert.That(DirectionExtensions.Max, Is.EqualTo(Direction.DownLeft));
    }

    [Test]
    public void ToGlyph_ReturnsExpectedCharacters_ForSupportedDirections()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.ToGlyph(), Is.EqualTo('N'));
            Assert.That(Direction.East.ToGlyph(), Is.EqualTo('E'));
            Assert.That(Direction.South.ToGlyph(), Is.EqualTo('S'));
            Assert.That(Direction.West.ToGlyph(), Is.EqualTo('W'));
            Assert.That(Direction.NorthWest.ToGlyph(), Is.EqualTo('J'));
            Assert.That(Direction.NorthEast.ToGlyph(), Is.EqualTo('L'));
            Assert.That(Direction.SouthWest.ToGlyph(), Is.EqualTo('7'));
            Assert.That(Direction.SouthEast.ToGlyph(), Is.EqualTo('F'));
            Assert.That((Direction.East | Direction.South | Direction.West).ToGlyph(), Is.EqualTo('\u2533'));
            Assert.That((Direction.East | Direction.North | Direction.West).ToGlyph(), Is.EqualTo('\u253b'));
            Assert.That((Direction.South | Direction.North | Direction.West).ToGlyph(), Is.EqualTo('\u252b'));
            Assert.That((Direction.South | Direction.North | Direction.East).ToGlyph(), Is.EqualTo('\u2523'));
            Assert.That((Direction.NorthEast | Direction.SouthWest).ToGlyph(), Is.EqualTo('+'));
            Assert.That((Direction.North | Direction.South).ToGlyph(), Is.EqualTo('|'));
            Assert.That((Direction.East | Direction.West).ToGlyph(), Is.EqualTo('-'));
            Assert.That(Direction.None.ToGlyph(), Is.EqualTo('*'));
        });
    }




    [Test]
    public void Opposite_ReturnsExpectedDirection()
    {
        var expectations = new Dictionary<Direction, Direction>
        {
            { Direction.North, Direction.South },
            { Direction.NorthEast, Direction.SouthWest },
            { Direction.East, Direction.West },
            { Direction.SouthEast, Direction.NorthWest },
            { Direction.South, Direction.North },
            { Direction.SouthWest, Direction.NorthEast },
            { Direction.West, Direction.East },
            { Direction.NorthWest, Direction.SouthEast }
        };

        foreach (var (direction, opposite) in expectations)
        {
            Assert.Multiple(() =>
            {
                Assert.That(direction.Opposite(), Is.EqualTo(opposite));
                Assert.That(direction.Opposite().Opposite(), Is.EqualTo(direction));
            });
        }
    }

    [Test]
    public void Opposite_None_Throws()
    {
        Assert.That(() => Direction.None.Opposite(), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Parse_ReturnsExpectedDirections()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DirectionExtensions.Parse('-'), Is.EqualTo(Direction.East | Direction.West));
            Assert.That(DirectionExtensions.Parse('|'), Is.EqualTo(Direction.North | Direction.South));
            Assert.That(DirectionExtensions.Parse('+'), Is.EqualTo(Direction.North | Direction.East | Direction.South | Direction.West));
            Assert.That(DirectionExtensions.Parse('>'), Is.EqualTo(Direction.East));
            Assert.That(DirectionExtensions.Parse('<'), Is.EqualTo(Direction.West));
            Assert.That(DirectionExtensions.Parse('^'), Is.EqualTo(Direction.North));
            Assert.That(DirectionExtensions.Parse('v'), Is.EqualTo(Direction.South));
            Assert.That(DirectionExtensions.Parse('7'), Is.EqualTo(Direction.South | Direction.West));
            Assert.That(DirectionExtensions.Parse('F'), Is.EqualTo(Direction.East | Direction.South));
            Assert.That(DirectionExtensions.Parse('L'), Is.EqualTo(Direction.North | Direction.East));
            Assert.That(DirectionExtensions.Parse('J'), Is.EqualTo(Direction.North | Direction.West));
            Assert.That(DirectionExtensions.Parse('N'), Is.EqualTo(Direction.North));
            Assert.That(DirectionExtensions.Parse('E'), Is.EqualTo(Direction.East));
            Assert.That(DirectionExtensions.Parse('S'), Is.EqualTo(Direction.South));
            Assert.That(DirectionExtensions.Parse('W'), Is.EqualTo(Direction.West));
            Assert.That(DirectionExtensions.Parse(' '), Is.EqualTo(Direction.None));
            Assert.That(DirectionExtensions.Parse('.'), Is.EqualTo(Direction.None));
        });
    }

    [Test]
    public void Parse_InvalidCharacter_Throws()
    {
        Assert.That(() => DirectionExtensions.Parse('X'), Throws.ArgumentException);
    }

    [Test]
    public void FlipX_FlipsHorizontalComponentsOnly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.East.FlipX(), Is.EqualTo(Direction.West));
            Assert.That(Direction.West.FlipX(), Is.EqualTo(Direction.East));
            Assert.That(Direction.NorthEast.FlipX(), Is.EqualTo(Direction.NorthWest));
            Assert.That(Direction.SouthWest.FlipX(), Is.EqualTo(Direction.SouthEast));
            Assert.That((Direction.East | Direction.West).FlipX(), Is.EqualTo(Direction.East | Direction.West));
            Assert.That(Direction.North.FlipX(), Is.EqualTo(Direction.North));
        });
    }

    [Test]
    public void FlipY_FlipsVerticalComponentsOnly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.FlipY(), Is.EqualTo(Direction.South));
            Assert.That(Direction.South.FlipY(), Is.EqualTo(Direction.North));
            Assert.That(Direction.NorthEast.FlipY(), Is.EqualTo(Direction.SouthEast));
            Assert.That(Direction.SouthWest.FlipY(), Is.EqualTo(Direction.NorthWest));
            Assert.That((Direction.North | Direction.South).FlipY(), Is.EqualTo(Direction.North | Direction.South));
            Assert.That(Direction.East.FlipY(), Is.EqualTo(Direction.East));
        });
    }

    [Test]
    public void Right_And_Left_RotateCardinals()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.Right(), Is.EqualTo(Direction.East));
            Assert.That(Direction.East.Right(), Is.EqualTo(Direction.South));
            Assert.That(Direction.South.Right(), Is.EqualTo(Direction.West));
            Assert.That(Direction.West.Right(), Is.EqualTo(Direction.North));
            Assert.That(Direction.North.Left(), Is.EqualTo(Direction.West));
            Assert.That(Direction.West.Left(), Is.EqualTo(Direction.South));
            Assert.That(Direction.South.Left(), Is.EqualTo(Direction.East));
            Assert.That(Direction.East.Left(), Is.EqualTo(Direction.North));
        });
    }

    [Test]
    public void Rotate_RespectsRotationAndAngle()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.None.Rotate(Rotation.Clockwise), Is.EqualTo(Direction.None));
            Assert.That(Direction.North.Rotate(Rotation.None), Is.EqualTo(Direction.North));
            Assert.That(Direction.North.Rotate(Rotation.Clockwise), Is.EqualTo(Direction.East));
            Assert.That(Direction.North.Rotate(Rotation.CounterClockwise), Is.EqualTo(Direction.West));
            Assert.That(Direction.North.Rotate(Rotation.Clockwise, Angle.EighthTurn), Is.EqualTo(Direction.NorthEast));
            Assert.That(Direction.North.Rotate(Rotation.CounterClockwise, Angle.EighthTurn), Is.EqualTo(Direction.NorthWest));
        });
    }

    [Test]
    public void Neighbours_DefaultAngle_ReturnsLeftSelfRight()
    {
        var neighbours = Direction.North.Neighbours().ToArray();
        Assert.That(neighbours, Is.EqualTo(new[] { Direction.West, Direction.North, Direction.East }));
    }

    [Test]
    public void Neighbours_CustomAngle_UsesProvidedSpacing()
    {
        var neighbours = Direction.NorthEast.Neighbours(Angle.EighthTurn).ToArray();
        Assert.That(neighbours, Is.EqualTo(new[] { Direction.North, Direction.NorthEast, Direction.East }));
    }

    [Test]
    public void Has_ChecksFlagPresence()
    {
        Assert.Multiple(() =>
        {
            Assert.That((Direction.North | Direction.East).Has(Direction.North), Is.True);
            Assert.That((Direction.North | Direction.East).Has(Direction.East), Is.True);
            Assert.That((Direction.North | Direction.East).Has(Direction.South), Is.False);
            Assert.That(Direction.North.Has(Direction.East), Is.False);
        });
    }

    [Test]
    public void Direction_FromCoordinateComponents()
    {
        static Direction From(IntegerCoordinate<int> coordinate)
        {
            ICoordinate<IntegerCoordinate<int>, int> asInterface = coordinate;
            return asInterface.Direction();
        }

        Assert.Multiple(() =>
        {
            Assert.That(From(new IntegerCoordinate<int>(1, 1)), Is.EqualTo(Direction.North | Direction.East));
            Assert.That(From(new IntegerCoordinate<int>(-1, 1)), Is.EqualTo(Direction.North | Direction.West));
            Assert.That(From(new IntegerCoordinate<int>(1, -1)), Is.EqualTo(Direction.South | Direction.East));
            Assert.That(From(new IntegerCoordinate<int>(-1, -1)), Is.EqualTo(Direction.South | Direction.West));
            Assert.That(From(new IntegerCoordinate<int>(1, 0)), Is.EqualTo(Direction.East));
            Assert.That(From(new IntegerCoordinate<int>(-1, 0)), Is.EqualTo(Direction.West));
            Assert.That(From(new IntegerCoordinate<int>(0, 1)), Is.EqualTo(Direction.North));
            Assert.That(From(new IntegerCoordinate<int>(0, -1)), Is.EqualTo(Direction.South));
            Assert.That(From(new IntegerCoordinate<int>(0, 0)), Is.EqualTo(Direction.None));
        });
    }

    [Test]
    public void ToCoordinate_ProducesExpectedIntegerCoordinate()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.ToCoordinate<int>(), Is.EqualTo(new IntegerCoordinate<int>(0, 1)));
            Assert.That(Direction.South.ToCoordinate<int>(), Is.EqualTo(new IntegerCoordinate<int>(0, -1)));
            Assert.That(Direction.East.ToCoordinate<int>(), Is.EqualTo(new IntegerCoordinate<int>(1, 0)));
            Assert.That(Direction.West.ToCoordinate<int>(), Is.EqualTo(new IntegerCoordinate<int>(-1, 0)));
            Assert.That(Direction.NorthEast.ToCoordinate<int>(), Is.EqualTo(new IntegerCoordinate<int>(1, 1)));
            Assert.That(Direction.SouthWest.ToCoordinate<int>(), Is.EqualTo(new IntegerCoordinate<int>(-1, -1)));
            Assert.That((Direction.East | Direction.West).ToCoordinate<int>(), Is.EqualTo(IntegerCoordinate<int>.Zero));
            Assert.That(Direction.None.ToCoordinate<int>(), Is.EqualTo(IntegerCoordinate<int>.Zero));
        });
    }

    [Test]
    public void ToAngle_ReturnsExpectedAngles_ForAliasesToo()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.ToAngle(), Is.EqualTo(Angle.None));
            Assert.That(Direction.Up.ToAngle(), Is.EqualTo(Angle.None));
            Assert.That(Direction.NorthEast.ToAngle(), Is.EqualTo(Angle.EighthTurn));
            Assert.That(Direction.UpRight.ToAngle(), Is.EqualTo(Angle.EighthTurn));
            Assert.That(Direction.East.ToAngle(), Is.EqualTo(Angle.QuarterTurn));
            Assert.That(Direction.Right.ToAngle(), Is.EqualTo(Angle.QuarterTurn));
            Assert.That(Direction.SouthEast.ToAngle(), Is.EqualTo(Angle.QuarterTurn | Angle.EighthTurn));
            Assert.That(Direction.DownRight.ToAngle(), Is.EqualTo(Angle.QuarterTurn | Angle.EighthTurn));
            Assert.That(Direction.South.ToAngle(), Is.EqualTo(Angle.HalfTurn));
            Assert.That(Direction.Down.ToAngle(), Is.EqualTo(Angle.HalfTurn));
            Assert.That(Direction.SouthWest.ToAngle(), Is.EqualTo(Angle.HalfTurn | Angle.EighthTurn));
            Assert.That(Direction.DownLeft.ToAngle(), Is.EqualTo(Angle.HalfTurn | Angle.EighthTurn));
            Assert.That(Direction.West.ToAngle(), Is.EqualTo(Angle.HalfTurn | Angle.QuarterTurn));
            Assert.That(Direction.Left.ToAngle(), Is.EqualTo(Angle.HalfTurn | Angle.QuarterTurn));
            Assert.That(Direction.NorthWest.ToAngle(), Is.EqualTo(Angle.HalfTurn | Angle.QuarterTurn | Angle.EighthTurn));
            Assert.That(Direction.UpLeft.ToAngle(), Is.EqualTo(Angle.HalfTurn | Angle.QuarterTurn | Angle.EighthTurn));
        });
    }

    [Test]
    public void ToDirection_MapsAnglesBackToDirections()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Angle.None.ToDirection(), Is.EqualTo(Direction.North));
            Assert.That(Angle.EighthTurn.ToDirection(), Is.EqualTo(Direction.NorthEast));
            Assert.That(Angle.QuarterTurn.ToDirection(), Is.EqualTo(Direction.East));
            Assert.That((Angle.QuarterTurn | Angle.EighthTurn).ToDirection(), Is.EqualTo(Direction.SouthEast));
            Assert.That(Angle.HalfTurn.ToDirection(), Is.EqualTo(Direction.South));
            Assert.That((Angle.HalfTurn | Angle.EighthTurn).ToDirection(), Is.EqualTo(Direction.SouthWest));
            Assert.That((Angle.HalfTurn | Angle.QuarterTurn).ToDirection(), Is.EqualTo(Direction.West));
            Assert.That((Angle.HalfTurn | Angle.QuarterTurn | Angle.EighthTurn).ToDirection(), Is.EqualTo(Direction.NorthWest));
        });
    }




    [Test]
    public void Ordinal_And_FromOrdinal_WorkForCardinals()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.Ordinal(), Is.EqualTo(0));
            Assert.That(Direction.East.Ordinal(), Is.EqualTo(1));
            Assert.That(Direction.South.Ordinal(), Is.EqualTo(2));
            Assert.That(Direction.West.Ordinal(), Is.EqualTo(3));
            Assert.That(DirectionExtensions.FromOrdinal(0), Is.EqualTo(Direction.North));
            Assert.That(DirectionExtensions.FromOrdinal(1), Is.EqualTo(Direction.East));
            Assert.That(DirectionExtensions.FromOrdinal(2), Is.EqualTo(Direction.South));
            Assert.That(DirectionExtensions.FromOrdinal(3), Is.EqualTo(Direction.West));
        });
    }

    [Test]
    public void Ordinal_InvalidDirection_Throws()
    {
        Assert.That(() => Direction.NorthEast.Ordinal(), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void FromOrdinal_InvalidValue_Throws()
    {
        Assert.Multiple(() =>
        {
            Assert.That(() => DirectionExtensions.FromOrdinal(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => DirectionExtensions.FromOrdinal(4), Throws.TypeOf<ArgumentOutOfRangeException>());
        });
    }

    [Test]
    public void Arrow_ReturnsExpectedGlyphs()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.East.Arrow(), Is.EqualTo('>'));
            Assert.That(Direction.West.Arrow(), Is.EqualTo('<'));
            Assert.That(Direction.North.Arrow(), Is.EqualTo('^'));
            Assert.That(Direction.South.Arrow(), Is.EqualTo('v'));
        });
    }

    [Test]
    public void Arrow_InvalidDirection_Throws()
    {
        Assert.That(() => Direction.NorthEast.Arrow(), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void IsVertical_And_IsHorizontal_CheckDirectionalSubset()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Direction.North.IsVertical(), Is.True);
            Assert.That(Direction.South.IsVertical(), Is.True);
            Assert.That((Direction.North | Direction.South).IsVertical(), Is.True);
            Assert.That(Direction.East.IsVertical(), Is.False);
            Assert.That(Direction.NorthEast.IsVertical(), Is.False);
            Assert.That(Direction.East.IsHorizontal(), Is.True);
            Assert.That(Direction.West.IsHorizontal(), Is.True);
            Assert.That((Direction.East | Direction.West).IsHorizontal(), Is.True);
            Assert.That(Direction.North.IsHorizontal(), Is.False);
            Assert.That(Direction.NorthEast.IsHorizontal(), Is.False);
            Assert.That(Direction.None.IsVertical(), Is.False);
            Assert.That(Direction.None.IsHorizontal(), Is.False);
        });
    }
}
