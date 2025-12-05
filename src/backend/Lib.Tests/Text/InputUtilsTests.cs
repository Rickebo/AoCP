using Lib.Text;

namespace Lib.Tests.Text;

public class InputUtilsTests
{
    [Test]
    public void SplitOnBlankLines_GroupsSections()
    {
        var lines = new[] { "a", "", "b", "c", "", "" };
        var groups = InputUtils.SplitOnBlankLines(lines);

        Assert.Multiple(() =>
        {
            Assert.That(groups.Count, Is.EqualTo(2));
            Assert.That(groups[0], Is.EqualTo(new[] { "a" }).AsCollection);
            Assert.That(groups[1], Is.EqualTo(new[] { "b", "c" }).AsCollection);
        });
    }

    [Test]
    public void ExtractIntegers_ReturnsSignedNumbers()
    {
        var numbers = InputUtils.ExtractIntegers("Value -2 and 15");

        Assert.That(numbers, Is.EqualTo(new long[] { -2, 15 }).AsCollection);
    }

    [Test]
    public void SplitAndTrim_RemovesWhitespaceAndEmpties()
    {
        var parts = InputUtils.SplitAndTrim(" a , b ,, c ", ',');

        Assert.That(parts, Is.EqualTo(new[] { "a", "b", "c" }).AsCollection);
    }
}


