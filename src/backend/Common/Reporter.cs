using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Common.Updates;
using Lib.Coordinate;
using Lib.Grid;

namespace Common;

public class Reporter
{
    private readonly Channel<ProblemUpdate> _updates = Channel.CreateUnbounded<ProblemUpdate>();

    public void Report(ProblemUpdate update)
    {
        if (!_updates.Writer.TryWrite(update))
            throw new Exception("Failed to write update to channel.");
    }

    public void ReportLine(string line) => ReportText(lines: [line]);

    public void ReportText(string? text = null, string[]? lines = null) => Report(
        new TextProblemUpdate()
        {
            Text = text,
            Lines = lines
        }
    );

    public void ReportSolution(IFormattable solution) =>
        ReportSolution(solution.ToString() ?? "");

    public void ReportSolution(string solution) =>
        Report(FinishedProblemUpdate.FromSolution(solution));

    public void ReportStringGridUpdate(IStringCoordinate coordinate, string text) =>
        ReportStringGridUpdate(
            builder => builder
                .WithEntry(
                    txt => txt
                        .WithText(text)
                        .WithCoordinate(coordinate)
                )
        );

    public void ReportStringGridUpdate(
        Func<
            StringGridUpdate.StringGridUpdateBuilder,
            StringGridUpdate.StringGridUpdateBuilder
        > configure
    ) => Report(configure(StringGridUpdate.Builder()).Build());

    public void ReportStringGridUpdate<T>(
        ArrayGrid<T> grid,
        Func<
                StringGridUpdate.StringCoordinateBuilder,
                IntegerCoordinate<int>,
                T,
                StringGridUpdate.StringCoordinateBuilder
            >
            configure
    ) => ReportStringGridUpdate(
        builder => builder
            .WithWidth(grid.Width)
            .WithHeight(grid.Height)
            .WithEntries(
                grid.Coordinates,
                (stringUpdateBuilder, coordinate) => configure(
                    stringUpdateBuilder,
                    coordinate,
                    grid[coordinate]
                )
            )
    );

    public void ReportGlyphGridUpdate(
        Func<
            GlyphGridUpdate.GlyphGridUpdateBuilder,
            GlyphGridUpdate.GlyphGridUpdateBuilder
        > configure
    ) => Report(configure(GlyphGridUpdate.Builder()).Build());

    public void ReportGlyphGridUpdate<T>(
        ArrayGrid<T> grid,
        Func<GlyphGridUpdate.GlyphBuilder, IntegerCoordinate<int>, T,
            GlyphGridUpdate.GlyphBuilder> configure,
        Func<IntegerCoordinate<int>, T, bool>? predicate = null,
        bool clear = false
    ) => ReportGlyphGridUpdate(
        builder => builder
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
            )
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