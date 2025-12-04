namespace Lib.Collections;

public static class EnumerableExtensions
{
    /// <summary>
    /// Checks whether a tuple contains a specified item, using <see cref="object.Equals(object?)"/> comparison.
    /// </summary>
    /// <typeparam name="T">Type of the tuple elements.</typeparam>
    /// <param name="pair">Tuple to search.</param>
    /// <param name="item">Item to look for.</param>
    /// <returns><c>true</c> if the item matches either tuple element; otherwise <c>false</c>.</returns>
    public static bool Contains<T>(this (T, T) pair, T item) =>
        AreEqual(pair.Item1, item) || AreEqual(pair.Item2, item);

    /// <summary>
    /// Compares two values for equality, handling <c>null</c> safely.
    /// </summary>
    /// <typeparam name="T">Type of values being compared.</typeparam>
    /// <param name="item1">First value.</param>
    /// <param name="item2">Second value.</param>
    /// <returns><c>true</c> if values are equal; otherwise <c>false</c>.</returns>
    private static bool AreEqual<T>(T item1, T item2) => item1 == null ? item2 == null : item1.Equals(item2);
}
