namespace Lib.Geometry;

[Flags]
/// <summary>
/// Represents 2D directions with flag combinations for diagonals and aliases.
/// </summary>
public enum Direction
{
    /// <summary>
    /// No direction.
    /// </summary>
    None = 0,
    /// <summary>
    /// North/up direction.
    /// </summary>
    North = 1 << 0,
    /// <summary>
    /// East/right direction.
    /// </summary>
    East = 1 << 1,
    /// <summary>
    /// South/down direction.
    /// </summary>
    South = 1 << 2,
    /// <summary>
    /// West/left direction.
    /// </summary>
    West = 1 << 3,
    /// <summary>
    /// North-east diagonal.
    /// </summary>
    NorthEast = North | East,
    /// <summary>
    /// North-west diagonal.
    /// </summary>
    NorthWest = North | West,
    /// <summary>
    /// South-east diagonal.
    /// </summary>
    SouthEast = South | East,
    /// <summary>
    /// South-west diagonal.
    /// </summary>
    SouthWest = South | West,
    /// <summary>
    /// Alias for <see cref="North"/>.
    /// </summary>
    Up = North,
    /// <summary>
    /// Alias for <see cref="East"/>.
    /// </summary>
    Right = East,
    /// <summary>
    /// Alias for <see cref="South"/>.
    /// </summary>
    Down = South,
    /// <summary>
    /// Alias for <see cref="West"/>.
    /// </summary>
    Left = West,
    /// <summary>
    /// Alias for <see cref="NorthEast"/>.
    /// </summary>
    UpRight = NorthEast,
    /// <summary>
    /// Alias for <see cref="NorthWest"/>.
    /// </summary>
    UpLeft = NorthWest,
    /// <summary>
    /// Alias for <see cref="SouthEast"/>.
    /// </summary>
    DownRight = SouthEast,
    /// <summary>
    /// Alias for <see cref="SouthWest"/>.
    /// </summary>
    DownLeft = SouthWest,
}
