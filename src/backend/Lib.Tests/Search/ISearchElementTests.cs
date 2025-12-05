using Lib.Search;

namespace Lib.Tests.Search;

public class ISearchElementTests
{
    [Test]
    public void TestNode_ImplementsInterface()
    {
        ISearchElement<int> element = new TestNode("id");

        Assert.That(element, Is.Not.Null);
    }
}


