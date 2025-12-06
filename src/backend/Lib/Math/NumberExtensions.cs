using System.Numerics;

namespace Lib.Math;

/// <summary>
/// Extension methods for clamping numeric values.
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    /// Clamps a value between inclusive minimum and maximum bounds.
    /// </summary>
    public static T Clamp<T>(this T value, T min, T max)
        where T : INumber<T> =>
        T.Clamp(value, min, max);

    /// <summary>
    /// Clamps a value using optional bounds; defaults to the numeric min/max values.
    /// </summary>
    public static T Clamp<T>(this T value, T? min = null, T? max = null)
        where T : struct, INumber<T>, IMinMaxValue<T> =>
        T.Clamp(value, min ?? T.MinValue, max ?? T.MaxValue);
}


