using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Lib.Collections;

public class PriorityQueue<TElement, TPriority> : IEnumerable<TElement>
    where TPriority : IComparable<TPriority>
{
    private readonly List<(TElement Element, TPriority Priority)> _heap = [];
    private readonly IComparer<TPriority> _comparer;

    public PriorityQueue()
        : this(Comparer<TPriority>.Default) { }

    public PriorityQueue(IComparer<TPriority> comparer)
    {
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    public int Count => _heap.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        _heap.Add((element, priority));
        BubbleUp(_heap.Count - 1);
    }

    public bool TryDequeue([MaybeNullWhen(false)] out TElement element, out TPriority priority)
    {
        if (_heap.Count == 0)
        {
            element = default;
            priority = default!;
            return false;
        }

        (element, priority) = _heap[0];
        RemoveRoot();
        return true;
    }

    public TElement Dequeue()
    {
        if (!TryDequeue(out var element, out _))
            throw new InvalidOperationException("Queue is empty");

        return element!;
    }

    public bool TryPeek([MaybeNullWhen(false)] out TElement element, out TPriority priority)
    {
        if (_heap.Count == 0)
        {
            element = default;
            priority = default!;
            return false;
        }

        (element, priority) = _heap[0];
        return true;
    }

    public IEnumerator<TElement> GetEnumerator() => _heap.Select(static h => h.Element).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void RemoveRoot()
    {
        var lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);
        SinkDown(0);
    }

    private void BubbleUp(int index)
    {
        while (index > 0)
        {
            var parentIndex = (index - 1) / 2;
            if (Compare(index, parentIndex) >= 0)
                break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void SinkDown(int index)
    {
        while (true)
        {
            var left = index * 2 + 1;
            var right = left + 1;
            var smallest = index;

            if (left < _heap.Count && Compare(left, smallest) < 0)
                smallest = left;
            if (right < _heap.Count && Compare(right, smallest) < 0)
                smallest = right;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        (_heap[a], _heap[b]) = (_heap[b], _heap[a]);
    }

    private int Compare(int a, int b)
    {
        return _comparer.Compare(_heap[a].Priority, _heap[b].Priority);
    }
}

