using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024;

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
            Sorter sorter = new();
            string[] rows = Parser.SplitBy(input, ["\r\n", "\r", "\n"]);
            int res = 0;

            foreach (string row in rows)
            {
                if (row.Contains('|'))
                {
                    sorter.AddRule(row, reporter);
                }
                else
                {
                    res += sorter.Update(row, reporter);
                }
            }

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = res.ToString()
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
            Sorter sorter = new();
            string[] rows = Parser.SplitBy(input, ["\r\n", "\r", "\n"]);
            int res = 0;

            foreach (string row in rows)
            {
                if (row.Contains('|'))
                {
                    sorter.AddRule(row, reporter);
                }
                else
                {
                    res += sorter.UpdateFixer(row, reporter);
                }
            }

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = res.ToString()
                }
            );
            return Task.CompletedTask;
        }
    }

    private class Sorter
    {
        public List<int> printed = [];
        public Dictionary<int, List<int>> printRules = [];

        public Sorter() { }

        public void AddRule(string row, Reporter reporter)
        {
            int[] pages = Parser.GetValues<int>(row);
            if (!printRules.ContainsKey(pages[1]))
            {
                printRules[pages[1]] = [];
            }
            if (!printRules[pages[1]].Contains(pages[0]))
            {
                printRules[pages[1]].Add(pages[0]);
            }

            reporter.Report(
                new TextProblemUpdate()
                {
                    Lines = [$"{row} added"]
                }
            );
        }

        public int Update(string row, Reporter reporter)
        {
            printed.Clear();
            int[] pages = Parser.GetValues<int>(row);
            foreach (int page in pages)
            {
                if (printRules.ContainsKey(page))
                {
                    foreach (var rule in printRules[page])
                    {
                        if (!printed.Contains(rule) && pages.Contains(rule))
                        {
                            return 0;
                        }
                    }
                }

                if (!printed.Contains(page))
                {
                    printed.Add(page);

                    reporter.Report(
                        new TextProblemUpdate()
                        {
                            Lines = [$"{page} printed"]
                        }
                    );
                }
            }

            reporter.Report(
                new TextProblemUpdate()
                {
                    Lines = [$"{row} = correct"]
                }
            );

            return pages[pages.Length / 2];
        }

        public int UpdateFixer(string row, Reporter reporter)
        {
            printed.Clear();
            int[] pages = Parser.GetValues<int>(row);
            Queue<int> queue = new();
            foreach (int page in pages)
            {
                queue.Enqueue(page);
            }

            bool correct = true;
            bool skip = false;

            while (queue.TryDequeue(out int page))
            {
                skip = false;
                if (printRules.ContainsKey(page))
                {
                    foreach (var rule in printRules[page])
                    {
                        if (!printed.Contains(rule) && pages.Contains(rule))
                        {
                            queue.Enqueue(page);
                            correct = false;
                            skip = true;
                            break;
                        }
                    }
                }

                if (skip) continue;

                if (!printed.Contains(page))
                {
                    printed.Add(page);

                    reporter.Report(
                        new TextProblemUpdate()
                        {
                            Lines = [$"{page} printed"]
                        }
                    );
                }
            }

            string rep = $"{row} = {(correct ? "correct" : "incorrect")}";
            string fix = $"Fixed: " + string.Join(",", printed);

            reporter.Report(
                new TextProblemUpdate()
                {
                    Lines = [$"{rep} {(!correct ? fix : "")}"], 
                }
            );

            return !correct ? printed[printed.Count / 2] : 0;
        }
    }
}