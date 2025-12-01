namespace Lib.Enums;

[Flags]
public enum Axis
{
    None = 0,
    X = 1 << 0,
    Y = 1 << 1,
}
