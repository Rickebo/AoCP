using Lib.Geometry;

namespace Lib.Tests.Geometry;

public class ICoordinateTests
{
    [Test]
    public void ManhattanLength_DefaultsToComponentSum()
    {
        Assert.That(((ICoordinate<Coordinate<int>, int>)new Coordinate<int>(2, 3)).ManhattanLength(), Is.EqualTo(5));
    }
}


