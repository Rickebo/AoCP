using Lib.Text;

namespace Lib.Tests.Text;

public class StringSpanTests
{
    [Test]
    public void Constructor_ClampsStartAndLength()
    {
        var span = new StringSpan("hello", 1, 3);

        Assert.Multiple(() =>
        {
            Assert.That(span.Start, Is.EqualTo(1));
            Assert.That(span.Length, Is.EqualTo(3));
            Assert.That(span.End, Is.EqualTo(4));
            Assert.That(span[0], Is.EqualTo('e'));
        });

        Assert.Throws<IndexOutOfRangeException>(() => { var _ = span[5]; });
    }

    [Test]
    public void Substring_And_StartsWith_Work()
    {
        var span = new StringSpan("abcdef", 1, 4); // bcde
        var sub = span.Substring(1, 2); // cd

        Assert.Multiple(() =>
        {
            Assert.That(sub.ToString(), Is.EqualTo("cd"));
            Assert.That(span.StartsWith("bc"), Is.True);
            Assert.That(span.StartsWith(new StringSpan("abcd", 1, 2)), Is.True);
        });
    }

    [Test]
    public void Equality_UsesContentAndLength()
    {
        var first = new StringSpan("hello", 0, 2);
        var second = new StringSpan("he", 0, 2);

        Assert.Multiple(() =>
        {
            Assert.That(first, Is.EqualTo(second));
            Assert.That(first == second, Is.True);
            Assert.That(first != new StringSpan("hx", 0, 2), Is.True);
            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
        });
    }
}


