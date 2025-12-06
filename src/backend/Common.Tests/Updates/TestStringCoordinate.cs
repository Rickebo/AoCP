using Lib.Geometry;

namespace Common.Tests.Updates;

internal sealed class TestStringCoordinate(string? x, string? y) : IStringCoordinate
{
    public string? GetStringX() => x;

    public string? GetStringY() => y;
}
