using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Common.Updates;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems;

public class Reporter
{
    private readonly SemaphoreSlim _semaphore = new(0);
    private readonly ConcurrentQueue<ProblemUpdate> _updates = new();

    public void Report(ProblemUpdate update)
    {
        _updates.Enqueue(update);
        _semaphore.Release();
    }

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
            GlyphGridUpdate.GlyphBuilder> configure
    ) => ReportGlyphGridUpdate(
        builder => builder
            .WithWidth(grid.Width)
            .WithHeight(grid.Height)
            .WithEntries(
                grid.Coordinates,
                (glyphBuilder, coordinate) => configure(
                    glyphBuilder,
                    coordinate,
                    grid[coordinate]
                )
            )
    );

    public async Task<ProblemUpdate> Read(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        if (_updates.TryDequeue(out var update))
            return update;

        throw new Exception("Failed to dequeue update after semaphore notification.");
    }

    public IEnumerable<ProblemUpdate> ReadAllCurrent()
    {
        while (_updates.TryDequeue(out var item))
            yield return item;
    }

    public async IAsyncEnumerable<ProblemUpdate> ReadAll(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        while (true)
        {
            var update = await Read(cancellationToken: cancellationToken);

            yield return update;

            if (update is FinishedProblemUpdate)
                break;
        }
    }
}