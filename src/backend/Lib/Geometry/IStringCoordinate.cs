namespace Lib.Geometry;

/// <summary>
/// Exposes string representations for coordinate components.
/// </summary>
public interface IStringCoordinate
{
    /// <summary>
    /// Returns the X component as a string.
    /// </summary>
    string? GetStringX();

    /// <summary>
    /// Returns the Y component as a string.
    /// </summary>
    string? GetStringY();
}
