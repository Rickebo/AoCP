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

    [Test]
    public void SplitToLineGroups_DefaultPredicateGroupsByEmptyLines()
    {
        var input = "a\n\nb\nc\n\n";

        var groups = input.SplitToLineGroups();

        Assert.Multiple(() =>
        {
            Assert.That(groups, Has.Length.EqualTo(2));
            Assert.That(groups[0], Is.EqualTo(new[] { "a" }).AsCollection);
            Assert.That(groups[1], Is.EqualTo(new[] { "b", "c" }).AsCollection);
        });
    }

    [Test]
    public void SplitToLineGroups_CustomPredicateSplitsOnCondition()
    {
        var input = "# meta\nfirst\n# meta\nsecond\nthird";

        var groups = input.SplitToLineGroups(line => line.StartsWith('#'));

        Assert.Multiple(() =>
        {
            Assert.That(groups.Length, Is.EqualTo(2));
            Assert.That(groups[0], Is.EqualTo(new[] { "first" }).AsCollection);
            Assert.That(groups[1], Is.EqualTo(new[] { "second", "third" }).AsCollection);
        });
    }
}


