namespace Lib.Math.Tests;

public class CombinatoricsTests
{
    [Test]
    public void Factorial_ValidatesInput()
    {
        Assert.That(Combinatorics.Factorial(5), Is.EqualTo(120));
        Assert.Throws<ArgumentOutOfRangeException>(() => Combinatorics.Factorial(-1));
    }

    [Test]
    public void Binomial_HandlesEdgeCases()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Combinatorics.Binomial(5, 0), Is.EqualTo(1));
            Assert.That(Combinatorics.Binomial(5, 2), Is.EqualTo(10));
            Assert.That(Combinatorics.Binomial(3, 5), Is.EqualTo(0));
        });
    }

    [Test]
    public void Combinations_GenerateAllKLengthChoices()
    {
        var combos = Combinatorics.Combinations([1, 2, 3], 2).ToArray();

        Assert.That(
            combos,
            Is.EquivalentTo(
            [
                new[] { 1, 2 },
                new[] { 1, 3 },
                new[] { 2, 3 }
            ]));
    }

    [Test]
    public void Permutations_ReturnLexicographicOrder()
    {
        var perms = Combinatorics.Permutations([1, 2, 3]).ToArray();

        Assert.That(
            perms,
            Is.EqualTo(new[]
            {
                new[] { 1, 2, 3 },
                new[] { 1, 3, 2 },
                new[] { 2, 1, 3 },
                new[] { 2, 3, 1 },
                new[] { 3, 1, 2 },
                new[] { 3, 2, 1 }
            }).AsCollection);
    }
}

