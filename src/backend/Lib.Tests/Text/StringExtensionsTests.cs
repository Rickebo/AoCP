using Lib.Text;

namespace Lib.Tests.Text;
public class StringExtensionsTests
{
    [Test]
    public void SplitLines_SplitsOnNewlines()
    {
        var lines = "a\nb\r\nc".SplitLines();

        Assert.That(lines, Is.EqualTo(new[] { "a", "b", "c" }).AsCollection);
    }

    [Test]
    public void SplitBy_UsesProvidedSeparators()
    {
        var parts = "a|b,c".SplitBy(new[] { "|", "," }, StringSplitOptions.RemoveEmptyEntries);

        Assert.That(parts, Is.EqualTo(new[] { "a", "b", "c" }).AsCollection);
    }
}


