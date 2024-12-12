using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day02 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 02);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Red-Nosed Reports";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create report manager
            ReportManager reportManager = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(reportManager.SafeReports(tolerateBadLevel: false)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create report manager
            ReportManager reportManager = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(reportManager.SafeReports(tolerateBadLevel: true)));
            return Task.CompletedTask;
        }
    }

    public class ReportManager
    {
        private readonly Reporter _reporter;
        private readonly List<int[]> _reports = [];
        private const int _maxDelta = 3;
        private const int _minDelta = 1;

        public ReportManager(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Retrieve reports from input
            foreach (string row in input.SplitLines())
                _reports.Add(Parser.GetValues<int>(row));
        }

        public int SafeReports(bool tolerateBadLevel)
        {
            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"Bad level tolerated: {tolerateBadLevel}\n\nSafe  | Report"));

            // Check reports
            int safeReports = 0;
            foreach (int[] report in _reports)
            {
                // Loop once for each level in report
                bool safe = false;
                for (int i = 0; i < report.Length; i++)
                {
                    // Copy report but exclude one level if tolerated
                    int[] reportCopy = report.Where((_, index) => index != i || !tolerateBadLevel).ToArray();

                    // Check if levels are safe
                    safe = IsSafe(reportCopy) || IsSafe(reportCopy.Reverse().ToArray());

                    // Accumulate safe reports
                    safeReports += safe ? 1 : 0;

                    // If report is safe or no bad level is tolerated
                    if (safe || !tolerateBadLevel)
                        break;
                }

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"{safe,-5} | {string.Join(" ", report)}"));
            }

            return safeReports;
        }

        private static bool IsSafe(int[] levels)
        {
            // Loop through levels
            for (int i = 1; i < levels.Length; i++)
            {
                // Get difference for each level
                int difference = levels[i] - levels[i - 1];

                // Check if difference is safe
                if (difference > _maxDelta || difference < _minDelta)
                    return false;
            }

            return true;
        }
    }
}