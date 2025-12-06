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

    public static StringGridUpdate FromColorGrid(ArrayGrid<Color> grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        return FromGrid(
            grid,
            cell => cell.ToRgbaString(),
            Construct
        );
    }

    public static StringGridUpdate FromStringGrid(ArrayGrid<string> grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        return FromGrid(
            grid,
            cell => cell ?? string.Empty,
            Construct
        );
    }

    public static StringGridUpdate FromCharGrid(ArrayGrid<char> grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        return FromGrid(
            grid,
            ch => ch.ToString(),
            Construct
        );
    }

    public static StringGridUpdate FromRect(IntegerCoordinate<int> origin, int width, int height, Color color) =>
        FromRect(origin, width, height, color.ToRgbaString());

    public static StringGridUpdate FromRect(IntegerCoordinate<int> origin, int width, int height, string color)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        ArgumentException.ThrowIfNullOrWhiteSpace(color);

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
        public IStringCoordinate Coordinate { get; set; } = default!;

        public string Text { get; set; } = string.Empty;

        public StringCoordinateBuilder WithCoordinate(IStringCoordinate coordinate)
        {
            ArgumentNullException.ThrowIfNull(coordinate);
            Coordinate = coordinate;
            return this;
        }

        public StringCoordinateBuilder WithText(string text)
        {
            Text = text ?? string.Empty;
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

        public StringGridUpdateBuilder WithClear(bool clear = true)
        {
            Clear = clear;
            return this;
        }

        public StringGridUpdateBuilder WithEntry(
            Func<StringCoordinateBuilder, StringCoordinateBuilder> configure
        )
        {
            ArgumentNullException.ThrowIfNull(configure);
            Texts.Add(configure(new StringCoordinateBuilder()));
            return this;
        }

        public StringGridUpdateBuilder WithEntries<T>(
            IEnumerable<T> entries,
            Func<StringCoordinateBuilder, T, StringCoordinateBuilder> configure
        )
        {
            ArgumentNullException.ThrowIfNull(entries);
            ArgumentNullException.ThrowIfNull(configure);
            return entries.Aggregate(
                this,
                (current, entry) => current
                    .WithEntry(builder => configure(builder, entry))
            );
        }

        public StringGridUpdate Build()
        {
            var rows = new Dictionary<string, Dictionary<string, string>>();
            foreach (var text in Texts)
            {
                if (text.Coordinate == null)
                    throw new InvalidOperationException("Every entry must specify a coordinate.");

                var x = text.Coordinate.GetStringX();
                var y = text.Coordinate.GetStringY();

                if (string.IsNullOrWhiteSpace(x) || string.IsNullOrWhiteSpace(y))
                    throw new InvalidOperationException("Coordinate X and Y components must be non-empty.");

                if (!rows.TryGetValue(y, out var row))
                    row = rows[y] = [];

                row[x] = text.Text ?? string.Empty;
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

