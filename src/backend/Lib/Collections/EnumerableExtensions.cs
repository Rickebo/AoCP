namespace Lib.Collections;

/// <summary>
/// Convenience extensions for working with small tuples.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Checks whether either element in a tuple equals the provided <paramref name="item"/>.
    /// </summary>
    /// <typeparam name="T">Value type stored in the tuple.</typeparam>
    /// <param name="pair">The tuple to inspect.</param>
    /// <param name="item">The value to search for.</param>
    /// <returns><see langword="true"/> if either tuple element is equal to <paramref name="item"/>.</returns>
    public static bool Contains<T>(this (T, T) pair, T item) =>
        AreEqual(pair.Item1, item) || AreEqual(pair.Item2, item);

    private static bool AreEqual<T>(T item1, T item2) => item1 == null ? item2 == null : item1.Equals(item2);
}
