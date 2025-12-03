using Lib.Coordinate;
using Lib.Geometry;

namespace Lib.UnitTests.Geometry;

public class LineHelpersTests
{
    [Test]
    public void EnumerateSegment_HandlesStraightAndDiagonalSegments()
    {
        var horizontal = LineHelpers.EnumerateSegment(new Coordinate<int>(0, 0), new Coordinate<int>(3, 0)).ToArray();
        var diagonal = LineHelpers.EnumerateSegment(new Coordinate<int>(0, 0), new Coordinate<int>(2, 2)).ToArray();

        CollectionAssert.AreEqual(
            new[]
            {
                new Coordinate<int>(0, 0),
                new Coordinate<int>(1, 0),
                new Coordinate<int>(2, 0),
                new Coordinate<int>(3, 0)
            },
            horizontal);

        CollectionAssert.AreEqual(
            new[]
            {
                new Coordinate<int>(0, 0),
                new Coordinate<int>(1, 1),
                new Coordinate<int>(2, 2)
            },
            diagonal);

        Assert.Throws<ArgumentException>(() =>
            LineHelpers.EnumerateSegment(new Coordinate<int>(0, 0), new Coordinate<int>(2, 1)).ToArray());
    }

    [Test]
    public void ManhattanDistance_ComputesAbsoluteDifference()
    {
        Assert.Multiple(() =>
        {
            Assert.That(LineHelpers.ManhattanDistance(new Coordinate<int>(1, 1), new Coordinate<int>(-1, 2)), Is.EqualTo(3));
            Assert.That(LineHelpers.ManhattanDistance(new Coordinate3D<int>(1, 2, 3), new Coordinate3D<int>(-1, 2, 0)), Is.EqualTo(5));
        });
    }
}
