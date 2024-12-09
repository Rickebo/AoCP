using Common;
using Common.Updates;
using Lib;
using System;

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

        public override async Task Solve(string input, Reporter reporter)
        {
            FileSystemDumb fs = new(input, reporter);

            fs.Compact();

            reporter.Report(FinishedProblemUpdate.FromSolution(fs.Checksum()));
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override async Task Solve(string input, Reporter reporter)
        {
            FileSystemDumb fs = new(input, reporter);

            fs.CompactSmart();

            reporter.Report(FinishedProblemUpdate.FromSolution(fs.ChecksumSmart()));
        }
    }

    public class FileSystem
    {
        private readonly Reporter _reporter;
        private readonly List<File> _files = [];
        private readonly List<File> _emptySpaces = [];
        private readonly List<int> _fileInts = [];

        public FileSystem(string input, Reporter reporter)
        {
            _reporter = reporter;
            int fileValue = 0;
            bool isEmpty = false;
            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < input[i] - '0'; j++)
                {
                    File file = new(isEmpty ? -1 : fileValue);
                    _files.Add(file);
                    if (isEmpty) _emptySpaces.Add(file);
                }
                if (!isEmpty) fileValue++;
                isEmpty = !isEmpty;
            }
        }

        public void Compact()
        {
            // Look from end of files
            int emptyIndex = 0;
            for (int i = _files.Count - 1; i >= 0; i--)
            {
                // Skip empty spaces
                if (_files[i].Value == -1) continue;

                // Stop if no more empty spaces left
                if (emptyIndex == _emptySpaces.Count) break;

                int val = _files[i].Value;
                _files[i].Value = -1;
                _emptySpaces[emptyIndex].Value = val;
                emptyIndex++;
            }
        }

        public long Checksum()
        {
            int count = 0;
            foreach (File file in _files)
            {
                if (file.Value != -1) count++;
                else break;
            }

            long sum = 0;
            for (int i = 0; i < _files.Count; i++)
            {
                // If empty spaces reached (compacted end of files)
                if (_files[i].Value == -1) break;

                // Accumulate
                sum += (long)_files[i].Value * i;
            }
            return sum;
        }


    }

    public class File(int value)
    {
        public int Value = value;
    }

    public class FileSystemDumb
    {
        private readonly Reporter _reporter;
        private readonly List<int> _files = [];

        public FileSystemDumb(string input, Reporter reporter)
        {
            _reporter = reporter;

            int fileValue = 0;
            bool isEmpty = false;
            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < input[i] - '0'; j++)
                {
                    _files.Add(isEmpty ? -1 : fileValue);
                }
                if (!isEmpty) fileValue++;
                isEmpty = !isEmpty;
            }
        }

        public void Compact()
        {
            // Look from end of files
            int curr = 0;
            int end = _files.Count - 1;
            for (int i = end; i >= 0; i--)
            {
                // Skip empty spaces
                if (_files[i] == -1) continue;

                // Look for empty spot
                for (; curr <= end; curr++)
                {
                    if (curr == i) break;

                    if (_files[curr] == -1)
                    {
                        // Swap values
                        int val = _files[i];
                        _files[i] = -1;
                        _files[curr] = val;
                        break;
                    }
                }

                if (curr == i) break;
            }
        }

        public void CompactSmart()
        {
            for (int i = _files.Count - 1; i >= 0; i--)
            {
                // Skip empty
                if (_files[i] == -1) continue;

                // Size of file
                int size = ValLength(_files[i], i, -1);

                // Look for empty
                bool inserted = false;
                for (int j = 0; j < _files.Count; j++)
                {
                    // Break if surpassing i
                    if (j >= i) break;

                    // Skip occupied
                    if (_files[j] != -1) continue;

                    // Size of space
                    int space = ValLength(_files[j], j, 1);

                    if (size <= space)
                    {
                        InsertAt(j, size, _files[i]);
                        RemoveAt(i, size);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                {
                    i -= size - 1;
                }
            }
        }

        public int Spaces(int index)
        {
            int spaces = 0;
            for (int i = index; i < _files.Count; i++)
            {
                if (_files[i] == 0) spaces++;
                else break;
            }

            return spaces;
        }

        public int ValLength(int val, int index, int delta)
        {
            int length = 0;
            if (delta > 0)
            {
                for (int i = index; i < _files.Count; i += delta)
                {
                    if (_files[i] == val) length++;
                    else break;
                }
            }
            else
            {
                for (int i = index; i >= 0; i += delta)
                {
                    if (_files[i] == val) length++;
                    else break;
                }
            }
            return length;
        }

        public void InsertAt(int index, int count, int val)
        {
            for (int i = index; i < _files.Count && count > 0; i++)
            {
                _files[i] = val;
                count--;
            }
        }

        public void RemoveAt(int index, int count)
        {
            for (int i = index; i >= 0 && count > 0; i--)
            {
                _files[i] = -1;
                count--;
            }
        }

        public long Checksum()
        {
            long sum = 0;
            for (int i = 0; i < _files.Count; i++)
            {
                // If empty spaces reached (compacted end of files)
                if (_files[i] == -1) break;

                // Accumulate
                sum += (long)_files[i] * i;
            }
            return sum;
        }

        public long ChecksumSmart()
        {
            long sum = 0;
            for (int i = 0; i < _files.Count; i++)
            {
                // Skip empty
                if (_files[i] == -1) continue;
                // Accumulate
                sum += (long)_files[i] * i;
            }
            return sum;
        }
    }
}