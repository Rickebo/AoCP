
namespace Lib.Text.Tests;

public class StringExtensionsTests
{
    [Test]
    public void SplitLines_SplitsOnNewlines()
    {
        var lines = "a\nb\r\nc".SplitLines();

        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, lines);
    }

    [Test]
    public void SplitBy_UsesProvidedSeparators()
    {
        var parts = "a|b,c".SplitBy(new[] { "|", "," }, StringSplitOptions.RemoveEmptyEntries);

        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, parts);
    }
}

