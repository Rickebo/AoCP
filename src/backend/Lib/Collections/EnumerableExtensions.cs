namespace Lib.Extensions;

public static class EnumerableExtensions
{
    public static bool Contains<T>(this (T, T) pair, T item) =>
        AreEqual(pair.Item1, item) || AreEqual(pair.Item2, item);

    private static bool AreEqual<T>(T item1, T item2) => item1 == null ? item2 == null : item1.Equals(item2);
}
