using Common.Updates;

namespace Common.Tests.Updates;

[TestFixture]
public class FinishedProblemUpdateTests
{
    [Test]
    public void FromSolution_WithFormattable_SetsSuccessfulAndSolution()
    {
        var update = FinishedProblemUpdate.FromSolution(99);

        Assert.Multiple(() =>
        {
            Assert.That(update.Successful, Is.True);
            Assert.That(update.Solution, Is.EqualTo("99"));
            Assert.That(update.Error, Is.Null);
        });
    }

    [Test]
    public void FromError_SetsErrorAndFailed()
    {
        var update = FinishedProblemUpdate.FromError("boom", "partial");

        Assert.Multiple(() =>
        {
            Assert.That(update.Successful, Is.False);
            Assert.That(update.Error, Is.EqualTo("boom"));
            Assert.That(update.Solution, Is.EqualTo("partial"));
        });
    }
}
