using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Common.Updates;
using Lib.Geometry;
using Lib.Grids;
using Lib.Color;

namespace Common;

/// <summary>
/// Streams <see cref="ProblemUpdate"/> instances to observers via an asynchronous channel.
/// </summary>
public class Reporter
{
    private readonly Channel<ProblemUpdate> _updates = Channel.CreateUnbounded<ProblemUpdate>();

    /// <summary>
    /// Enqueues an update for downstream consumers.
    /// </summary>
    /// <param name="update">Update to publish.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="update"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the channel refuses the update.</exception>
    public void Report(ProblemUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);

        if (!_updates.Writer.TryWrite(update))
            throw new InvalidOperationException("Failed to write update to channel.");
    }

    /// <summary>
    /// Publishes a single-line text update.
    /// </summary>
    /// <param name="line">Line to send.</param>
    public void ReportLine(string line) =>
        ReportLines([line ?? string.Empty]);

    /// <summary>
    /// Publishes a text update containing multiple lines.
    /// </summary>
    /// <param name="lines">Lines to send.</param>
    public void ReportLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ReportText(lines: lines);
    }

    /// <summary>
    /// Publishes a text update containing freeform text and/or lines.
    /// </summary>
    /// <param name="text">Optional freeform text block.</param>
    /// <param name="lines">Optional lines to include.</param>
    public void ReportText(string? text = null, IEnumerable<string>? lines = null) =>
        Report(new TextProblemUpdate(){ Text = text, Lines = lines?.ToArray() });

    /// <summary>
    /// Publishes a successful completion update using a formatted value.
    /// </summary>
    /// <param name="solution">Solution to format.</param>
    public void ReportSolution(IFormattable solution)
    {
        ArgumentNullException.ThrowIfNull(solution);
        ReportSolution(solution.ToString() ?? string.Empty);
    }

    /// <summary>
    /// Publishes a successful completion update.
    /// </summary>
    /// <param name="solution">Solution text.</param>
    public void ReportSolution(string solution) =>
        Report(FinishedProblemUpdate.FromSolution(solution));

    /// <summary>
    /// Publishes a failed completion update with an error message.
    /// </summary>
    /// <param name="error">Error description.</param>
    /// <param name="partialSolution">Optional partial solution value.</param>
    public void ReportError(string error, string? partialSolution = null) =>
        Report(FinishedProblemUpdate.FromError(error, partialSolution));

    /// <summary>
    /// Publishes a single string-grid update for one coordinate using a <see cref="Color"/> payload.
    /// </summary>
    /// <param name="coordinate">Coordinate to update.</param>
    /// <param name="color">Color to render.</param>
    public void ReportStringGridUpdate(IStringCoordinate coordinate, Color color)
    {
        ArgumentNullException.ThrowIfNull(coordinate);
        ReportStringGridUpdate(coordinate, color.ToRgbaString());
    }

    /// <summary>
    /// Publishes a single string-grid update for one coordinate using a string payload.
    /// </summary>
    /// <param name="coordinate">Coordinate to update.</param>
    /// <param name="text">Text or color to render.</param>
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
    /// Publishes a string-grid update configured by a builder delegate.
    /// </summary>
    /// <param name="configure">Delegate that configures the update builder.</param>
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
    /// Publishes a string-grid update created from a grid structure.
    /// </summary>
    /// <param name="grid">Source grid.</param>
    /// <param name="configure">Delegate to configure each coordinate.</param>
    /// <typeparam name="T">Source grid cell type.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="grid"/> or <paramref name="configure"/> is null.</exception>
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
    /// Publishes a glyph-grid update configured by a builder delegate.
    /// </summary>
    /// <param name="configure">Delegate that configures the update builder.</param>
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
    /// Publishes a glyph-grid update created from a grid structure.
    /// </summary>
    /// <param name="grid">Source grid.</param>
    /// <param name="configure">Delegate to configure each coordinate.</param>
    /// <param name="predicate">Optional predicate to filter which cells are included.</param>
    /// <param name="clear">Whether to clear the grid before applying.</param>
    /// <typeparam name="T">Source grid cell type.</typeparam>
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
    /// Reads all currently buffered updates without awaiting further items.
    /// </summary>
    /// <returns>Enumerable of buffered updates.</returns>
    public IEnumerable<ProblemUpdate> ReadAllCurrent()
    {
        while (_updates.Reader.TryRead(out var item))
            yield return item;
    }

    /// <summary>
    /// Streams updates until a <see cref="FinishedProblemUpdate"/> is observed.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to stop reading.</param>
    /// <returns>An asynchronous stream of updates.</returns>
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

