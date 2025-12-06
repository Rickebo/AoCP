using Lib.Math;

namespace Lib.Collections;

public static class DictionaryExtensions
{
    public static void Increment<T>(this Dictionary<T, int> dict, T key, int max = int.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out int val);
        dict[key] = System.Math.Min(val + 1, max);
    }

    public static void Decrement<T>(this Dictionary<T, int> dict, T key, int min = int.MinValue) where T : notnull
    {
        dict.TryGetValue(key, out int val);
        dict[key] = System.Math.Max(val - 1, min);
    }

    public static void Increment<T>(this Dictionary<T, long> dict, T key, long max = long.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out long val);
        dict[key] = System.Math.Min(val + 1, max);
    }

    public static void Decrement<T>(this Dictionary<T, long> dict, T key, long min = long.MinValue) where T : notnull
    {
        dict.TryGetValue(key, out long val);
        dict[key] = System.Math.Max(val - 1, min);
    }

    public static void AddOrUpdate<T>(this Dictionary<T, int> dict, T key, int val, int min = int.MinValue, int max = int.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out int currVal);
        dict[key] = (currVal + val).Clamp(min, max);
    }

    public static void AddOrUpdate<T>(this Dictionary<T, long> dict, T key, long val, long min = long.MinValue, long max = long.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out long currVal);
        dict[key] = (currVal + val).Clamp(min, max);
    }

    public static void AddOrUpdate<T>(this Dictionary<T, double> dict, T key, double val, double min = double.MinValue, double max = double.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out double currVal);
        dict[key] = (currVal + val).Clamp(min, max);
    }
}


