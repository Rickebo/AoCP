using Lib.Coordinate;

namespace Lib.UnitTests.Geometry;

public class ICoordinateTests
{
    [Test]
    public void ManhattanLength_DefaultsToComponentSum()
    {
        ICoordinate<Coordinate<int>, int> coordinate = new Coordinate<int>(2, 3);

        Assert.That(coordinate.ManhattanLength(), Is.EqualTo(5));
    }
}
