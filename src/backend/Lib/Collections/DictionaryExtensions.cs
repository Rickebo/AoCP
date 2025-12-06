using Lib.Math;

namespace Lib.Collections;

/// <summary>
/// Extension helpers for incrementing and clamping numeric values stored in dictionaries.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Increments an integer value for the given <paramref name="key"/> or initializes it to one, clamped to <paramref name="max"/>.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">The dictionary that holds integer counters.</param>
    /// <param name="key">Key whose value should be incremented.</param>
    /// <param name="max">Maximum allowed value after incrementing.</param>
    public static void Increment<T>(this Dictionary<T, int> dict, T key, int max = int.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out int val);
        dict[key] = System.Math.Min(val + 1, max);
    }

    /// <summary>
    /// Decrements an integer value for the given <paramref name="key"/> or initializes it to negative one, clamped to <paramref name="min"/>.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">The dictionary that holds integer counters.</param>
    /// <param name="key">Key whose value should be decremented.</param>
    /// <param name="min">Minimum allowed value after decrementing.</param>
    public static void Decrement<T>(this Dictionary<T, int> dict, T key, int min = int.MinValue) where T : notnull
    {
        dict.TryGetValue(key, out int val);
        dict[key] = System.Math.Max(val - 1, min);
    }

    /// <summary>
    /// Increments a long value for the given <paramref name="key"/> or initializes it to one, clamped to <paramref name="max"/>.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">The dictionary that holds long counters.</param>
    /// <param name="key">Key whose value should be incremented.</param>
    /// <param name="max">Maximum allowed value after incrementing.</param>
    public static void Increment<T>(this Dictionary<T, long> dict, T key, long max = long.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out long val);
        dict[key] = System.Math.Min(val + 1, max);
    }

    /// <summary>
    /// Decrements a long value for the given <paramref name="key"/> or initializes it to negative one, clamped to <paramref name="min"/>.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">The dictionary that holds long counters.</param>
    /// <param name="key">Key whose value should be decremented.</param>
    /// <param name="min">Minimum allowed value after decrementing.</param>
    public static void Decrement<T>(this Dictionary<T, long> dict, T key, long min = long.MinValue) where T : notnull
    {
        dict.TryGetValue(key, out long val);
        dict[key] = System.Math.Max(val - 1, min);
    }

    /// <summary>
    /// Adds <paramref name="val"/> to the stored integer value and clamps the result.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">The dictionary that holds integer counters.</param>
    /// <param name="key">Key whose value should be updated.</param>
    /// <param name="val">Amount to add.</param>
    /// <param name="min">Minimum allowed value after the update.</param>
    /// <param name="max">Maximum allowed value after the update.</param>
    public static void AddOrUpdate<T>(this Dictionary<T, int> dict, T key, int val, int min = int.MinValue, int max = int.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out int currVal);
        dict[key] = (currVal + val).Clamp(min, max);
    }

    /// <summary>
    /// Adds <paramref name="val"/> to the stored long value and clamps the result.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">The dictionary that holds long counters.</param>
    /// <param name="key">Key whose value should be updated.</param>
    /// <param name="val">Amount to add.</param>
    /// <param name="min">Minimum allowed value after the update.</param>
    /// <param name="max">Maximum allowed value after the update.</param>
    public static void AddOrUpdate<T>(this Dictionary<T, long> dict, T key, long val, long min = long.MinValue, long max = long.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out long currVal);
        dict[key] = (currVal + val).Clamp(min, max);
    }

    /// <summary>
    /// Adds <paramref name="val"/> to the stored double value and clamps the result.
    /// </summary>
    /// <typeparam name="T">Dictionary key type.</typeparam>
    /// <param name="dict">The dictionary that holds double counters.</param>
    /// <param name="key">Key whose value should be updated.</param>
    /// <param name="val">Amount to add.</param>
    /// <param name="min">Minimum allowed value after the update.</param>
    /// <param name="max">Maximum allowed value after the update.</param>
    public static void AddOrUpdate<T>(this Dictionary<T, double> dict, T key, double val, double min = double.MinValue, double max = double.MaxValue) where T : notnull
    {
        dict.TryGetValue(key, out double currVal);
        dict[key] = (currVal + val).Clamp(min, max);
    }
}


