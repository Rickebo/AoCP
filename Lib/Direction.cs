namespace Lib;

[Flags]
public enum Direction
{
    North = 1,
    NorthEast = 1 | 2,
    East = 2,
    SouthEast = 2 | 4,
    South = 4,
    SouthWest = 4 | 8,
    West = 8,
    NorthWest = 8 | 1
}