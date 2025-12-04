using System.Numerics;

namespace Lib.Math;

public static class NumberExtensions
{
    /// <summary>
    /// Clamps a numeric value between the specified minimum and maximum values.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="value">Value to clamp.</param>
    /// <param name="min">Minimum allowed value.</param>
    /// <param name="max">Maximum allowed value.</param>
    /// <returns>The clamped value.</returns>
    public static T Clamp<T>(this T value, T min, T max)
        where T : INumber<T> =>
        T.Clamp(value, min, max);

    /// <summary>
    /// Clamps a numeric value between optional bounds, defaulting to the type's min and max values.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="value">Value to clamp.</param>
    /// <param name="min">Optional minimum bound.</param>
    /// <param name="max">Optional maximum bound.</param>
    /// <returns>The clamped value.</returns>
    public static T Clamp<T>(this T value, T? min = null, T? max = null)
        where T : struct, INumber<T>, IMinMaxValue<T> =>
        T.Clamp(value, min ?? T.MinValue, max ?? T.MaxValue);
}


