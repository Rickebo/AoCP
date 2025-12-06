using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Lib.Collections;

/// <summary>
/// Simple binary heap based priority queue that orders items by a comparer on <typeparamref name="TPriority"/>.
/// </summary>
/// <typeparam name="TElement">Type of items stored in the queue.</typeparam>
/// <typeparam name="TPriority">Priority value type used for ordering.</typeparam>
public class PriorityQueue<TElement, TPriority> : IEnumerable<TElement>
    where TPriority : IComparable<TPriority>
{
    private readonly List<(TElement Element, TPriority Priority)> _heap = [];
    private readonly IComparer<TPriority> _comparer;

    /// <summary>
    /// Creates a queue that orders priorities using the default comparer.
    /// </summary>
    public PriorityQueue()
        : this(Comparer<TPriority>.Default) { }

    /// <summary>
    /// Creates a queue that orders priorities using the provided comparer.
    /// </summary>
    /// <param name="comparer">Comparer to use when ordering priorities.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparer"/> is <see langword="null"/>.</exception>
    public PriorityQueue(IComparer<TPriority> comparer)
    {
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    /// <summary>
    /// Gets the number of elements currently stored in the queue.
    /// </summary>
    public int Count => _heap.Count;

    /// <summary>
    /// Adds a new element with the associated <paramref name="priority"/> to the queue.
    /// </summary>
    /// <param name="element">Element to store.</param>
    /// <param name="priority">Priority used to order the element.</param>
    public void Enqueue(TElement element, TPriority priority)
    {
        _heap.Add((element, priority));
        BubbleUp(_heap.Count - 1);
    }

    /// <summary>
    /// Attempts to remove and return the element with the smallest priority.
    /// </summary>
    /// <param name="element">When the method returns, contains the dequeued element if available.</param>
    /// <param name="priority">When the method returns, contains the dequeued priority if available.</param>
    /// <returns><see langword="true"/> when an element was dequeued; otherwise <see langword="false"/>.</returns>
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
    /// Removes and returns the element with the smallest priority.
    /// </summary>
    /// <returns>The element at the head of the queue.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the queue is empty.</exception>
    public TElement Dequeue()
    {
        if (!TryDequeue(out var element, out _))
            throw new InvalidOperationException("Queue is empty");

        return element!;
    }

    /// <summary>
    /// Attempts to peek at the element with the smallest priority without removing it.
    /// </summary>
    /// <param name="element">When the method returns, contains the element if available.</param>
    /// <param name="priority">When the method returns, contains the priority if available.</param>
    /// <returns><see langword="true"/> if an element exists; otherwise <see langword="false"/>.</returns>
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
    /// Returns an enumerator that iterates through all elements in the queue in heap order.
    /// </summary>
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

