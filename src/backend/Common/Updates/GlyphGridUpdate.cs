using Lib.Geometry;
using Lib.Grids;
using Lib.Color;

namespace Common.Updates;

/// <summary>
/// Represents a glyph cell with optional character and color data.
/// </summary>
public sealed record Cell
{
    /// <summary>
    /// Gets or sets the glyph to render, typically a multi-character string.
    /// </summary>
    public string? Glyph { get; init; }

    /// <summary>
    /// Gets or sets the single character to render.
    /// </summary>
    public string? Char { get; init; }

    /// <summary>
    /// Gets or sets the foreground color as an RGBA string.
    /// </summary>
    public string? Fg { get; init; }

    /// <summary>
    /// Gets or sets the background color as an RGBA string.
    /// </summary>
    public string? Bg { get; init; }
}

/// <summary>
/// Grid update that carries glyph/character payloads.
/// </summary>
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

    /// <summary>
    /// Creates a glyph grid update from a grid by applying a converter to each cell.
    /// </summary>
    /// <param name="grid">Grid to translate.</param>
    /// <param name="cellConverter">Function converting grid cells to <see cref="Cell"/>.</param>
    /// <typeparam name="T">Source grid cell type.</typeparam>
    /// <returns>A <see cref="GlyphGridUpdate"/>.</returns>
    public static GlyphGridUpdate FromGrid<T>(
        ArrayGrid<T> grid,
        Func<T, Cell> cellConverter
    ) => FromGrid(
        grid,
        cellConverter,
        Construct
    );

    /// <summary>
    /// Creates a glyph grid update from a character grid using explicit color strings.
    /// </summary>
    /// <param name="grid">Character grid.</param>
    /// <param name="foregroundColor">Foreground color string.</param>
    /// <param name="backgroundColor">Background color string.</param>
    /// <returns>A <see cref="GlyphGridUpdate"/>.</returns>
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

    /// <summary>
    /// Creates a glyph grid update from a character grid using <see cref="Color"/> values.
    /// </summary>
    /// <param name="grid">Character grid.</param>
    /// <param name="foregroundColor">Foreground color.</param>
    /// <param name="backgroundColor">Background color.</param>
    /// <returns>A <see cref="GlyphGridUpdate"/>.</returns>
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

    /// <summary>
    /// Creates a builder for composing glyph grid updates.
    /// </summary>
    /// <returns>A new <see cref="GlyphGridUpdateBuilder"/>.</returns>
    public static GlyphGridUpdateBuilder Builder() => new();

    /// <summary>
    /// Builder for configuring individual glyph entries.
    /// </summary>
    public class GlyphBuilder
    {
        /// <summary>
        /// Gets or sets the grid coordinate for the glyph.
        /// </summary>
        public IStringCoordinate Coordinate { get; set; } = default!;

        /// <summary>
        /// Gets or sets the single character to render.
        /// </summary>
        public char? Character { get; set; }

        /// <summary>
        /// Gets or sets a glyph string (for example, arrows or box-drawing characters).
        /// </summary>
        public string? Glyph { get; set; }

        /// <summary>
        /// Gets or sets the foreground color as an RGBA string.
        /// </summary>
        public string? Foreground { get; set; }

        /// <summary>
        /// Gets or sets the background color as an RGBA string.
        /// </summary>
        public string? Background { get; set; }

        /// <summary>
        /// Assigns the coordinate for the glyph entry.
        /// </summary>
        /// <param name="coordinate">Target coordinate.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphBuilder WithCoordinate(IStringCoordinate coordinate)
        {
            Coordinate = coordinate;
            return this;
        }

        /// <summary>
        /// Assigns a character for the glyph entry.
        /// </summary>
        /// <param name="ch">Character to render.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphBuilder WithChar(char ch)
        {
            Character = ch;
            return this;
        }

        /// <summary>
        /// Assigns a glyph string from a character.
        /// </summary>
        /// <param name="glyph">Glyph character.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphBuilder WithGlyph(char glyph) =>
            WithGlyph(glyph.ToString());

        /// <summary>
        /// Assigns a glyph string.
        /// </summary>
        /// <param name="glyph">Glyph text.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphBuilder WithGlyph(string glyph)
        {
            Glyph = glyph;
            return this;
        }

        /// <summary>
        /// Sets the foreground color from a <see cref="Color"/> value.
        /// </summary>
        /// <param name="color">Foreground color.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphBuilder WithForeground(Color color)
        {
            Foreground = color.ToRgbaString();
            return this;
        }

        /// <summary>
        /// Sets the background color from a <see cref="Color"/> value.
        /// </summary>
        /// <param name="color">Background color.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphBuilder WithBackground(Color color)
        {
            Background = color.ToRgbaString();
            return this;
        }

        /// <summary>
        /// Sets the foreground color from a string.
        /// </summary>
        /// <param name="foreground">Foreground color string.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphBuilder WithForeground(string foreground)
        {
            Foreground = foreground;
            return this;
        }

        /// <summary>
        /// Sets the background color from a string.
        /// </summary>
        /// <param name="background">Background color string.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphBuilder WithBackground(string background)
        {
            Background = background;
            return this;
        }
    }

    /// <summary>
    /// Builder for composing a glyph grid update.
    /// </summary>
    public class GlyphGridUpdateBuilder
    {
        private bool Clear { get; set; } = false;
        private int? Width { get; set; }
        private int? Height { get; set; }
        private List<GlyphBuilder> Glyphs { get; } = [];

        /// <summary>
        /// Adds a visual path to the grid using connected cardinal directions.
        /// </summary>
        /// <param name="coordinates">Coordinates forming the path.</param>
        /// <param name="foreground">Optional foreground color.</param>
        /// <param name="background">Optional background color.</param>
        /// <returns>The same builder instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinates"/> is null.</exception>
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

        /// <summary>
        /// Sets the total grid width.
        /// </summary>
        /// <param name="width">Grid width.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphGridUpdateBuilder WithWidth(int width)
        {
            Width = width;
            return this;
        }

        /// <summary>
        /// Sets the total grid height.
        /// </summary>
        /// <param name="height">Grid height.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphGridUpdateBuilder WithHeight(int height)
        {
            Height = height;
            return this;
        }

        /// <summary>
        /// Requests that the client clears existing grid state before applying this update.
        /// </summary>
        /// <param name="clear">Flag indicating whether to clear.</param>
        /// <returns>The same builder instance.</returns>
        public GlyphGridUpdateBuilder WithClear(bool clear = true)
        {
            Clear = clear;
            return this;
        }

        /// <summary>
        /// Adds a single glyph entry to the grid update.
        /// </summary>
        /// <param name="configure">Configuration delegate for the entry.</param>
        /// <returns>The same builder instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
        public GlyphGridUpdateBuilder WithEntry(
            Func<GlyphBuilder, GlyphBuilder> configure
        )
        {
            ArgumentNullException.ThrowIfNull(configure);
            Glyphs.Add(configure(new GlyphBuilder()));
            return this;
        }

        /// <summary>
        /// Adds multiple glyph entries to the grid update.
        /// </summary>
        /// <param name="entries">Entries to translate into grid updates.</param>
        /// <param name="configure">Configuration delegate that maps entries to a builder.</param>
        /// <typeparam name="T">Type of the source entries.</typeparam>
        /// <returns>The same builder instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entries"/> or <paramref name="configure"/> is null.</exception>
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

        /// <summary>
        /// Creates the immutable <see cref="GlyphGridUpdate"/> instance.
        /// </summary>
        /// <returns>A configured <see cref="GlyphGridUpdate"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when required data is missing.</exception>
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

