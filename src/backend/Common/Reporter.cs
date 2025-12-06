using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Common.Updates;
using Lib.Geometry;
using Lib.Grids;
using Lib.Color;

namespace Common;

public class Reporter
{
    private readonly Channel<ProblemUpdate> _updates = Channel.CreateUnbounded<ProblemUpdate>();

    public void Report(ProblemUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);

        if (!_updates.Writer.TryWrite(update))
            throw new InvalidOperationException("Failed to write update to channel.");
    }

    public void ReportLine(string line) =>
        ReportLines([line ?? string.Empty]);

    public void ReportLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ReportText(lines: lines);
    }

    public void ReportText(string? text = null, IEnumerable<string>? lines = null) =>
        Report(new TextProblemUpdate(){ Text = text, Lines = lines?.ToArray() });

    public void ReportSolution(IFormattable solution)
    {
        ArgumentNullException.ThrowIfNull(solution);
        ReportSolution(solution.ToString() ?? string.Empty);
    }

    public void ReportSolution(string solution) =>
        Report(FinishedProblemUpdate.FromSolution(solution));

    public void ReportError(string error, string? partialSolution = null) =>
        Report(FinishedProblemUpdate.FromError(error, partialSolution));

    public void ReportStringGridUpdate(IStringCoordinate coordinate, Color color)
    {
        ArgumentNullException.ThrowIfNull(coordinate);
        ReportStringGridUpdate(coordinate, color.ToRgbaString());
    }

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

    public void ReportStringGridUpdate(
        Func<
            StringGridUpdate.StringGridUpdateBuilder,
            StringGridUpdate.StringGridUpdateBuilder
        > configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        Report(configure(StringGridUpdate.Builder()).Build());
    }

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

    public IEnumerable<ProblemUpdate> ReadAllCurrent()
    {
        while (_updates.Reader.TryRead(out var item))
            yield return item;
    }

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

