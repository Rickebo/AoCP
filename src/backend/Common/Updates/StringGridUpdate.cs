using Lib.Color;
using Lib.Geometry;
using Lib.Grids;

namespace Common.Updates;

/// <summary>
/// Grid update that carries string payloads for each addressed cell.
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
    /// Creates a string grid update from a <see cref="Color"/> grid.
    /// </summary>
    /// <param name="grid">Grid to translate.</param>
    /// <returns>A <see cref="StringGridUpdate"/> containing RGBA string values.</returns>
    public static StringGridUpdate FromColorGrid(ArrayGrid<Color> grid) => FromGrid(
        grid,
        cell => cell.ToRgbaString(),
        Construct
    );

    /// <summary>
    /// Creates a string grid update from a string grid.
    /// </summary>
    /// <param name="grid">Grid to translate.</param>
    /// <returns>A <see cref="StringGridUpdate"/>.</returns>
    public static StringGridUpdate FromStringGrid(ArrayGrid<string> grid) => FromGrid(
        grid,
        cell => cell ?? string.Empty,
        Construct
    );

    /// <summary>
    /// Creates a string grid update from a char grid.
    /// </summary>
    /// <param name="grid">Grid to translate.</param>
    /// <returns>A <see cref="StringGridUpdate"/>.</returns>
    public static StringGridUpdate FromCharGrid(ArrayGrid<char> grid) => FromGrid(
        grid,
        ch => ch.ToString(),
        Construct
    );

    /// <summary>
    /// Creates a rectangular fill starting at an origin with the provided color.
    /// </summary>
    /// <param name="origin">Top-left origin for the rectangle.</param>
    /// <param name="width">Rectangle width.</param>
    /// <param name="height">Rectangle height.</param>
    /// <param name="color">Fill color.</param>
    /// <returns>A <see cref="StringGridUpdate"/> describing the fill.</returns>
    public static StringGridUpdate FromRect(IntegerCoordinate<int> origin, int width, int height, Color color) =>
        FromRect(origin, width, height, color.ToRgbaString());

    /// <summary>
    /// Creates a rectangular fill starting at an origin with a string payload.
    /// </summary>
    /// <param name="origin">Top-left origin for the rectangle.</param>
    /// <param name="width">Rectangle width.</param>
    /// <param name="height">Rectangle height.</param>
    /// <param name="color">Value to set for each cell.</param>
    /// <returns>A <see cref="StringGridUpdate"/> describing the fill.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="color"/> is null or whitespace.</exception>
    public static StringGridUpdate FromRect(IntegerCoordinate<int> origin, int width, int height, string color)
    {
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
    /// Creates a builder for composing a string grid update fluently.
    /// </summary>
    /// <returns>A new <see cref="StringGridUpdateBuilder"/>.</returns>
    public static StringGridUpdateBuilder Builder() => new();

    /// <summary>
    /// Builder for configuring an individual string entry within a grid update.
    /// </summary>
    public class StringCoordinateBuilder
    {
        /// <summary>
        /// Gets or sets the coordinate targeted by the entry.
        /// </summary>
        public IStringCoordinate Coordinate { get; set; } = default!;

        /// <summary>
        /// Gets or sets the text to render at the coordinate.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Sets the coordinate for the entry.
        /// </summary>
        /// <param name="coordinate">Coordinate for the string entry.</param>
        /// <returns>The same builder instance.</returns>
        public StringCoordinateBuilder WithCoordinate(IStringCoordinate coordinate)
        {
            Coordinate = coordinate;
            return this;
        }

        /// <summary>
        /// Sets the text for the entry.
        /// </summary>
        /// <param name="text">Text to render.</param>
        /// <returns>The same builder instance.</returns>
        public StringCoordinateBuilder WithText(string text)
        {
            Text = text;
            return this;
        }

        /// <summary>
        /// Sets the text for the entry using the RGBA string for a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">Color to render.</param>
        /// <returns>The same builder instance.</returns>
        public StringCoordinateBuilder WithColor(Color color)
        {
            Text = color.ToRgbaString();
            return this;
        }
    }

    /// <summary>
    /// Builder for composing a complete string grid update.
    /// </summary>
    public class StringGridUpdateBuilder
    {
        private bool Clear { get; set; } = false;
        private int? Width { get; set; }
        private int? Height { get; set; }
        private List<StringCoordinateBuilder> Texts { get; } = [];
        
        /// <summary>
        /// Sets the total grid width.
        /// </summary>
        /// <param name="width">Grid width.</param>
        /// <returns>The same builder instance.</returns>
        public StringGridUpdateBuilder WithWidth(int width)
        {
            Width = width;
            return this;
        }

        /// <summary>
        /// Sets the total grid height.
        /// </summary>
        /// <param name="height">Grid height.</param>
        /// <returns>The same builder instance.</returns>
        public StringGridUpdateBuilder WithHeight(int height)
        {
            Height = height;
            return this;
        }

        /// <summary>
        /// Requests that the client clears existing grid state before applying this update.
        /// </summary>
        /// <param name="clear">Flag indicating whether to clear.</param>
        /// <returns>The same builder instance.</returns>
        public StringGridUpdateBuilder WithClear(bool clear = true)
        {
            Clear = clear;
            return this;
        }

        /// <summary>
        /// Adds a single entry to the grid update.
        /// </summary>
        /// <param name="configure">Configuration delegate for the entry.</param>
        /// <returns>The same builder instance.</returns>
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
        /// Adds multiple entries to the grid update.
        /// </summary>
        /// <param name="entries">Entries to translate into grid updates.</param>
        /// <param name="configure">Configuration delegate that maps entries to a builder.</param>
        /// <typeparam name="T">Type of the source entries.</typeparam>
        /// <returns>The same builder instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entries"/> or <paramref name="configure"/> is null.</exception>
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
        /// Creates the immutable <see cref="StringGridUpdate"/> instance.
        /// </summary>
        /// <returns>A configured <see cref="StringGridUpdate"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when coordinates are missing or invalid.</exception>
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

