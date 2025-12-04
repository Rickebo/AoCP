using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Lib.Collections;

/// <summary>
/// Minimal binary heap priority queue that works on any comparable priority type.
/// .NET ships a PriorityQueue in newer runtimes, but having an explicit implementation here keeps
/// AoC solutions self-contained and avoids cross-targetting issues.
/// </summary>
public class PriorityQueue<TElement, TPriority> : IEnumerable<TElement>
    where TPriority : IComparable<TPriority>
{
    private readonly List<(TElement Element, TPriority Priority)> _heap = [];
    private readonly IComparer<TPriority> _comparer;

    /// <summary>
    /// Initializes a new priority queue using the default comparer for <typeparamref name="TPriority"/>.
    /// </summary>
    public PriorityQueue()
        : this(Comparer<TPriority>.Default) { }

    /// <summary>
    /// Initializes a new priority queue with a custom priority comparer.
    /// </summary>
    /// <param name="comparer">Comparer used to order priorities.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
    public PriorityQueue(IComparer<TPriority> comparer)
    {
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    /// <summary>
    /// Gets the number of elements contained in the queue.
    /// </summary>
    public int Count => _heap.Count;

    /// <summary>
    /// Adds an element with the specified priority to the queue.
    /// </summary>
    /// <param name="element">Element to enqueue.</param>
    /// <param name="priority">Priority associated with the element.</param>
    public void Enqueue(TElement element, TPriority priority)
    {
        _heap.Add((element, priority));
        BubbleUp(_heap.Count - 1);
    }

    /// <summary>
    /// Attempts to remove and return the element with the highest priority (lowest value) from the queue.
    /// </summary>
    /// <param name="element">The removed element if the operation succeeded; otherwise the default value.</param>
    /// <param name="priority">The priority of the removed element if the operation succeeded; otherwise the default value.</param>
    /// <returns><c>true</c> if an element was dequeued; otherwise <c>false</c>.</returns>
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

    /// <summary>
    /// Removes and returns the element with the highest priority (lowest value) from the queue.
    /// </summary>
    /// <returns>The dequeued element.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the queue is empty.</exception>
    public TElement Dequeue()
    {
        if (!TryDequeue(out var element, out _))
            throw new InvalidOperationException("Queue is empty");

        return element!;
    }

    /// <summary>
    /// Attempts to return the element with the highest priority without removing it from the queue.
    /// </summary>
    /// <param name="element">The element at the front of the queue if present; otherwise the default value.</param>
    /// <param name="priority">The priority of the element if present; otherwise the default value.</param>
    /// <returns><c>true</c> if an element exists; otherwise <c>false</c>.</returns>
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

    /// <summary>
    /// Returns an enumerator that iterates through the queue elements in heap order.
    /// </summary>
    public IEnumerator<TElement> GetEnumerator() => _heap.Select(static h => h.Element).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Removes the root of the heap and restores heap invariants.
    /// </summary>
    private void RemoveRoot()
    {
        var lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);
        SinkDown(0);
    }

    /// <summary>
    /// Restores heap ordering by moving the element at <paramref name="index"/> up the tree.
    /// </summary>
    /// <param name="index">Index of the element to bubble up.</param>
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

    /// <summary>
    /// Restores heap ordering by moving the element at <paramref name="index"/> down the tree.
    /// </summary>
    /// <param name="index">Index of the element to sink.</param>
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

    /// <summary>
    /// Swaps two elements in the underlying heap list.
    /// </summary>
    /// <param name="a">Index of the first element.</param>
    /// <param name="b">Index of the second element.</param>
    private void Swap(int a, int b)
    {
        (_heap[a], _heap[b]) = (_heap[b], _heap[a]);
    }

    /// <summary>
    /// Compares the priorities at two heap indices using the configured comparer.
    /// </summary>
    /// <param name="a">Index of the first heap element.</param>
    /// <param name="b">Index of the second heap element.</param>
    /// <returns>A signed integer indicating relative priority ordering.</returns>
    private int Compare(int a, int b)
    {
        return _comparer.Compare(_heap[a].Priority, _heap[b].Priority);
    }
}

