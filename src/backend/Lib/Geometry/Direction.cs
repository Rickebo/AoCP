namespace Lib.Geometry;

[Flags]
public enum Direction
{
    None = 0,
    North = 1 << 0,
    East = 1 << 1,
    South = 1 << 2,
    West = 1 << 3,
    NorthEast = North | East,
    NorthWest = North | West,
    SouthEast = South | East,
    SouthWest = South | West,
    Up = North,
    Right = East,
    Down = South,
    Left = West,
    UpRight = NorthEast,
    UpLeft = NorthWest,
    DownRight = SouthEast,
    DownLeft = SouthWest,
}

