using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Common.Updates;

public record Cell(string? Glyph, string? Char, string? Fg, string? Bg);

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
        ch => new Cell(null, ch.ToString(), foregroundColor, backgroundColor),
        Construct
    );

    public static GlyphGridUpdate FromCharGrid(
        CharGrid grid,
        Color foregroundColor,
        Color backgroundColor
    ) => FromGrid(
        grid,
        ch => new Cell(
            null,
            ch.ToString(),
            foregroundColor.ToString(),
            backgroundColor.ToString()
        ),
        Construct
    );

    public static GlyphGridUpdateBuilder Builder() => new();

    public class GlyphBuilder
    {
        public IStringCoordinate Coordinate { get; set; }
        public char? Character { get; set; }
        public string Glyph { get; set; }
        public string Foreground { get; set; }
        public string Background { get; set; }

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
            var set = coordinates as HashSet<IntegerCoordinate<int>> ??
                      coordinates.ToHashSet();

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
            Glyphs.Add(configure(new GlyphBuilder()));
            return this;
        }

        public GlyphGridUpdateBuilder WithEntries<T>(
            IEnumerable<T> entries,
            Func<GlyphBuilder, T, GlyphBuilder> configure
        ) => entries.Aggregate(
            this,
            (current, entry) => current
                .WithEntry(builder => configure(builder, entry))
        );

        public GlyphGridUpdate Build()
        {
            var rows = new Dictionary<string, Dictionary<string, Cell>>();
            foreach (var glyph in Glyphs)
            {
                var x = glyph.Coordinate.GetStringX();
                var y = glyph.Coordinate.GetStringY();

                if (x == null || y == null)
                    throw new Exception("X and Y cannot be null.");

                if (!rows.TryGetValue(y, out var row))
                    row = rows[y] = new Dictionary<string, Cell>();

                row[x] = new Cell(
                    glyph.Glyph,
                    glyph.Character.ToString(),
                    glyph.Foreground,
                    glyph.Background
                );
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