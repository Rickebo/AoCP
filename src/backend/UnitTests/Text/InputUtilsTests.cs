using Lib.Text;

namespace Lib.Text.Tests;

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
            CollectionAssert.AreEqual(new[] { "a" }, groups[0]);
            CollectionAssert.AreEqual(new[] { "b", "c" }, groups[1]);
        });
    }

    [Test]
    public void ExtractIntegers_ReturnsSignedNumbers()
    {
        var numbers = InputUtils.ExtractIntegers("Value -2 and 15");

        CollectionAssert.AreEqual(new long[] { -2, 15 }, numbers);
    }

    [Test]
    public void SplitAndTrim_RemovesWhitespaceAndEmpties()
    {
        var parts = InputUtils.SplitAndTrim(" a , b ,, c ", ',');

        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, parts);
    }
}

