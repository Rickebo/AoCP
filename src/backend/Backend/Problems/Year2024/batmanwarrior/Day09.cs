using System;
using System.Collections.Generic;
using Common;
using Common.Updates;
using System.Text;
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
            Drive drive = new(input, MemoryMode.Partial, reporter);

            // Compact memory
            drive.Compact();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(drive.Checksum()));
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
            Drive drive = new(input, MemoryMode.Full, reporter);

            // Compact memory
            drive.Compact();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(drive.Checksum()));
            return Task.CompletedTask;
        }
    }

    public class Drive
    {
        private readonly MemoryMode _mode;
        private readonly LinkedList<Memory> _memory = [];
        private readonly Reporter _reporter;

        public Drive(string data, MemoryMode mode, Reporter reporter)
        {
            // Save reporter for frontend printing
            _reporter = reporter;

            // Set operation mode
            _mode = mode;

            // Create and store memory from data
            int fileID = 0;
            bool free = false;
            for (int i = 0; i < data.Length; i++)
            {
                // Memory size
                int size = data[i] - '0';

                // Create and link memory
                for (int j = 0; j < size; j++)
                {
                    // Create new memory
                    Memory memory = new(free ? -1 : fileID, _mode == MemoryMode.Full ? size : 1);

                    // Store memory
                    _memory.AddLast(memory);

                    // Break if operating in full memory mode
                    if (_mode == MemoryMode.Full)
                        break;
                }

                // Increment file ID if memory not free
                if (!free)
                    fileID++;

                // Toggle free memory
                free = !free;
            }

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine(MemoryToStr()));
        }

        private string MemoryToStr()
        {
            StringBuilder sb = new();
            foreach (Memory memory in _memory)
            {
                for (int i = 0; i < memory.Size; i++)
                {
                    if (sb.Length > 0) sb.Append('|');
                    sb.Append(memory.Value == -1 ? "." : memory.Value);
                }
            }

            return sb.ToString();
        }

        public void Compact()
        {
            // No memory to compact
            if (_memory.First == null || _memory.Last == null)
                return;

            // Retrieve first and last memory
            LinkedListNode<Memory> left = _memory.First;
            LinkedListNode<Memory> right = _memory.Last;

            // Compact loop
            for (;;)
            {
                string memoryStr = MemoryToStr();

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
                    if (right.Previous == null || right.Previous == left || left == right)
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

        public long Checksum()
        {
            // No memory to checksum
            if (_memory.First == null)
                return 0;

            // Calculate checksum
            long sum = 0;
            long memoryIndex = 0;
            for (LinkedListNode<Memory> node = _memory.First; node.Next != null; node = node.Next, memoryIndex++)
            {
                // Skip empty
                if (node.Value.Value == -1)
                    continue;

                // Add to sum
                sum += node.Value.Value * memoryIndex;
            }
            return sum;
        }
    }

    public enum MemoryMode
    {
        Full,
        Partial
    }

    public class Memory(int val, int size)
    {
        public int Value = val;
        public int Size = size;
    }
}