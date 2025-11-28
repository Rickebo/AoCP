using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib.Extensions;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day25 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 25);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Code Chronicle";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create office floor
            OfficeFloor officeFloor = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(officeFloor.FittingPairs()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution("Boom ez"));
            return Task.CompletedTask;
        }
    }

    public class OfficeFloor
    {
        private readonly Reporter _reporter;
        private readonly List<int[]> _locks = [];
        private readonly List<int[]> _keys = [];

        public OfficeFloor(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Split input into nicely packed rows
            string[] rows = input.SplitLines();

            // All schematics are width 5 and height 7
            const int width = 5;
            const int height = 7;
            for (int i = 0; i < rows.Length; i += height)
            {
                // Retrieve schematic column values
                int[] vals = new int[width];
                for (int x = 0; x < width; x++)
                {
                    int val = 0;
                    for (int y = 0; y < (height - 2); y++)
                    {
                        if (rows[i + 1 + y][x] == '#')
                            val++;
                    }
                    vals[x] = val;
                }

                // Check if this schematic is a lock or key
                if (rows[i][0] == '#')
                {
                    // Lock
                    _locks.Add(vals);
                }
                else
                {
                    // Key
                    _keys.Add(vals);
                }
            }
        }

        public long FittingPairs()
        {
            long fitting = 0;
            foreach (int[] lockVals in _locks)
            {
                foreach (int[] keyVals in _keys)
                {
                    if (Fit(keyVals, lockVals))
                        fitting++;
                }
            }

            return fitting;
        }

        private static bool Fit(int[] keyVals, int[] lockVals)
        {
            for (int i = 0; i < lockVals.Length; i++)
            {
                if (keyVals[i] + lockVals[i] > 5)
                    return false;
            }

            return true;
        }
    }
}