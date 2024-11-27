namespace Lib;

public static class MathExtensions
{
    public static int Remainder(int x, int divisor) =>
        (x % divisor + divisor) % divisor;
}