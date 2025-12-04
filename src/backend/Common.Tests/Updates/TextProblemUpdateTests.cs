using Common.Updates;

namespace Common.Tests.Updates;

[TestFixture]
public class TextProblemUpdateTests
{
    [Test]
    public void FromLine_WithFormattable_UsesFormattedValue()
    {
        var update = TextProblemUpdate.FromLine(12);

        Assert.Multiple(() =>
        {
            Assert.That(update.Lines, Is.EqualTo(new[] { "12" }));
            Assert.That(update.Text, Is.Null);
        });
    }

    [Test]
    public void FromLines_WithEnumerable_ThrowsOnNull()
    {
        Assert.Throws<ArgumentNullException>(() => TextProblemUpdate.FromLines((IEnumerable<string>)null!));
    }

    [Test]
    public void FromLines_WithStrings_AllowsNullArrayAndReturnsEmpty()
    {
        var update = TextProblemUpdate.FromLines((string[]?)null!);

        Assert.That(update.Lines, Is.Empty);
    }

    [Test]
    public void FromText_WithFormattable_UsesFormattedValue()
    {
        var update = TextProblemUpdate.FromText(42);

        Assert.Multiple(() =>
        {
            Assert.That(update.Lines, Is.Null);
            Assert.That(update.Text, Is.EqualTo("42"));
        });
    }
}
