namespace Lib.Geometry;

[Flags]
public enum Angle
{
    None = 0,
    EighthTurn = 1 << 0,
    QuarterTurn = 1 << 1,
    HalfTurn = 1 << 2,
}


