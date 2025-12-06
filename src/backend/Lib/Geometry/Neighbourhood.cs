namespace Lib.Geometry;

[Flags]
/// <summary>
/// Represents neighbour sets commonly used in grid traversal.
/// </summary>
public enum Neighbourhood
{
    /// <summary>
    /// No neighbours.
    /// </summary>
    None = 0,
    /// <summary>
    /// Horizontal neighbours (west/east).
    /// </summary>
    Horizontal = 1 << 0,
    /// <summary>
    /// Vertical neighbours (north/south).
    /// </summary>
    Vertical = 1 << 1,
    /// <summary>
    /// Diagonal neighbours.
    /// </summary>
    Diagonal = 1 << 2,
    /// <summary>
    /// Cardinal neighbours (horizontal and vertical).
    /// </summary>
    Cardinal = Horizontal | Vertical,
    /// <summary>
    /// All eight neighbours.
    /// </summary>
    All = Cardinal | Diagonal
}
