namespace Lib.Geometry;

[Flags]
/// <summary>
/// Represents axes that can be flipped or reflected across.
/// </summary>
public enum Axis
{
    /// <summary>
    /// No axis specified.
    /// </summary>
    None = 0,
    /// <summary>
    /// Horizontal (X) axis.
    /// </summary>
    X = 1 << 0,
    /// <summary>
    /// Vertical (Y) axis.
    /// </summary>
    Y = 1 << 1,
}


