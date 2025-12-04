namespace Lib.Geometry;

[Flags]
public enum Neighbourhood
{
    None = 0,
    Horizontal = 1 << 0,
    Vertical = 1 << 1,
    Diagonal = 1 << 2,
    Cardinal = Horizontal | Vertical,
    All = Cardinal | Diagonal
}
