
namespace Lib.Collections.Tests;

public class EnumerableExtensionsTests
{
    [Test]
    public void Contains_FindsEitherItemInTuple()
    {
        var pair = (1, 2);

        Assert.That(pair.Contains(1), Is.True);
        Assert.That(pair.Contains(2), Is.True);
        Assert.That(pair.Contains(3), Is.False);
    }

    [Test]
    public void Contains_HandlesNullValues()
    {
        (string?, string?) pair = ("first", null);

        Assert.That(pair.Contains("first"), Is.True);
        Assert.That(pair.Contains((string?)null), Is.True);
        Assert.That(pair.Contains("missing"), Is.False);
    }
}

