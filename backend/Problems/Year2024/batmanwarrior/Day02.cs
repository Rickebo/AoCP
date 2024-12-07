using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day02 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 02, 0, 0, 0);

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
            // Retrieve input reports
            string[] reports = Parser.SplitBy(input, ["\r\n", "\r", "\n"]);

            // Check for safe reports
            int safeReports = 0;
            foreach (string report in reports)
            {
                // Parse levels from report
                int[] levels = Parser.GetValues<int>(report);

                // Check if levels are safe
                bool safe = IsSafe(levels) || IsSafe(levels.Reverse());

                // Send to frontend
                reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [$"[{report}] = {safe}"]
                    }
                );

                // Accumulate safe reports
                if (safe)
                {
                    safeReports++;
                }
            }

            // Send solution to frontend
            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = safeReports.ToString()
                }
            );
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Retrieve input reports
            string[] reports = Parser.SplitBy(input, ["\r\n", "\r", "\n"]);

            // Check for safe reports
            int safeReports = 0;
            foreach (string report in reports)
            {
                // Parse levels from report
                int[] levels = Parser.GetValues<int>(report);

                // Remove one level at the time and check for safe report
                bool safe = false;
                for (int i = 0; i < levels.Length; i++)
                {
                    // Create sub array
                    int[] subLevels = levels.Where((_, index) => index != i).ToArray();

                    // Check if levels are safe
                    safe = IsSafe(subLevels) || IsSafe(subLevels.Reverse());

                    // Accumulate safe reports
                    if (safe)
                    {
                        safeReports++;
                        break;
                    }
                }

                // Send to frontend
                reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [$"[{report}] = {safe}"]
                    }
                );
            }

            // Send solution to frontend
            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = safeReports.ToString()
                }
            );
            return Task.CompletedTask;
        }
    }

    private static bool IsSafe(IEnumerable<int> levels)
    {
        // Loop through levels
        int? last = null;
        foreach (int level in levels)
        {
            // Catch first level
            if (last == null)
            {
                last = level;
                continue;
            }

            // Get difference for each level
            int difference = level - last.Value;

            // Make sure the difference is within safe specs
            if (difference > 3 || difference < 1)
            {
                return false;
            }

            last = level;
        }

        return true;
    }
}