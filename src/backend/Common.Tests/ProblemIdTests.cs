using Common;

namespace Common.Tests;

[TestFixture]
public class ProblemIdTests
{
    [Test]
    public void Create_WithValidInput_SetsPropertiesAndDisplayName()
    {
        var id = ProblemId.Create(2024, "Source", "Author", "Set", "Problem");

        Assert.Multiple(() =>
        {
            Assert.That(id.Year, Is.EqualTo(2024));
            Assert.That(id.Source, Is.EqualTo("Source"));
            Assert.That(id.Author, Is.EqualTo("Author"));
            Assert.That(id.SetName, Is.EqualTo("Set"));
            Assert.That(id.ProblemName, Is.EqualTo("Problem"));
            Assert.That(id.DisplayName, Is.EqualTo("Source/2024/Author/Set/Problem"));
        });
    }

    [Test]
    public void Create_WithWhitespaceSource_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => ProblemId.Create(2024, " ", "Author", "Set", "Problem")
        );
    }

    [Test]
    public void Create_WithWhitespaceAuthor_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => ProblemId.Create(2024, "Source", "\t", "Set", "Problem")
        );
    }

    [Test]
    public void Create_WithWhitespaceSetName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => ProblemId.Create(2024, "Source", "Author", "", "Problem")
        );
    }

    [Test]
    public void Create_WithWhitespaceProblemName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => ProblemId.Create(2024, "Source", "Author", "Set", "  ")
        );
    }
}
