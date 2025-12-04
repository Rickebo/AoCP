namespace Lib.Math;

public static class Combinatorics
{
    /// <summary>
    /// Calculates the factorial of a non-negative integer.
    /// </summary>
    /// <param name="n">Value to compute the factorial for.</param>
    /// <returns><paramref name="n"/>!.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="n"/> is negative.</exception>
    public static long Factorial(int n)
    {
        if (n < 0)
            throw new ArgumentOutOfRangeException(nameof(n));

        var result = 1L;
        for (var i = 2; i <= n; i++)
            result *= i;
        return result;
    }

    /// <summary>
    /// Calculates the binomial coefficient "n choose k".
    /// </summary>
    /// <param name="n">Total number of items.</param>
    /// <param name="k">Number of items to choose.</param>
    /// <returns>Number of ways to choose <paramref name="k"/> items from <paramref name="n"/>.</returns>
    public static long Binomial(int n, int k)
    {
        if (k < 0 || k > n)
            return 0;
        if (k == 0 || k == n)
            return 1;

        k = System.Math.Min(k, n - k);
        long result = 1;
        for (var i = 1; i <= k; i++)
        {
            result *= n - (k - i);
            result /= i;
        }

        return result;
    }

    /// <summary>
    /// Generates all k-length combinations of the source list.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="source">Source items.</param>
    /// <param name="k">Number of items per combination.</param>
    /// <returns>All unique combinations of length <paramref name="k"/>.</returns>
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

    /// <summary>
    /// Generates permutations of the source list in lexicographic order.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="source">Items to permute.</param>
    /// <returns>All permutations of the source list.</returns>
    public static IEnumerable<IReadOnlyList<T>> Permutations<T>(IReadOnlyList<T> source)
        where T : IComparable<T>
    {
        var arr = source.ToArray();
        yield return arr.ToArray();

        while (NextPermutation(arr))
            yield return arr.ToArray();
    }

    /// <summary>
    /// Advances the array to the next lexicographic permutation.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="array">Array to permute in place.</param>
    /// <returns><c>true</c> if the next permutation exists; otherwise <c>false</c>.</returns>
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


