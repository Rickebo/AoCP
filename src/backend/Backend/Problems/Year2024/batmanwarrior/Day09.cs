using System;
using System.Collections.Generic;
using Common;
using Common.Updates;
using System.Threading.Tasks;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day09 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 09);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Disk Fragmenter";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create drive
            Drive drive = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(drive.Checksum(fragmented: true)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create drive
            Drive drive = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(drive.Checksum(fragmented: false)));
            return Task.CompletedTask;
        }
    }

    public class Drive
    {
        private readonly Reporter _reporter;
        private readonly LinkedList<Memory> _memory = [];

        public Drive(string data, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Save file information for printing
            Dictionary<int, int> spaces = new() { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };
            Dictionary<int, int> files = new() { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 } };

            // Store files or free space in memory
            int fileID = 0;
            bool free = false;
            for (int i = 0; i < data.Length; i++)
            {
                // If anything else than digits
                if (!char.IsDigit(data[i]))
                    continue;

                // Memory size
                int size = data[i] - '0';

                // If memory has size
                if (size > 0)
                {
                    // Create new memory
                    Memory memory = new(free ? -1 : fileID, size);

                    // Store memory
                    _memory.AddLast(memory);
                }

                // Store memory information
                if (free)
                {
                    // Free space
                    spaces[size]++;
                }
                else
                {
                    // File
                    files[size]++;
                }

                // Increment file ID if memory not free
                if (!free)
                    fileID++;

                // Toggle free memory
                free = !free;
            }

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"{fileID} files added to drive\n\nFILES:\nSize | Count"));
            foreach (KeyValuePair<int, int> pair in files) _reporter.Report(TextProblemUpdate.FromLine($"{pair.Key,-7}{pair.Value}"));
            _reporter.Report(TextProblemUpdate.FromLine($"\nFREE SPACE:\nSize | Count"));
            foreach (KeyValuePair<int, int> pair in spaces) _reporter.Report(TextProblemUpdate.FromLine($"{pair.Key,-7}{pair.Value}"));
        }

        public void Compact(bool fragmented)
        {
            // No memory to compact
            if (_memory.First == null || _memory.Last == null)
                return;

            // Fragment memory
            if (fragmented)
            {
                // Fragment all memory
                LinkedListNode<Memory>? node = _memory.First;
                while (node != null)
                {
                    // Push memory ahead but leave one size behind
                    if (node.Value.Size > 1)
                    {
                        _memory.AddAfter(node, new Memory(node.Value.Value, node.Value.Size - 1));
                        node.Value.Size = 1;
                    }

                    // Get next memory
                    node = node.Next;
                }
            }

            // Retrieve first and last memory
            LinkedListNode<Memory> left = _memory.First;
            LinkedListNode<Memory> right = _memory.Last;

            // Compact loop
            for (;;)
            {
                // Go to first memory that is not free from right
                while (right.Value.Value == -1)
                {
                    // Check if no more memory left to move
                    if (right.Previous == null || right == left)
                        return;

                    // Update right side
                    right = right.Previous;
                }

                // Look for space to move memory
                for (LinkedListNode<Memory> node = left; node.Next != null && node != right; node = node.Next)
                {
                    // Check if memory is free and has room
                    if (node.Value.Value == -1 && node.Value.Size >= right.Value.Size)
                    {
                        // Move memory
                        MoveMemory(node, right);

                        // Update left to leftmost free space
                        while (left.Value.Value != -1)
                        {
                            // Return if no more free space
                            if (left.Next == null || left == right)
                                return;

                            // Move left node
                            left = left.Next;
                        }

                        // Break and look for next memory to move
                        break;
                    }
                }

                // Check if swap was not possible
                if (right.Value.Value != -1)
                {
                    // Skip this memory
                    if (right.Previous == null || right.Previous == left)
                        return;
                    else
                        right = right.Previous;
                }
            }
        }

        private void MoveMemory(LinkedListNode<Memory> a, LinkedListNode<Memory> b)
        {
            // If a has space left, add empty memory after a
            if (b.Value.Size < a.Value.Size)
                _memory.AddAfter(a, new Memory(-1, a.Value.Size - b.Value.Size));

            // Swap values and update size of a
            a.Value.Value = b.Value.Value;
            a.Value.Size = b.Value.Size;

            // Empty memory of b
            b.Value.Value = -1;
        }

        public long Checksum(bool fragmented)
        {
            // No memory to checksum
            if (_memory.First == null)
                return 0;

            // Compact memory
            Compact(fragmented);

            // Calculate checksum
            long sum = 0;
            long memoryIndex = 0;
            for (LinkedListNode<Memory> node = _memory.First; node.Next != null; node = node.Next)
            {
                // Retrieve the correct memory indexes for checksum
                for (int i = 0; i < node.Value.Size; i++)
                {
                    // Only count memory with value
                    if (node.Value.Value != -1)
                        sum += node.Value.Value * memoryIndex;

                    // Increment index
                    memoryIndex++;
                }
            }

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"\nCompacted memory checksum with {(fragmented ? "file blocks" : "whole files")}: {sum}"));

            return sum;
        }
    }

    public class Memory(int val, int size)
    {
        public int Value = val;
        public int Size = size;
    }
}

