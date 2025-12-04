using Lib.Math;

namespace Lib.Collections;

public static class DictionaryExtensions
{
    /// <summary>
    /// Increments the value stored for the given key by one, respecting an optional maximum.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">Dictionary to update.</param>
    /// <param name="key">Key whose value should be incremented.</param>
    /// <param name="max">Maximum value allowed after increment.</param>
    public static void Increment<T>(this Dictionary<T, int> dict, T key, int max = int.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out int val);
        dict[key] = System.Math.Min(val + 1, max);
    }

    /// <summary>
    /// Decrements the value stored for the given key by one, respecting an optional minimum.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">Dictionary to update.</param>
    /// <param name="key">Key whose value should be decremented.</param>
    /// <param name="min">Minimum value allowed after decrement.</param>
    public static void Decrement<T>(this Dictionary<T, int> dict, T key, int min = int.MinValue) where T : notnull
    {
        dict.TryGetValue(key, out int val);
        dict[key] = System.Math.Max(val - 1, min);
    }

    /// <summary>
    /// Increments the value stored for the given key by one, respecting an optional maximum.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">Dictionary to update.</param>
    /// <param name="key">Key whose value should be incremented.</param>
    /// <param name="max">Maximum value allowed after increment.</param>
    public static void Increment<T>(this Dictionary<T, long> dict, T key, long max = long.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out long val);
        dict[key] = System.Math.Min(val + 1, max);
    }

    /// <summary>
    /// Decrements the value stored for the given key by one, respecting an optional minimum.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">Dictionary to update.</param>
    /// <param name="key">Key whose value should be decremented.</param>
    /// <param name="min">Minimum value allowed after decrement.</param>
    public static void Decrement<T>(this Dictionary<T, long> dict, T key, long min = long.MinValue) where T : notnull
    {
        dict.TryGetValue(key, out long val);
        dict[key] = System.Math.Max(val - 1, min);
    }

    /// <summary>
    /// Adds the supplied value to the current value for the key or inserts it if missing, with optional clamping.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">Dictionary to update.</param>
    /// <param name="key">Key whose value should be updated.</param>
    /// <param name="val">Value to add to the existing entry.</param>
    /// <param name="min">Minimum value allowed after the update.</param>
    /// <param name="max">Maximum value allowed after the update.</param>
    public static void AddOrUpdate<T>(this Dictionary<T, int> dict, T key, int val, int min = int.MinValue, int max = int.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out int currVal);
        dict[key] = (currVal + val).Clamp(min, max);
    }

    /// <summary>
    /// Adds the supplied value to the current value for the key or inserts it if missing, with optional clamping.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">Dictionary to update.</param>
    /// <param name="key">Key whose value should be updated.</param>
    /// <param name="val">Value to add to the existing entry.</param>
    /// <param name="min">Minimum value allowed after the update.</param>
    /// <param name="max">Maximum value allowed after the update.</param>
    public static void AddOrUpdate<T>(this Dictionary<T, long> dict, T key, long val, long min = long.MinValue, long max = long.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out long currVal);
        dict[key] = (currVal + val).Clamp(min, max);
    }

    /// <summary>
    /// Adds the supplied value to the current value for the key or inserts it if missing, with optional clamping.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">Dictionary to update.</param>
    /// <param name="key">Key whose value should be updated.</param>
    /// <param name="val">Value to add to the existing entry.</param>
    /// <param name="min">Minimum value allowed after the update.</param>
    /// <param name="max">Maximum value allowed after the update.</param>
    public static void AddOrUpdate<T>(this Dictionary<T, double> dict, T key, double val, double min = double.MinValue, double max = double.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out double currVal);
        dict[key] = (currVal + val).Clamp(min, max);
    }
}


