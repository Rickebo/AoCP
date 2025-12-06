using Lib.Color;
using Lib.Geometry;
using Lib.Grids;

namespace Common.Updates;

/// <summary>
/// Represents an update that renders text content on a grid.
/// </summary>
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

    /// <summary>
    /// Creates an update from a color grid, converting each cell to its RGBA representation.
    /// </summary>
    /// <param name="grid">Source grid of colors.</param>
    /// <returns>A <see cref="StringGridUpdate"/> describing the grid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="grid"/> is null.</exception>
    public static StringGridUpdate FromColorGrid(ArrayGrid<Color> grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        return FromGrid(
            grid,
            cell => cell.ToRgbaString(),
            Construct
        );
    }

    /// <summary>
    /// Creates an update from a grid of strings.
    /// </summary>
    /// <param name="grid">Source grid of strings.</param>
    /// <returns>A <see cref="StringGridUpdate"/> describing the grid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="grid"/> is null.</exception>
    public static StringGridUpdate FromStringGrid(ArrayGrid<string> grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        return FromGrid(
            grid,
            cell => cell ?? string.Empty,
            Construct
        );
    }

    /// <summary>
    /// Creates an update from a grid of characters.
    /// </summary>
    /// <param name="grid">Source grid of characters.</param>
    /// <returns>A <see cref="StringGridUpdate"/> describing the grid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="grid"/> is null.</exception>
    public static StringGridUpdate FromCharGrid(ArrayGrid<char> grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        return FromGrid(
            grid,
            ch => ch.ToString(),
            Construct
        );
    }

    /// <summary>
    /// Creates an update that paints a rectangle of a single color.
    /// </summary>
    /// <param name="origin">Top-left origin of the rectangle.</param>
    /// <param name="width">Width of the rectangle.</param>
    /// <param name="height">Height of the rectangle.</param>
    /// <param name="color">Color to paint.</param>
    /// <returns>A <see cref="StringGridUpdate"/> describing the rectangle.</returns>
    public static StringGridUpdate FromRect(IntegerCoordinate<int> origin, int width, int height, Color color) =>
        FromRect(origin, width, height, color.ToRgbaString());

    /// <summary>
    /// Creates an update that paints a rectangle of a single color.
    /// </summary>
    /// <param name="origin">Top-left origin of the rectangle.</param>
    /// <param name="width">Width of the rectangle.</param>
    /// <param name="height">Height of the rectangle.</param>
    /// <param name="color">Color to paint, expressed as a string.</param>
    /// <returns>A <see cref="StringGridUpdate"/> describing the rectangle.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="width"/> or <paramref name="height"/> is non-positive.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="color"/> is null or whitespace.</exception>
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

    /// <summary>
    /// Creates a new builder for composing string grid updates.
    /// </summary>
    /// <returns>A new <see cref="StringGridUpdateBuilder"/>.</returns>
    public static StringGridUpdateBuilder Builder() => new();

    /// <summary>
    /// Fluent builder for configuring a single string grid entry.
    /// </summary>
    public class StringCoordinateBuilder
    {
        /// <summary>
        /// Gets or sets the coordinate to update.
        /// </summary>
        public IStringCoordinate Coordinate { get; set; } = default!;

        /// <summary>
        /// Gets or sets the text to render at the coordinate.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Sets the coordinate to update.
        /// </summary>
        /// <param name="coordinate">Coordinate to update.</param>
        /// <returns>The same builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinate"/> is null.</exception>
        public StringCoordinateBuilder WithCoordinate(IStringCoordinate coordinate)
        {
            ArgumentNullException.ThrowIfNull(coordinate);
            Coordinate = coordinate;
            return this;
        }

        /// <summary>
        /// Sets the text to render.
        /// </summary>
        /// <param name="text">Text to render.</param>
        /// <returns>The same builder for chaining.</returns>
        public StringCoordinateBuilder WithText(string text)
        {
            Text = text ?? string.Empty;
            return this;
        }

        /// <summary>
        /// Sets the text to the provided color string.
        /// </summary>
        /// <param name="color">Color to render.</param>
        /// <returns>The same builder for chaining.</returns>
        public StringCoordinateBuilder WithColor(Color color)
        {
            Text = color.ToRgbaString();
            return this;
        }
    }

    /// <summary>
    /// Fluent builder for creating <see cref="StringGridUpdate"/> instances.
    /// </summary>
    public class StringGridUpdateBuilder
    {
        private bool Clear { get; set; } = false;
        private int? Width { get; set; }
        private int? Height { get; set; }
        private List<StringCoordinateBuilder> Texts { get; } = [];
        
        /// <summary>
        /// Sets the grid width to communicate to the client.
        /// </summary>
        /// <param name="width">Grid width.</param>
        /// <returns>The same builder for chaining.</returns>
        public StringGridUpdateBuilder WithWidth(int width)
        {
            Width = width;
            return this;
        }

        /// <summary>
        /// Sets the grid height to communicate to the client.
        /// </summary>
        /// <param name="height">Grid height.</param>
        /// <returns>The same builder for chaining.</returns>
        public StringGridUpdateBuilder WithHeight(int height)
        {
            Height = height;
            return this;
        }

        /// <summary>
        /// Flags whether the client should clear the grid before applying updates.
        /// </summary>
        /// <param name="clear">Whether to request a clear.</param>
        /// <returns>The same builder for chaining.</returns>
        public StringGridUpdateBuilder WithClear(bool clear = true)
        {
            Clear = clear;
            return this;
        }

        /// <summary>
        /// Adds a single entry configured via a callback.
        /// </summary>
        /// <param name="configure">Callback to populate the entry.</param>
        /// <returns>The same builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
        public StringGridUpdateBuilder WithEntry(
            Func<StringCoordinateBuilder, StringCoordinateBuilder> configure
        )
        {
            ArgumentNullException.ThrowIfNull(configure);
            Texts.Add(configure(new StringCoordinateBuilder()));
            return this;
        }

        /// <summary>
        /// Adds multiple entries configured from a collection.
        /// </summary>
        /// <typeparam name="T">Type of source entries.</typeparam>
        /// <param name="entries">Source entries to convert.</param>
        /// <param name="configure">Callback that maps an entry to a builder configuration.</param>
        /// <returns>The same builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="entries"/> or <paramref name="configure"/> is null.
        /// </exception>
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

        /// <summary>
        /// Builds the immutable <see cref="StringGridUpdate"/> instance.
        /// </summary>
        /// <returns>The composed string grid update.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an entry is missing a coordinate or when coordinate components are blank.
        /// </exception>
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

