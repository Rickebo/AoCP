using System.Numerics;

namespace Lib.Math;

/// <summary>
/// Number theory utilities such as GCD/LCM and prime generation.
/// </summary>
public static class NumberTheory
{
    /// <summary>
    /// Computes the greatest common divisor using the Euclidean algorithm.
    /// </summary>
    public static T GreatestCommonDivisor<T>(T a, T b) where T : INumber<T>
    {
        a = T.Abs(a);
        b = T.Abs(b);

        while (b != T.Zero)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }

    /// <summary>
    /// Computes the least common multiple of two numbers.
    /// </summary>
    public static T LeastCommonMultiple<T>(T a, T b) where T : INumber<T>
    {
        if (a == T.Zero || b == T.Zero)
            return T.Zero;

        return T.Abs(a * b) / GreatestCommonDivisor(a, b);
    }

    /// <summary>
    /// Computes the least common multiple of a sequence of numbers.
    /// </summary>
    public static T LeastCommonMultiple<T>(IEnumerable<T> values) where T : INumber<T>
    {
        var enumerator = values.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new ArgumentException("Sequence is empty", nameof(values));

        var lcm = enumerator.Current;
        while (enumerator.MoveNext())
            lcm = LeastCommonMultiple(lcm, enumerator.Current);

        return lcm;
    }

    /// <summary>
    /// Generates prime numbers up to and including <paramref name="limit"/> using the Sieve of Eratosthenes.
    /// </summary>
    public static IEnumerable<int> Sieve(int limit)
    {
        if (limit < 2)
            yield break;

        var isComposite = new bool[limit + 1];
        for (var number = 2; number <= limit; number++)
        {
            if (isComposite[number])
                continue;

            yield return number;

            if ((long)number * number > limit)
                continue;

            for (var multiple = number * number; multiple <= limit; multiple += number)
                isComposite[multiple] = true;
        }
    }
}

