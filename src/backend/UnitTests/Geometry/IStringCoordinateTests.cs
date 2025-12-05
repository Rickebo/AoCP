namespace Lib.Geometry.Tests;

public class IStringCoordinateTests
{
    [Test]
    public void GetStringCoordinates_ReturnComponentStrings()
    {
        IStringCoordinate coordinate = new Coordinate<int>(5, 6);

        Assert.Multiple(() =>
        {
            Assert.That(coordinate.GetStringX(), Is.EqualTo("5"));
            Assert.That(coordinate.GetStringY(), Is.EqualTo("6"));
        });
    }
}

