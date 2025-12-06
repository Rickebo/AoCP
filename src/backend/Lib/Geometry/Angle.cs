namespace Lib.Geometry;

[Flags]
/// <summary>
/// Represents a rotation in 45 degree increments.
/// </summary>
public enum Angle
{
    /// <summary>
    /// Zero degrees.
    /// </summary>
    None = 0,
    /// <summary>
    /// 45 degree turn.
    /// </summary>
    EighthTurn = 1 << 0,
    /// <summary>
    /// 90 degree turn.
    /// </summary>
    QuarterTurn = 1 << 1,
    /// <summary>
    /// 180 degree turn.
    /// </summary>
    HalfTurn = 1 << 2,
}


