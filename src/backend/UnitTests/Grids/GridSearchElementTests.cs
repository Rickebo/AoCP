using Lib.Geometry;
using Lib.Grids;

namespace Lib.Grids.Tests;

public class GridSearchElementTests
{
    [Test]
    public void Equality_DependsOnCoordinateOnly()
    {
        var coord = new IntegerCoordinate<int>(1, 2);
        var first = new GridSearchElement<char, int>(coord, 'a');
        var second = new GridSearchElement<char, int>(coord, 'b');

        Assert.Multiple(() =>
        {
            Assert.That(first.Equals(second), Is.True);
            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
            Assert.That(first.ToString(), Is.EqualTo(coord.ToString()));
        });
    }
}

