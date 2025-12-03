namespace Lib.Geometry;

/// <summary>
/// Angle statements
/// </summary>
[Flags]
public enum Angle
{
    /// <summary> 0 degrees. </summary>
    None = 0,
    /// <summary> 45 degrees. </summary>
    EighthTurn = 1 << 0, 
    /// <summary> 90 degrees. </summary>
    QuarterTurn = 1 << 1, // 90 degrees
    /// <summary> 180 degrees. </summary>
    HalfTurn = 1 << 2, // 180 degrees
}


