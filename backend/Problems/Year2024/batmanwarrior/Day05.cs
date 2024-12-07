using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day05 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 05, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Print Queue";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create printer
            Printer printer = new();

            // Prase print instructions
            foreach (string row in input.SplitLines())
            {
                if (row.Contains('|'))
                {
                    // Add rule
                    printer.AddRule(row, reporter);
                }
                else
                {
                    // Try to print pages
                    printer.Print(row, reporter);
                }
            }

            // Send to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(printer.correctlyOrdered));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create printer
            Printer printer = new();

            // Prase print instructions
            foreach (string row in input.SplitLines())
            {
                if (row.Contains('|'))
                {
                    // Add rule
                    printer.AddRule(row, reporter);
                }
                else
                {
                    // Try to print pages
                    printer.OrderedPrint(row, reporter);
                }
            }

            // Send to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(printer.correctlyOrdered));
            return Task.CompletedTask;
        }
    }

    private class Printer
    {
        public Dictionary<int, List<int>> printRules = [];
        public int correctlyOrdered = 0;

        public Printer() { }

        public void AddRule(string row, Reporter reporter)
        {
            // Parse page values
            int[] pages = Parser.GetValues<int>(row);

            // Check if rule exists
            if (printRules.TryGetValue(pages[1], out List<int>? value))
            {
                // Add to existing rules
                if (!value.Contains(pages[0])) value.Add(pages[0]);
            }
            else
            {
                // Create new rule
                printRules[pages[1]] = [pages[0]];
            }

            // Send to frontend
            reporter.Report(TextProblemUpdate.FromLine($"{row} Rule added"));
        }

        public void Print(string row, Reporter reporter)
        {
            // Send to frontend
            reporter.Report(TextProblemUpdate.FromLine("\nPrinting pages:"));

            // Try to print pages
            List<int> printed = [];
            int[] pages = Parser.GetValues<int>(row);
            foreach (int page in pages)
            {
                // Check if this page has rules
                if (printRules.TryGetValue(page, out List<int>? rules))
                {
                    // Make sure all rules are satisfied
                    foreach (var rule in rules)
                    {
                        // Printing this page is against the rules
                        if (!printed.Contains(rule) && pages.Contains(rule))
                        {
                            // Send to frontend
                            reporter.Report(TextProblemUpdate.FromText(" X"));
                            reporter.Report(TextProblemUpdate.FromLine($"{row} = INCORRECT"));
                            return;
                        }
                    }
                }

                // Print page
                printed.Add(page);

                // Send to frontend
                reporter.Report(TextProblemUpdate.FromText($" {page}"));
            }

            // Send to frontend
            reporter.Report(TextProblemUpdate.FromLine($"{row} = CORRECT"));
            correctlyOrdered += pages[pages.Length / 2];
        }

        public void OrderedPrint(string row, Reporter reporter)
        {
            // Send to frontend
            reporter.Report(TextProblemUpdate.FromLine("\nPrinting pages:"));

            // Try to print pages
            List<int> printed = [];
            int[] pages = Parser.GetValues<int>(row);
            Queue<int> queue = new();
            foreach (int page in pages) queue.Enqueue(page);
            bool correct = true;
            while (queue.TryDequeue(out int page))
            {
                // Check if this page has rules
                bool allowed = true;
                if (printRules.TryGetValue(page, out List<int>? rules))
                {
                    // Make sure all rules are satisfied
                    foreach (var rule in rules)
                    {
                        // Printing this page is against the rules
                        if (!printed.Contains(rule) && pages.Contains(rule))
                        {
                            queue.Enqueue(page);
                            correct = false;
                            allowed = false;
                            break;
                        }
                    }
                }

                // Skip if page is requeued to print later
                if (!allowed) continue;

                // Print page
                printed.Add(page);

                // Send to frontend
                reporter.Report(TextProblemUpdate.FromText($" {page}"));
            }

            // Send to frontend
            string[] lines =
            [
                $"{row} = {(correct ? "CORRECT" : "INCORRECT")}",
                $"{(!correct ? string.Join(",", printed) + " (FIXED)" : "")}"
            ];
            reporter.Report(TextProblemUpdate.FromLines(lines));
            correctlyOrdered += !correct ? printed[printed.Count / 2] : 0;
        }
    }
}