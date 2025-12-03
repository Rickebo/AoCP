using Lib.Extensions;

namespace Lib.UnitTests.Collections;

public class DictionaryExtensionsTests
{
    [Test]
    public void IncrementInt_AddsAndClamps()
    {
        var dict = new Dictionary<string, int>();

        dict.Increment("a");
        dict.Increment("a", max: 2);
        dict.Increment("a", max: 2);

        Assert.That(dict["a"], Is.EqualTo(2));
    }

    [Test]
    public void DecrementInt_AddsMissingKeyAndHonorsMin()
    {
        var dict = new Dictionary<int, int>();

        dict.Decrement(5, min: -1);
        dict.Decrement(5, min: -1);

        Assert.That(dict[5], Is.EqualTo(-1));
    }

    [Test]
    public void IncrementLong_UsesProvidedMax()
    {
        var dict = new Dictionary<string, long> { ["key"] = 9 };

        dict.Increment("key", max: 10);
        dict.Increment("key", max: 10);

        Assert.That(dict["key"], Is.EqualTo(10));
    }

    [Test]
    public void DecrementLong_UsesProvidedMin()
    {
        var dict = new Dictionary<int, long> { [1] = 0 };

        dict.Decrement(1, min: 0);

        Assert.That(dict[1], Is.EqualTo(0));
    }

    [Test]
    public void AddOrUpdateInt_ClampsToRange()
    {
        var dict = new Dictionary<string, int> { ["k"] = 5 };

        dict.AddOrUpdate("k", -10, min: 0, max: 10);
        dict.AddOrUpdate("new", 3, min: 0, max: 10);

        Assert.Multiple(() =>
        {
            Assert.That(dict["k"], Is.EqualTo(0));
            Assert.That(dict["new"], Is.EqualTo(3));
        });
    }

    [Test]
    public void AddOrUpdateLongAndDouble_WorkForNewAndExistingKeys()
    {
        var longDict = new Dictionary<int, long>();
        var doubleDict = new Dictionary<int, double> { [1] = 1.5 };

        longDict.AddOrUpdate(2, 4);
        doubleDict.AddOrUpdate(1, 0.75, max: 2);

        Assert.Multiple(() =>
        {
            Assert.That(longDict[2], Is.EqualTo(4));
            Assert.That(doubleDict[1], Is.EqualTo(2));
        });
    }
}
