using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day05 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 05);

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
            Printer printer = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(printer.Print(orderPages: false)));
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
            Printer printer = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(printer.Print(orderPages: true)));
            return Task.CompletedTask;
        }
    }

    private class Printer
    {
        private readonly Reporter _reporter;
        private readonly Dictionary<int, List<int>> _printRules = [];
        private readonly List<int[]> _printQueue = [];

        public Printer(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Parse print instructions
            foreach (string row in input.SplitLines())
            {
                if (row.Contains('|'))
                {
                    // Add rule
                    AddRule(Parser.GetValues<int>(row));
                }
                else
                {
                    // Add to print queue
                    _printQueue.Add(Parser.GetValues<int>(row));
                }
            }
        }

        public void AddRule(int[] pages)
        {
            // Check if page has rules
            if (_printRules.TryGetValue(pages[1], out List<int>? value))
            {
                // Add to existing rules
                if (!value.Contains(pages[0]))
                    value.Add(pages[0]);
            }
            else
            {
                // Create new rule
                _printRules[pages[1]] = [pages[0]];
            }

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"{pages[0]}|{pages[1]} Rule added"));
        }

        public int Print(bool orderPages)
        {
            // Try to print queue
            int sum = 0;
            foreach (int[] printQueue in _printQueue)
            {
                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine("\nPrinting pages:"));

                // Keep track of printed pages
                List<int> printed = [];

                // Create queue
                Queue<int> queue = [];

                // Enqueue pages from print queue
                foreach (int page in printQueue)
                    queue.Enqueue(page);

                // Start printing
                bool wrongOrder = false;
                while (queue.TryDequeue(out var page))
                {
                    // Get print rules for this page
                    bool canPrint = true;
                    if (_printRules.TryGetValue(page, out List<int>? rules))
                    {
                        // Check if rules are satisfied
                        foreach (var rule in rules)
                        {
                            // Printing this page is against the rules
                            if (!printed.Contains(rule) && printQueue.Contains(rule))
                            {
                                // Put page back into print queue
                                if (orderPages) queue.Enqueue(page);

                                // Update print state
                                wrongOrder = true;
                                canPrint = false;
                                break;
                            }
                        }
                    }

                    // If page is requeued to print later
                    if (!canPrint && orderPages)
                        continue;

                    // If wrong order and not set to order pages
                    if (!canPrint && !orderPages)
                    {
                        // Send to frontend
                        _reporter.Report(TextProblemUpdate.FromText(" X"));
                        break;
                    }

                    // Print page
                    if (!printed.Contains(page))
                        printed.Add(page);

                    // Send to frontend
                    _reporter.Report(TextProblemUpdate.FromText($" {page}"));
                }

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"{string.Join(",", printQueue)} | {(wrongOrder ? "INCORRECT" : "CORRECT")}"));
                if (wrongOrder && orderPages) _reporter.Report(TextProblemUpdate.FromLine($"{string.Join(",", printed)} | FIXED"));

                // Add to sum
                if (wrongOrder && orderPages || !wrongOrder && !orderPages)
                    sum += printed[printed.Count / 2];
            }

            return sum;
        }
    }
}