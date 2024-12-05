using Lib;
using Lib.Grid;

namespace Common.Updates;

public class StringGridUpdate : GridUpdate<string>
{
    private static StringGridUpdate Construct(
        int width,
        int height,
        Dictionary<string, Dictionary<string, string>> rows
    ) => new()
    {
        Width = width,
        Height = height,
        Rows = rows
    };

    public static StringGridUpdate FromColorGrid(ArrayGrid<Color32> grid) => FromGrid(
        grid,
        color => color.ToString(),
        Construct
    );

    public static StringGridUpdate FromStringGrid(ArrayGrid<string> grid) => FromGrid(
        grid,
        ch => ch.ToString(),
        Construct
    );

    public static StringGridUpdate FromCharGrid(ArrayGrid<char> grid) => FromGrid(
        grid,
        ch => ch.ToString(),
        Construct
    );
}