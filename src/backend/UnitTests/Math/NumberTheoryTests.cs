using Lib.Numerics;

namespace Lib.UnitTests.Math;

public class NumberTheoryTests
{
    [Test]
    public void GreatestCommonDivisor_ComputesForPositiveAndNegative()
    {
        Assert.Multiple(() =>
        {
            Assert.That(NumberTheory.GreatestCommonDivisor(54, 24), Is.EqualTo(6));
            Assert.That(NumberTheory.GreatestCommonDivisor(-8, 12), Is.EqualTo(4));
        });
    }

    [Test]
    public void LeastCommonMultiple_HandlesZerosAndCollections()
    {
        Assert.Multiple(() =>
        {
            Assert.That(NumberTheory.LeastCommonMultiple(0, 5), Is.EqualTo(0));
            Assert.That(NumberTheory.LeastCommonMultiple(6, 8), Is.EqualTo(24));
            Assert.That(NumberTheory.LeastCommonMultiple(new[] { 4, 6, 8 }), Is.EqualTo(24));
        });
    }

    [Test]
    public void Sieve_ReturnsPrimesUpToLimit()
    {
        var primes = NumberTheory.Sieve(20).ToArray();

        CollectionAssert.AreEqual(new[] { 2, 3, 5, 7, 11, 13, 17, 19 }, primes);
    }
}
