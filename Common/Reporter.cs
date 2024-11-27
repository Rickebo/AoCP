using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Common.Updates;

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

    public async Task<ProblemUpdate> Read(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        if (_updates.TryDequeue(out var update))
            return update;

        throw new Exception("Failed to dequeue update after semaphore notification.");
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