using System.Numerics;

namespace Lib.Numerics;

public static class NumberTheory
{
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

    public static T LeastCommonMultiple<T>(T a, T b) where T : INumber<T>
    {
        if (a == T.Zero || b == T.Zero)
            return T.Zero;

        return T.Abs(a * b) / GreatestCommonDivisor(a, b);
    }

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
