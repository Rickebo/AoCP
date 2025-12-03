namespace Lib.Collections.Tests;

public class PriorityQueueTests
{
    [Test]
    public void TryDequeue_OnEmptyQueueReturnsFalse()
    {
        var queue = new Lib.Collections.PriorityQueue<string, int>();

        var result = queue.TryDequeue(out var element, out var priority);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(element, Is.Null);
            Assert.That(priority, Is.EqualTo(default(int)));
            Assert.That(queue.Count, Is.EqualTo(0));
        });
    }

    [Test]
    public void EnqueueAndDequeue_ReturnItemsByPriority()
    {
        var queue = new Lib.Collections.PriorityQueue<string, int>();

        queue.Enqueue("middle", 5);
        queue.Enqueue("low", 1);
        queue.Enqueue("high", 10);

        Assert.Multiple(() =>
        {
            Assert.That(queue.Count, Is.EqualTo(3));
            Assert.That(queue.Dequeue(), Is.EqualTo("low"));
            Assert.That(queue.Count, Is.EqualTo(2));
        });

        Assert.That(queue.TryPeek(out var peek, out var peekPriority), Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(peek, Is.EqualTo("middle"));
            Assert.That(peekPriority, Is.EqualTo(5));
        });
    }

    [Test]
    public void Enumerator_ContainsAllElements()
    {
        var queue = new Lib.Collections.PriorityQueue<int, int>();
        queue.Enqueue(1, 1);
        queue.Enqueue(2, 2);
        queue.Enqueue(3, 3);

        var enumerated = queue.ToList();

        CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, enumerated);
    }

    [Test]
    public void CustomComparer_ReversesPriorityOrder()
    {
        var queue = new Lib.Collections.PriorityQueue<int, int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));

        queue.Enqueue(1, 1);
        queue.Enqueue(2, 2);

        Assert.That(queue.Dequeue(), Is.EqualTo(2));
    }
}

