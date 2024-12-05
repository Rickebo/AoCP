using Lib.Grid;

namespace Common.Updates;

public record Cell(string Glyph, string Fg, string Bg);

public class GlyphGridUpdate : GridUpdate<Cell>
{
    private static GlyphGridUpdate Construct(
        int width,
        int height,
        Dictionary<string, Dictionary<string, Cell>> rows
    ) => new()
    {
        Width = width,
        Height = height,
        Rows = rows
    };

    public static GlyphGridUpdate FromGrid<T>(
        ArrayGrid<T> grid,
        Func<T, Cell> cellConverter
    ) => FromGrid(
        grid,
        cellConverter,
        Construct
    );

    public static GlyphGridUpdate FromCharGrid(
        CharGrid grid,
        string foregroundColor,
        string backgroundColor
    ) => FromGrid(
        grid,
        ch => new Cell(ch.ToString(), foregroundColor, backgroundColor),
        Construct
    );
}