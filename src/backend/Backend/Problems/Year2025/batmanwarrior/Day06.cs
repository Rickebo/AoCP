using Common;
using Lib.Grids;
using Lib.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day06 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 06);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Trash Compactor";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Send solution to frontend (Part 1)
            reporter.ReportSolution(new Solver(input, reporter, 1).PartOne());
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Send solution to frontend (Part 2)
            reporter.ReportSolution(new Solver(input, reporter, 2).PartTwo());
            return Task.CompletedTask;
        }
    }

    public class Solver
    {
        private readonly Reporter _reporter;
        private readonly List<List<long>> _values = [];
        private readonly List<char> _operators = [];

        public Solver(string input, Reporter reporter, int part)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            if (part == 1)
            {
                // Operators
                string[] lines = input.SplitLines();
                foreach (var c in lines[^1].Split())
                    _operators.AddRange(c);

                // Values
                for (int i = 0; i < lines.Length - 1; i++)
                    _values.Add([.. Parser.GetValues<long>(lines[i])]);
            }
            else if (part == 2)
            {
                // Operators
                string[] lines = input.SplitLines();
                foreach (var c in lines[^1].Split())
                    _operators.AddRange(c);

                // Retrieve columns
                int rows = lines.Length - 1;
                int cols = lines[0].Length;
                List<long> values = [];
                for (int x = 0; x < cols; x++)
                {
                    // Get digit characters
                    StringBuilder sb = new();
                    for (int y = 0; y < rows; y++)
                        if (lines[y][x] != ' ')
                            sb.Append(lines[y][x]);

                    // Parse to long
                    if (sb.Length > 0)
                        values.Add(long.Parse(sb.ToString()));

                    // Empty or last column
                    if (sb.Length == 0 || x == cols - 1)
                    {
                        _values.Add(values);
                        values = [];
                    }
                }
            }
        }

        public long PartOne()
        {
            long total = 0;

            int rows = _values.Count;
            int cols = _values[0].Count;

            for (int col = 0; col < cols; col++)
            {
                long columnValue = 0;
                for (int row = 0; row < rows; row++)
                {
                    if (columnValue == 0)
                        columnValue = _values[row][col];
                    else if (_operators[col] == '+')
                        columnValue += _values[row][col];
                    else if (_operators[col] == '*')
                        columnValue *= _values[row][col];
                }
                total += columnValue;
            }

            return total;
        }

        public long PartTwo()
        {
            long total = 0;

            int rows = _values.Count;

            for (int row = 0; row < rows; row++)
            {
                long columnValue = 0;
                for (int i = 0; i < _values[row].Count; i++)
                {
                    if (columnValue == 0)
                        columnValue = _values[row][i];
                    else if (_operators[row] == '+')
                        columnValue += _values[row][i];
                    else if (_operators[row] == '*')
                        columnValue *= _values[row][i];
                }
                total += columnValue;
            }

            return total;
        }
    }
}

