using Lib.Geometry;
using Lib.Grids;
using Lib.Color;

namespace Common.Updates;

/// <summary>
/// Represents a single glyph entry within a glyph grid update.
/// </summary>
public sealed record GlyphCell
{
    /// <summary>
    /// Gets the glyph identifier to render.
    /// </summary>
    public string? Glyph { get; init; }

    /// <summary>
    /// Gets the raw character to render if no glyph is specified.
    /// </summary>
    public string? Char { get; init; }

    /// <summary>
    /// Gets the foreground color.
    /// </summary>
    public string? Fg { get; init; }

    /// <summary>
    /// Gets the background color.
    /// </summary>
    public string? Bg { get; init; }
}

/// <summary>
/// Represents an update that renders glyphs on a grid.
/// </summary>
public class GlyphGridUpdate : GridUpdate<GlyphCell>
{
    private static GlyphGridUpdate Construct(
        int width,
        int height,
        Dictionary<string, Dictionary<string, GlyphCell>> rows
    ) => new()
    {
        Width = width,
        Height = height,
        Rows = rows
    };

    /// <summary>
    /// Creates an update from a generic grid using the provided converter.
    /// </summary>
    /// <typeparam name="T">Type stored in the source grid.</typeparam>
    /// <param name="grid">Source grid.</param>
    /// <param name="cellConverter">Converts a source cell to a <see cref="GlyphCell"/>.</param>
    /// <returns>A <see cref="GlyphGridUpdate"/> describing the grid.</returns>
    public static GlyphGridUpdate FromGrid<T>(
        ArrayGrid<T> grid,
        Func<T, GlyphCell> cellConverter
    ) => FromGrid(
        grid,
        cellConverter,
        Construct
    );

    /// <summary>
    /// Creates an update from a character grid using string colors.
    /// </summary>
    /// <param name="grid">Source grid of characters.</param>
    /// <param name="foregroundColor">Foreground color applied to each cell.</param>
    /// <param name="backgroundColor">Background color applied to each cell.</param>
    /// <returns>A <see cref="GlyphGridUpdate"/> describing the grid.</returns>
    public static GlyphGridUpdate FromCharGrid(
        CharGrid grid,
        string foregroundColor,
        string backgroundColor
    ) => FromGrid(
        grid,
        ch => new GlyphCell
        {
            Glyph = null,
            Char = ch.ToString(),
            Fg = foregroundColor,
            Bg = backgroundColor
        },
        Construct
    );

    /// <summary>
    /// Creates an update from a character grid using <see cref="Color"/> values.
    /// </summary>
    /// <param name="grid">Source grid of characters.</param>
    /// <param name="foregroundColor">Foreground color applied to each cell.</param>
    /// <param name="backgroundColor">Background color applied to each cell.</param>
    /// <returns>A <see cref="GlyphGridUpdate"/> describing the grid.</returns>
    public static GlyphGridUpdate FromCharGrid(
        CharGrid grid,
        Color foregroundColor,
        Color backgroundColor
    ) => FromGrid(
        grid,
        ch => new GlyphCell
        {
            Glyph = null,
            Char = ch.ToString(),
            Fg = foregroundColor.ToString(),
            Bg = backgroundColor.ToString()
        },
        Construct
    );

    /// <summary>
    /// Creates a new builder for composing glyph grid updates.
    /// </summary>
    /// <returns>A new <see cref="GlyphGridUpdateBuilder"/>.</returns>
    public static GlyphGridUpdateBuilder Builder() => new();

    /// <summary>
    /// Fluent builder for configuring a single glyph entry.
    /// </summary>
    public class GlyphBuilder
    {
        /// <summary>
        /// Gets or sets the coordinate to update.
        /// </summary>
        public IStringCoordinate Coordinate { get; set; } = default!;

        /// <summary>
        /// Gets or sets the character to render.
        /// </summary>
        public char? Character { get; set; }

        /// <summary>
        /// Gets or sets the glyph identifier to render.
        /// </summary>
        public string? Glyph { get; set; }

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        public string? Foreground { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public string? Background { get; set; }

        /// <summary>
        /// Sets the coordinate to update.
        /// </summary>
        /// <param name="coordinate">Coordinate to update.</param>
        /// <returns>The same builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinate"/> is null.</exception>
        public GlyphBuilder WithCoordinate(IStringCoordinate coordinate)
        {
            ArgumentNullException.ThrowIfNull(coordinate);
            Coordinate = coordinate;
            return this;
        }

        /// <summary>
        /// Sets the character to render.
        /// </summary>
        /// <param name="ch">Character to render.</param>
        /// <returns>The same builder for chaining.</returns>
        public GlyphBuilder WithChar(char ch)
        {
            Character = ch;
            return this;
        }

        /// <summary>
        /// Sets the glyph identifier from a character.
        /// </summary>
        /// <param name="glyph">Glyph character.</param>
        /// <returns>The same builder for chaining.</returns>
        public GlyphBuilder WithGlyph(char glyph) =>
            WithGlyph(glyph.ToString());

        /// <summary>
        /// Sets the glyph identifier.
        /// </summary>
        /// <param name="glyph">Glyph identifier.</param>
        /// <returns>The same builder for chaining.</returns>
        public GlyphBuilder WithGlyph(string glyph)
        {
            Glyph = glyph;
            return this;
        }

        /// <summary>
        /// Sets the foreground color.
        /// </summary>
        /// <param name="color">Foreground color.</param>
        /// <returns>The same builder for chaining.</returns>
        public GlyphBuilder WithForeground(Color color)
        {
            Foreground = color.ToRgbaString();
            return this;
        }

        /// <summary>
        /// Sets the background color.
        /// </summary>
        /// <param name="color">Background color.</param>
        /// <returns>The same builder for chaining.</returns>
        public GlyphBuilder WithBackground(Color color)
        {
            Background = color.ToRgbaString();
            return this;
        }

        /// <summary>
        /// Sets the foreground color.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        /// <returns>The same builder for chaining.</returns>
        public GlyphBuilder WithForeground(string foreground)
        {
            Foreground = foreground;
            return this;
        }

        /// <summary>
        /// Sets the background color.
        /// </summary>
        /// <param name="background">Background color.</param>
        /// <returns>The same builder for chaining.</returns>
        public GlyphBuilder WithBackground(string background)
        {
            Background = background;
            return this;
        }
    }

    /// <summary>
    /// Fluent builder for creating <see cref="GlyphGridUpdate"/> instances.
    /// </summary>
    public class GlyphGridUpdateBuilder
    {
        private bool Clear { get; set; } = false;
        private int? Width { get; set; }
        private int? Height { get; set; }
        private List<GlyphBuilder> Glyphs { get; } = [];

        /// <summary>
        /// Adds a path of glyph entries representing the provided coordinates.
        /// </summary>
        /// <param name="coordinates">Coordinates that form the path.</param>
        /// <param name="foreground">Optional foreground color.</param>
        /// <param name="background">Optional background color.</param>
        /// <returns>The same builder for chaining.</returns>
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
        /// Sets the grid width to communicate to the client.
        /// </summary>
        /// <param name="width">Grid width.</param>
        /// <returns>The same builder for chaining.</returns>
        public GlyphGridUpdateBuilder WithWidth(int width)
        {
            Width = width;
            return this;
        }

        /// <summary>
        /// Sets the grid height to communicate to the client.
        /// </summary>
        /// <param name="height">Grid height.</param>
        /// <returns>The same builder for chaining.</returns>
        public GlyphGridUpdateBuilder WithHeight(int height)
        {
            Height = height;
            return this;
        }

        /// <summary>
        /// Flags whether the client should clear the grid before applying updates.
        /// </summary>
        /// <param name="clear">Whether to request a clear.</param>
        /// <returns>The same builder for chaining.</returns>
        public GlyphGridUpdateBuilder WithClear(bool clear = true)
        {
            Clear = clear;
            return this;
        }

        /// <summary>
        /// Adds a single glyph entry configured via a callback.
        /// </summary>
        /// <param name="configure">Callback to populate the entry.</param>
        /// <returns>The same builder for chaining.</returns>
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
        /// Adds multiple glyph entries configured from a collection.
        /// </summary>
        /// <typeparam name="T">Type of source entries.</typeparam>
        /// <param name="entries">Source entries to convert.</param>
        /// <param name="configure">Callback that maps an entry to a builder configuration.</param>
        /// <returns>The same builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="entries"/> or <paramref name="configure"/> is null.
        /// </exception>
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
        /// Builds the immutable <see cref="GlyphGridUpdate"/> instance.
        /// </summary>
        /// <returns>The composed glyph grid update.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when required coordinate or glyph information is missing.
        /// </exception>
        public GlyphGridUpdate Build()
        {
            var rows = new Dictionary<string, Dictionary<string, GlyphCell>>();
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

                row[x] = new GlyphCell
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

