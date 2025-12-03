using Lib.Color;
using Lib.Geometry;
using Lib.Grids;

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

    public static StringGridUpdate FromColorGrid(ArrayGrid<Color> grid) => FromGrid(
        grid,
        cell => cell.ToRgbaString(),
        Construct
    );

    public static StringGridUpdate FromStringGrid(ArrayGrid<string> grid) => FromGrid(
        grid,
        cell => cell.ToString(),
        Construct
    );

    public static StringGridUpdate FromCharGrid(ArrayGrid<char> grid) => FromGrid(
        grid,
        ch => ch.ToString(),
        Construct
    );

    public static StringGridUpdate FromRect(IntegerCoordinate<int> origin, int width, int height, Color color) =>
        FromRect(origin, width, height, color.ToRgbaString());

    public static StringGridUpdate FromRect(IntegerCoordinate<int> origin, int width, int height, string color)
    {
        var rows = new Dictionary<string, Dictionary<string, string>>();
        for (var y = 0; y < height; y++)
        {
            var row = new Dictionary<string, string>();
            for (var x = 0; x < width; x++)
                row[(origin.X + x).ToString()] = color;

            rows[(origin.Y + y).ToString()] = row;
        }

        return new StringGridUpdate()
        {
            Width = null,
            Height = null,
            Rows = rows,
        };
    }

    public static StringGridUpdateBuilder Builder() => new();

    public class StringCoordinateBuilder
    {
        public IStringCoordinate Coordinate { get; set; }
        public string Text { get; set; }

        public StringCoordinateBuilder WithCoordinate(IStringCoordinate coordinate)
        {
            Coordinate = coordinate;
            return this;
        }

        public StringCoordinateBuilder WithText(string text)
        {
            Text = text;
            return this;
        }

        public StringCoordinateBuilder WithColor(Color color)
        {
            Text = color.ToRgbaString();
            return this;
        }
    }

    public class StringGridUpdateBuilder
    {
        private bool Clear { get; set; } = false;
        private int? Width { get; set; }
        private int? Height { get; set; }
        private List<StringCoordinateBuilder> Texts { get; } = [];
        
        public StringGridUpdateBuilder WithWidth(int width)
        {
            Width = width;
            return this;
        }

        public StringGridUpdateBuilder WithHeight(int height)
        {
            Height = height;
            return this;
        }

        public StringGridUpdateBuilder WithClear()
        {
            Clear = true;
            return this;
        }

        public StringGridUpdateBuilder WithEntry(
            Func<StringCoordinateBuilder, StringCoordinateBuilder> configure
        )
        {
            Texts.Add(configure(new StringCoordinateBuilder()));
            return this;
        }

        public StringGridUpdateBuilder WithEntries<T>(
            IEnumerable<T> entries,
            Func<StringCoordinateBuilder, T, StringCoordinateBuilder> configure
        ) => entries.Aggregate(
            this,
            (current, entry) => current
                .WithEntry(builder => configure(builder, entry))
        );

        public StringGridUpdate Build()
        {
            var rows = new Dictionary<string, Dictionary<string, string>>();
            foreach (var text in Texts)
            {
                var x = text.Coordinate.GetStringX();
                var y = text.Coordinate.GetStringY();
                if (!rows.TryGetValue(y, out var row))
                    row = rows[y] = [];

                row[x] = text.Text;
            }

            return new StringGridUpdate
            {
                Width = Width,
                Height = Height,
                Clear = Clear,
                Rows = rows
            };
        }
    }
}

