namespace Lib.Numerics;

public static class Combinatorics
{
    public static long Factorial(int n)
    {
        if (n < 0)
            throw new ArgumentOutOfRangeException(nameof(n));

        var result = 1L;
        for (var i = 2; i <= n; i++)
            result *= i;
        return result;
    }

    public static long Binomial(int n, int k)
    {
        if (k < 0 || k > n)
            return 0;
        if (k == 0 || k == n)
            return 1;

        k = Math.Min(k, n - k);
        long result = 1;
        for (var i = 1; i <= k; i++)
        {
            result *= n - (k - i);
            result /= i;
        }

        return result;
    }

    public static IEnumerable<IReadOnlyList<T>> Combinations<T>(IReadOnlyList<T> source, int k)
    {
        if (k < 0 || k > source.Count)
            yield break;

        var indices = Enumerable.Range(0, k).ToArray();
        while (true)
        {
            yield return indices.Select(i => source[i]).ToArray();

            var position = k - 1;
            while (position >= 0 && indices[position] == source.Count - k + position)
                position--;

            if (position < 0)
                yield break;

            indices[position]++;
            for (var i = position + 1; i < k; i++)
                indices[i] = indices[i - 1] + 1;
        }
    }

    public static IEnumerable<IReadOnlyList<T>> Permutations<T>(IReadOnlyList<T> source)
        where T : IComparable<T>
    {
        var arr = source.ToArray();
        yield return arr.ToArray();

        while (NextPermutation(arr))
            yield return arr.ToArray();
    }

    private static bool NextPermutation<T>(T[] array) where T : IComparable<T>
    {
        var i = array.Length - 2;
        while (i >= 0 && array[i].CompareTo(array[i + 1]) >= 0)
            i--;
        if (i < 0)
            return false;

        var j = array.Length - 1;
        while (array[j].CompareTo(array[i]) <= 0)
            j--;

        (array[i], array[j]) = (array[j], array[i]);
        Array.Reverse(array, i + 1, array.Length - i - 1);
        return true;
    }
}
