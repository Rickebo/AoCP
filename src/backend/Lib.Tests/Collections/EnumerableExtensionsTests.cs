using Lib.Collections;

namespace Lib.Tests.Collections;

public class EnumerableExtensionsTests
{
    [Test]
    public void Contains_FindsEitherItemInTuple()
    {
        var pair = (1, 2);

        Assert.Multiple(() =>
        {
            Assert.That(pair.Contains(1), Is.True);
            Assert.That(pair.Contains(2), Is.True);
            Assert.That(pair.Contains(3), Is.False);
        });
    }

    [Test]
    public void Contains_HandlesNullValues()
    {
        (string?, string?) pair = ("first", null);

        Assert.Multiple(() =>
        {
            Assert.That(pair.Contains("first"), Is.True);
            Assert.That(pair.Contains((string?)null), Is.True);
            Assert.That(pair.Contains("missing"), Is.False);
        });
    }
}