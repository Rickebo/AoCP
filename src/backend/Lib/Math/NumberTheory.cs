using System.Numerics;

namespace Lib.Math;

public static class NumberTheory
{
    /// <summary>
    /// Computes the greatest common divisor of two numbers using the Euclidean algorithm.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <returns>The greatest common divisor of the two values.</returns>
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
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <returns>The least common multiple, or zero when either input is zero.</returns>
    public static T LeastCommonMultiple<T>(T a, T b) where T : INumber<T>
    {
        if (a == T.Zero || b == T.Zero)
            return T.Zero;

        return T.Abs(a * b) / GreatestCommonDivisor(a, b);
    }

    /// <summary>
    /// Computes the least common multiple of a collection of values.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="values">Sequence of values.</param>
    /// <returns>Least common multiple of the sequence.</returns>
    /// <exception cref="ArgumentException">Thrown when the sequence is empty.</exception>
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
    /// Yields all prime numbers up to the given limit using the sieve of Eratosthenes.
    /// </summary>
    /// <param name="limit">Largest number to consider.</param>
    /// <returns>Sequence of primes up to <paramref name="limit"/>.</returns>
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

