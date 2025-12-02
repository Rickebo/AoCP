using System.Numerics;

namespace Lib.Extensions;

public static class NumberExtensions
{
    public static T Clamp<T>(this T value, T min, T max)
        where T : INumber<T> =>
        T.Clamp(value, min, max);

    public static T Clamp<T>(this T value, T? min = null, T? max = null)
        where T : struct, INumber<T>, IMinMaxValue<T> =>
        T.Clamp(value, min ?? T.MinValue, max ?? T.MaxValue);
}

