namespace Lib.Math.Tests;

public class NumberRangeTests
{
    [Test]
    public void Constructor_ValidatesOrder()
    {
        Assert.Throws<InvalidOperationException>(() => new NumberRange<int>(5, 3));
    }

    [Test]
    public void ContainsAndRelations_Work()
    {
        var range = new NumberRange<int>(1, 5);

        Assert.Multiple(() =>
        {
            Assert.That(range.Contains(3), Is.True);
            Assert.That(range.GreaterThan(3).Start, Is.EqualTo(4));
            Assert.That(range.LessThan(3).Stop, Is.EqualTo(3));
            Assert.That(range.IsNonEmpty, Is.True);
            Assert.That(range.IsEmpty, Is.False);
        });
    }

    [Test]
    public void Operators_UseIntersectionAndUnion()
    {
        var a = new NumberRange<int>(1, 5);
        var b = new NumberRange<int>(3, 7);

        Assert.Multiple(() =>
        {
            Assert.That((a | b), Is.EqualTo(a.Intersection(b)));
            Assert.That((a & b), Is.EqualTo(a.Union(b)));
        });
    }

    [Test]
    public void Without_RemovesLeadingOrTrailingSegments()
    {
        var range = new NumberRange<int>(0, 10);

        Assert.Multiple(() =>
        {
            Assert.That(range.Without(new NumberRange<int>(0, 5)), Is.EqualTo(new NumberRange<int>(6, 10)));
            Assert.That(range.Without(new NumberRange<int>(5, 10)), Is.EqualTo(new NumberRange<int>(0, 4)));
        });
    }

    [Test]
    public void StartStopAndComparisons_Work()
    {
        var range = new NumberRange<int>(2, 6);
        var other = new NumberRange<int>(7, 8);

        Assert.Multiple(() =>
        {
            Assert.That(range.StartAt(3), Is.EqualTo(new NumberRange<int>(3, 6)));
            Assert.That(range.StopAt(4), Is.EqualTo(new NumberRange<int>(2, 4)));
            Assert.That(range.IsBefore(other), Is.True);
            Assert.That(other.IsAfter(range), Is.True);
        });
    }

    [Test]
    public void BitwiseNot_ExtendsToBounds()
    {
        var upperOpen = new NumberRange<int>(0, int.MaxValue);
        var lowerOpen = new NumberRange<int>(int.MinValue, 0);

        Assert.Multiple(() =>
        {
            Assert.That((~upperOpen), Is.EqualTo(new NumberRange<int>(int.MinValue, 0)));
            Assert.That((~lowerOpen), Is.EqualTo(new NumberRange<int>(0, int.MaxValue)));
        });
    }

    [Test]
    public void IntersectionsAndUnions_AreValidated()
    {
        var a = new NumberRange<int>(1, 5);
        var b = new NumberRange<int>(4, 8);

        Assert.Multiple(() =>
        {
            Assert.That(a.Intersects(b), Is.True);
            Assert.That(a.Union(b), Is.EqualTo(new NumberRange<int>(1, 8)));
            Assert.That(a.Contains(new NumberRange<int>(2, 4)), Is.True);
            Assert.That(new NumberRange<int>(2, 4).ContainedBy(a), Is.True);
            Assert.That(a.Intersection(b), Is.EqualTo(new NumberRange<int>(4, 5)));
        });
    }

    [Test]
    public void Split_ByCoordinateAndRange()
    {
        var range = new NumberRange<int>(0, 4);

        var splitAtTwo = range.Split(2).ToArray();
        Assert.That(splitAtTwo, Has.Length.EqualTo(2));

        var splitByRange = range.Split(new NumberRange<int>(1, 2)).ToArray();
        Assert.That(splitByRange, Has.Length.EqualTo(3));
    }

    [Test]
    public void EqualityAndStringRepresentation()
    {
        var range = new NumberRange<int>(1, 3);

        Assert.Multiple(() =>
        {
            Assert.That(range, Is.EqualTo(new NumberRange<int>(1, 3)));
            Assert.That(range.GetHashCode(), Is.Not.Zero);
            Assert.That(range.ToString(), Is.EqualTo("[1,3]"));
        });
    }
}

