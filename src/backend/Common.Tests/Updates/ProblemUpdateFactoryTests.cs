using Common.Updates;

namespace Common.Tests.Updates;

[TestFixture]
public class ProblemUpdateFactoryTests
{
    [Test]
    public void TextProblemUpdate_FromLine_FormattableFormatsValue()
    {
        var expected = 12.5.ToString();
        var update = TextProblemUpdate.FromLine(12.5);

        Assert.Multiple(() =>
        {
            Assert.That(update.Lines, Is.EqualTo(new[] { expected }));
            Assert.That(update.Text, Is.Null);
        });
    }

    [Test]
    public void TextProblemUpdate_FromFormattableArray_FormatsValues()
    {
        var update = TextProblemUpdate.FromLines(new IFormattable[] { 1, 2.5m });

        Assert.That(update.Lines, Is.EqualTo(new[] { 1.ToString(), 2.5m.ToString() }));
    }

    [Test]
    public void TextProblemUpdate_FromLines_NormalizesNullEntries()
    {
        var update = TextProblemUpdate.FromLines(new string[] { "a", null!, "c" });

        Assert.Multiple(() =>
        {
            Assert.That(update.Text, Is.Null);
            Assert.That(update.Lines, Is.EqualTo(new[] { "a", string.Empty, "c" }));
        });
    }

    [Test]
    public void TextProblemUpdate_FromLines_NullEnumerable_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => TextProblemUpdate.FromLines((IEnumerable<string>)null!)
        );
    }

    [Test]
    public void TextProblemUpdate_FromText_NullString_UsesEmpty()
    {
        var update = TextProblemUpdate.FromText((string)null!);

        Assert.Multiple(() =>
        {
            Assert.That(update.Text, Is.EqualTo(string.Empty));
            Assert.That(update.Lines, Is.Null);
        });
    }

    [Test]
    public void TextProblemUpdate_FromText_NullFormattable_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => TextProblemUpdate.FromText((IFormattable)null!)
        );
    }

    [Test]
    public void FinishedProblemUpdate_FromSolution_String_SetsSuccess()
    {
        var update = FinishedProblemUpdate.FromSolution((string)null!);

        Assert.Multiple(() =>
        {
            Assert.That(update.Successful, Is.True);
            Assert.That(update.Solution, Is.EqualTo(string.Empty));
            Assert.That(update.Error, Is.Null);
        });
    }

    [Test]
    public void FinishedProblemUpdate_FromSolution_Formattable_FormatsValue()
    {
        var update = FinishedProblemUpdate.FromSolution(new Version(1, 2));

        Assert.Multiple(() =>
        {
            Assert.That(update.Successful, Is.True);
            Assert.That(update.Solution, Is.EqualTo("1.2"));
            Assert.That(update.Error, Is.Null);
        });
    }

    [Test]
    public void FinishedProblemUpdate_FromError_SetsFailureAndCarriesSolution()
    {
        var update = FinishedProblemUpdate.FromError("broken", "partial");

        Assert.Multiple(() =>
        {
            Assert.That(update.Successful, Is.False);
            Assert.That(update.Error, Is.EqualTo("broken"));
            Assert.That(update.Solution, Is.EqualTo("partial"));
        });
    }

    [Test]
    public void FinishedProblemUpdate_FromError_WithNullError_UsesEmptyString()
    {
        var update = FinishedProblemUpdate.FromError(null!, null);

        Assert.Multiple(() =>
        {
            Assert.That(update.Successful, Is.False);
            Assert.That(update.Error, Is.EqualTo(string.Empty));
            Assert.That(update.Solution, Is.Null);
        });
    }
}
