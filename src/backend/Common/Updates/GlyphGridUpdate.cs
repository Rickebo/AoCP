using Lib.Geometry;
using Lib.Grids;
using Lib.Color;

namespace Common.Updates;

public sealed record Cell
{
    public string? Glyph { get; init; }

    public string? Char { get; init; }

    public string? Fg { get; init; }

    public string? Bg { get; init; }
}

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
        ch => new Cell
        {
            Glyph = null,
            Char = ch.ToString(),
            Fg = foregroundColor,
            Bg = backgroundColor
        },
        Construct
    );

    public static GlyphGridUpdate FromCharGrid(
        CharGrid grid,
        Color foregroundColor,
        Color backgroundColor
    ) => FromGrid(
        grid,
        ch => new Cell
        {
            Glyph = null,
            Char = ch.ToString(),
            Fg = foregroundColor.ToString(),
            Bg = backgroundColor.ToString()
        },
        Construct
    );

    public static GlyphGridUpdateBuilder Builder() => new();

    public class GlyphBuilder
    {
        public IStringCoordinate Coordinate { get; set; } = default!;

        public char? Character { get; set; }

        public string? Glyph { get; set; }

        public string? Foreground { get; set; }

        public string? Background { get; set; }

        public GlyphBuilder WithCoordinate(IStringCoordinate coordinate)
        {
            Coordinate = coordinate;
            return this;
        }

        public GlyphBuilder WithChar(char ch)
        {
            Character = ch;
            return this;
        }

        public GlyphBuilder WithGlyph(char glyph) =>
            WithGlyph(glyph.ToString());

        public GlyphBuilder WithGlyph(string glyph)
        {
            Glyph = glyph;
            return this;
        }

        public GlyphBuilder WithForeground(Color color)
        {
            Foreground = color.ToRgbaString();
            return this;
        }

        public GlyphBuilder WithBackground(Color color)
        {
            Background = color.ToRgbaString();
            return this;
        }

        public GlyphBuilder WithForeground(string foreground)
        {
            Foreground = foreground;
            return this;
        }

        public GlyphBuilder WithBackground(string background)
        {
            Background = background;
            return this;
        }
    }

    public class GlyphGridUpdateBuilder
    {
        private bool Clear { get; set; } = false;
        private int? Width { get; set; }
        private int? Height { get; set; }
        private List<GlyphBuilder> Glyphs { get; } = [];

        public GlyphGridUpdateBuilder WithPath(
            IEnumerable<IntegerCoordinate<int>> coordinates,
            Color? foreground = null,
            Color? background = null
        )
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            var set = coordinates as HashSet<IntegerCoordinate<int>> ??
                      [.. coordinates];

            foreach (var coordinate in set)
            {
                var dir = DirectionExtensions.Cardinals
                    .Aggregate(
                        Direction.None,
                        (current, offset) =>
                            set.Contains(coordinate.Move(offset))
                                ? current | offset
                                : current
                    );
                
                dir = dir.FlipY();

                WithEntry(
                    builder =>
                    {
                        if (foreground.HasValue)
                            builder = builder.WithForeground(foreground.Value);
                        if (background.HasValue)
                            builder = builder.WithBackground(background.Value);

                        return builder
                            .WithCoordinate(coordinate)
                            .WithGlyph(dir.ToGlyph());
                    }
                );
            }

            return this;
        }

        public GlyphGridUpdateBuilder WithWidth(int width)
        {
            Width = width;
            return this;
        }

        public GlyphGridUpdateBuilder WithHeight(int height)
        {
            Height = height;
            return this;
        }

        public GlyphGridUpdateBuilder WithClear(bool clear = true)
        {
            Clear = clear;
            return this;
        }

        public GlyphGridUpdateBuilder WithEntry(
            Func<GlyphBuilder, GlyphBuilder> configure
        )
        {
            ArgumentNullException.ThrowIfNull(configure);
            Glyphs.Add(configure(new GlyphBuilder()));
            return this;
        }

        public GlyphGridUpdateBuilder WithEntries<T>(
            IEnumerable<T> entries,
            Func<GlyphBuilder, T, GlyphBuilder> configure
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

        public GlyphGridUpdate Build()
        {
            var rows = new Dictionary<string, Dictionary<string, Cell>>();
            foreach (var glyph in Glyphs)
            {
                if (glyph.Coordinate == null)
                    throw new InvalidOperationException("Every entry must specify a coordinate.");

                var x = glyph.Coordinate.GetStringX();
                var y = glyph.Coordinate.GetStringY();

                if (string.IsNullOrWhiteSpace(x) || string.IsNullOrWhiteSpace(y))
                    throw new InvalidOperationException("Coordinate X and Y components must be non-empty.");

                if (string.IsNullOrWhiteSpace(glyph.Glyph) && !glyph.Character.HasValue)
                    throw new InvalidOperationException("At least a glyph or character must be provided for each entry.");

                if (!rows.TryGetValue(y, out var row))
                    row = rows[y] = [];

                row[x] = new Cell
                {
                    Glyph = glyph.Glyph,
                    Char = glyph.Character?.ToString(),
                    Fg = glyph.Foreground,
                    Bg = glyph.Background
                };
            }

            return new GlyphGridUpdate
            {
                Width = Width,
                Height = Height,
                Clear = Clear,
                Rows = rows
            };
        }
    }
}

