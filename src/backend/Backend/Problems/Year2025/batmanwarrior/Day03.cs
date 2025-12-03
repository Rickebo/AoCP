using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day03 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 03);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Lobby";

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
        private readonly List<List<int>> _batteries = [];

        public Solver(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            foreach (var line in input.SplitLines())
            {
                List<int> lineValues = [];
                for (int i = 0; i < line.Length; i++)
                    lineValues.Add(line[i] - '0');
                _batteries.Add(lineValues);
            }
        }

        public int PartOne()
        {
            int joltageTotal = 0;
            foreach (var bank in _batteries)
            {
                int joltageMax = 0;
                int maxFirst = 0;
                for (int i = 0; i < bank.Count - 1; i++)
                {
                    // Skip smaller batteries
                    if (maxFirst == 9)
                        break;
                    else if (bank[i] < maxFirst)
                        continue;
                    maxFirst = bank[i];

                    int maxSecond = 0;
                    for (int j = i + 1; j < bank.Count; j++)
                    {
                        // Skip smaller batteries
                        if (maxSecond == 9)
                            break;
                        else if (bank[j] < maxSecond)
                            continue;
                        maxSecond = bank[j];
                    }

                    joltageMax = Math.Max(joltageMax, maxFirst * 10 + maxSecond);
                }

                joltageTotal += joltageMax;
            }

            return joltageTotal;
        }

        public long PartTwo()
        {
            long joltageTotal = 0;
            for (int i = 0; i < _batteries.Count; i++)
            {
                while (_batteries[i].Count > 12)
                {
                    // Look for smaller batteries
                    bool removed = false;
                    for (int j = 0; j < _batteries[i].Count - 1; j++)
                    {
                        // Remove battery
                        if (_batteries[i][j] < _batteries[i][j + 1])
                        {
                            _batteries[i].RemoveAt(j);
                            removed = true;
                            break;
                        }
                    }

                    // No battery removed, joltage optimal
                    if (!removed)
                        break;
                }

                // Evaluate joltage
                long joltageMax = 0;
                for (int j = 0; j < 12; j++)
                    joltageMax = joltageMax * 10 + _batteries[i][j];

                joltageTotal += joltageMax;
            }

            return joltageTotal;
        }
    }
}

