namespace Lib
{
    public static class NumberExtensions
    {
        public static int Clamp(this int value, int min = int.MinValue, int max = int.MaxValue)
            => Math.Min(Math.Max(value, min), max);

        public static long Clamp(this long value, long min = long.MinValue, long max = long.MaxValue)
            => Math.Min(Math.Max(value, min), max);

        public static double Clamp(this double value, double min = double.MinValue, double max = double.MaxValue)
            => Math.Min(Math.Max(value, min), max);
    }
}
