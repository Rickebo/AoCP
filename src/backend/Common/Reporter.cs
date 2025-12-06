using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Common.Updates;
using Lib.Geometry;
using Lib.Grids;
using Lib.Color;

namespace Common;

/// <summary>
/// Collects and streams updates about a problem execution to interested consumers.
/// </summary>
public class Reporter
{
    private readonly Channel<ProblemUpdate> _updates = Channel.CreateUnbounded<ProblemUpdate>();

    /// <summary>
    /// Queues a raw <see cref="ProblemUpdate"/> for downstream readers.
    /// </summary>
    /// <param name="update">The update to enqueue.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="update"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the update cannot be written to the channel.</exception>
    public void Report(ProblemUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);

        if (!_updates.Writer.TryWrite(update))
            throw new InvalidOperationException("Failed to write update to channel.");
    }

    /// <summary>
    /// Reports a single line of text output.
    /// </summary>
    /// <param name="line">The line to emit; null becomes an empty string.</param>
    public void ReportLine(string line) =>
        ReportLines([line ?? string.Empty]);

    /// <summary>
    /// Reports multiple lines of textual output.
    /// </summary>
    /// <param name="lines">The lines to emit.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="lines"/> is null.</exception>
    public void ReportLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ReportText(lines: lines);
    }

    /// <summary>
    /// Reports either a single block of text or a sequence of lines.
    /// </summary>
    /// <param name="text">Block of text to emit.</param>
    /// <param name="lines">Lines to emit.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when both <paramref name="text"/> and <paramref name="lines"/> are provided
    /// or when neither is supplied.
    /// </exception>
    public void ReportText(string? text = null, IEnumerable<string>? lines = null)
    {
        var hasText = text != null;
        var hasLines = lines != null;

        if (!hasText && !hasLines)
            throw new ArgumentException("Provide either text or lines to report.", nameof(text));

        if (hasText && hasLines)
            throw new ArgumentException("Provide either text or lines, not both.", nameof(text));

        Report(hasLines
            ? TextProblemUpdate.FromLines(lines!)
            : TextProblemUpdate.FromText(text!));
    }

    /// <summary>
    /// Reports a formatted solution value as a finished update.
    /// </summary>
    /// <param name="solution">The computed solution value.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="solution"/> is null.</exception>
    public void ReportSolution(IFormattable solution)
    {
        ArgumentNullException.ThrowIfNull(solution);
        ReportSolution(solution.ToString() ?? string.Empty);
    }

    /// <summary>
    /// Reports a solution string as a finished update.
    /// </summary>
    /// <param name="solution">The computed solution.</param>
    public void ReportSolution(string solution) =>
        Report(FinishedProblemUpdate.FromSolution(solution));

    /// <summary>
    /// Reports an error and optional partial solution as a finished update.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <param name="partialSolution">An optional partial solution to display.</param>
    public void ReportError(string error, string? partialSolution = null) =>
        Report(FinishedProblemUpdate.FromError(error, partialSolution));

    /// <summary>
    /// Reports a string grid update using the provided coordinate and color.
    /// </summary>
    /// <param name="coordinate">Grid coordinate to update.</param>
    /// <param name="color">Color to render at the coordinate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinate"/> is null.</exception>
    public void ReportStringGridUpdate(IStringCoordinate coordinate, Color color)
    {
        ArgumentNullException.ThrowIfNull(coordinate);
        ReportStringGridUpdate(coordinate, color.ToRgbaString());
    }

    /// <summary>
    /// Reports a string grid update for a single coordinate.
    /// </summary>
    /// <param name="coordinate">Grid coordinate to update.</param>
    /// <param name="text">Text to render at the coordinate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinate"/> is null.</exception>
    public void ReportStringGridUpdate(IStringCoordinate coordinate, string text)
    {
        ArgumentNullException.ThrowIfNull(coordinate);
        ReportStringGridUpdate(
            builder => builder
                .WithEntry(
                    txt => txt
                        .WithText(text)
                        .WithCoordinate(coordinate)
                )
        );
    }

    /// <summary>
    /// Reports a composed string grid update built with a fluent builder.
    /// </summary>
    /// <param name="configure">Callback to populate the update.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
    public void ReportStringGridUpdate(
        Func<
            StringGridUpdate.StringGridUpdateBuilder,
            StringGridUpdate.StringGridUpdateBuilder
        > configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        Report(configure(StringGridUpdate.Builder()).Build());
    }

    /// <summary>
    /// Reports a string grid update generated from a source grid.
    /// </summary>
    /// <typeparam name="T">Type stored in the source grid.</typeparam>
    /// <param name="grid">Grid to read from.</param>
    /// <param name="configure">
    /// Callback that maps each coordinate and cell value to a configured string entry.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="grid"/> or <paramref name="configure"/> is null.
    /// </exception>
    public void ReportStringGridUpdate<T>(
        ArrayGrid<T> grid,
        Func<
            StringGridUpdate.StringCoordinateBuilder,
            IntegerCoordinate<int>,
            T,
            StringGridUpdate.StringCoordinateBuilder
        > configure) =>
        ReportStringGridUpdate(
            builder =>
            {
                ArgumentNullException.ThrowIfNull(grid);
                ArgumentNullException.ThrowIfNull(configure);

                return builder
                    .WithWidth(grid.Width)
                    .WithHeight(grid.Height)
                    .WithEntries(
                        grid.Coordinates,
                        (stringUpdateBuilder, coordinate) => configure(
                            stringUpdateBuilder,
                            coordinate,
                            grid[coordinate]
                        )
                    );
            }
        );

    /// <summary>
    /// Reports a glyph grid update constructed with a fluent builder.
    /// </summary>
    /// <param name="configure">Callback to populate the glyph update.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
    public void ReportGlyphGridUpdate(
        Func<
            GlyphGridUpdate.GlyphGridUpdateBuilder,
            GlyphGridUpdate.GlyphGridUpdateBuilder
        > configure
    )
    {
        ArgumentNullException.ThrowIfNull(configure);
        Report(configure(GlyphGridUpdate.Builder()).Build());
    }

    /// <summary>
    /// Reports a glyph grid update generated from a source grid.
    /// </summary>
    /// <typeparam name="T">Type stored in the source grid.</typeparam>
    /// <param name="grid">Grid to render.</param>
    /// <param name="configure">
    /// Callback that maps each coordinate and value to a glyph entry.
    /// </param>
    /// <param name="predicate">Optional filter to select which coordinates to emit.</param>
    /// <param name="clear">Whether to request clearing the target grid before applying entries.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="grid"/> or <paramref name="configure"/> is null.
    /// </exception>
    public void ReportGlyphGridUpdate<T>(
        ArrayGrid<T> grid,
        Func<GlyphGridUpdate.GlyphBuilder, IntegerCoordinate<int>, T,
            GlyphGridUpdate.GlyphBuilder> configure,
        Func<IntegerCoordinate<int>, T, bool>? predicate = null,
        bool clear = false
    ) => ReportGlyphGridUpdate(
        builder =>
        {
            ArgumentNullException.ThrowIfNull(grid);
            ArgumentNullException.ThrowIfNull(configure);

            return builder
                .WithClear(clear)
                .WithWidth(grid.Width)
                .WithHeight(grid.Height)
                .WithEntries(
                    predicate != null 
                        ? grid.Coordinates.Where(p => predicate.Invoke(p, grid[p])) 
                        : grid.Coordinates,
                    (glyphBuilder, coordinate) => configure(
                        glyphBuilder,
                        coordinate,
                        grid[coordinate]
                    )
                );
        }
    );

    /// <summary>
    /// Reads all currently buffered updates without waiting for future items.
    /// </summary>
    /// <returns>An enumerable of buffered updates.</returns>
    public IEnumerable<ProblemUpdate> ReadAllCurrent()
    {
        while (_updates.Reader.TryRead(out var item))
            yield return item;
    }

    /// <summary>
    /// Continuously yields updates until a <see cref="FinishedProblemUpdate"/> is read.
    /// </summary>
    /// <param name="cancellationToken">Token that cancels reading.</param>
    /// <returns>An async stream of updates.</returns>
    public async IAsyncEnumerable<ProblemUpdate> ReadAll(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        while (true)
        {
            var update = await _updates.Reader.ReadAsync(cancellationToken);

            yield return update;

            if (update is FinishedProblemUpdate)
                break;
        }
    }
}

