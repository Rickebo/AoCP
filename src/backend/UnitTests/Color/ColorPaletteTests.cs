using Lib.Color;
using ColorStruct = Lib.Color.Color;
using ColorConstants = Lib.Color.Colors;

namespace Lib.UnitTests.Coloring;

public class ColorPaletteTests
{
    [Test]
    public void ColorHex_DefinesExpectedCoreValues()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ColorHex.Black, Is.EqualTo("#000000FF"));
            Assert.That(ColorHex.White, Is.EqualTo("#FFFFFFFF"));
            Assert.That(ColorHex.Transparent, Is.EqualTo("#FFFFFF00"));
        });
    }

    [Test]
    public void Colors_ParseHexValuesCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ColorConstants.Red, Is.EqualTo(ColorStruct.Parse("#FF0000FF")));
            Assert.That(ColorConstants.Green, Is.EqualTo(ColorStruct.Parse("#008000FF")));
            Assert.That(ColorConstants.Blue, Is.EqualTo(ColorStruct.Parse("#0000FFFF")));
            Assert.That(ColorConstants.Transparent, Is.EqualTo(ColorStruct.Parse(ColorHex.Transparent)));
        });
    }
}
