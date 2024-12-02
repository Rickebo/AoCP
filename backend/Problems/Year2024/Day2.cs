using Common.Updates;

namespace Backend.Problems.Year2024;

public class Day2 : ProblemSet
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

        public override string Description { get; } = "<article class=\"day-desc\"><p>Fortunately, the first location The Historians want to search isn't a long walk from the Chief Historian's office.</p>\r\n<p>While the <a href=\"/2015/day/19\">Red-Nosed Reindeer nuclear fusion/fission plant</a> appears to contain no sign of the Chief Historian, the engineers there run up to you as soon as they see you. Apparently, they <em>still</em> talk about the time Rudolph was saved through molecular synthesis from a single electron.</p>\r\n<p>They're quick to add that - since you're already here - they'd really appreciate your help analyzing some unusual data from the Red-Nosed reactor. You turn to check if The Historians are waiting for you, but they seem to have already divided into groups that are currently searching every corner of the facility. You offer to help with the unusual data.</p>\r\n<p>The unusual data (your puzzle input) consists of many <em>reports</em>, one report per line. Each report is a list of numbers called <em>levels</em> that are separated by spaces. For example:</p>\r\n<pre><code>7 6 4 2 1\r\n1 2 7 8 9\r\n9 7 6 2 1\r\n1 3 2 4 5\r\n8 6 4 4 1\r\n1 3 6 7 9\r\n</code></pre>\r\n<p>This example data contains six reports each containing five levels.</p>\r\n<p>The engineers are trying to figure out which reports are <em>safe</em>. The Red-Nosed reactor safety systems can only tolerate levels that are either gradually increasing or gradually decreasing. So, a report only counts as safe if both of the following are true:</p>\r\n<ul>\r\n<li>The levels are either <em>all increasing</em> or <em>all decreasing</em>.</li>\r\n<li>Any two adjacent levels differ by <em>at least one</em> and <em>at most three</em>.</li>\r\n</ul>\r\n<p>In the example above, the reports can be found safe or unsafe by checking those rules:</p>\r\n<ul>\r\n<li><code>7 6 4 2 1</code>: <em>Safe</em> because the levels are all decreasing by 1 or 2.</li>\r\n<li><code>1 2 7 8 9</code>: <em>Unsafe</em> because <code>2 7</code> is an increase of 5.</li>\r\n<li><code>9 7 6 2 1</code>: <em>Unsafe</em> because <code>6 2</code> is a decrease of 4.</li>\r\n<li><code>1 3 2 4 5</code>: <em>Unsafe</em> because <code>1 3</code> is increasing but <code>3 2</code> is decreasing.</li>\r\n<li><code>8 6 4 4 1</code>: <em>Unsafe</em> because <code>4 4</code> is neither an increase or a decrease.</li>\r\n<li><code>1 3 6 7 9</code>: <em>Safe</em> because the levels are all increasing by 1, 2, or 3.</li>\r\n</ul>\r\n<p>So, in this example, <code><em>2</em></code> reports are <em>safe</em>.</p>\r\n<p>Analyze the unusual data from the engineers. <em>How many reports are safe?</em></p>\r\n</article>";

        public override async Task Solve(string input, Reporter reporter)
        {
            string[] lines = input.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);

            int safeCount = 0;

            foreach (string line in lines)
            {
                List<int> numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                bool safe = CheckIncreasing(numbers) || CheckDecreasing(numbers);

                reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [$"{line} = {safe}"]
                    }
                );

                if (safe)
                {
                    safeCount++;
                }
            }

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = safeCount.ToString()
                }
            );
        }

        private static bool CheckIncreasing(List<int> numbers)
        {
            for (int i = 1; i < numbers.Count; i++)
            {
                int diff = numbers[i] - numbers[i - 1];
                if (diff < 1 || diff > 3)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckDecreasing(List<int> numbers)
        {
            for (int i = 1; i < numbers.Count; i++)
            {
                int diff = numbers[i] - numbers[i - 1];
                if (diff < -3 || diff > -1)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "<article class=\"day-desc\"><p>The engineers are surprised by the low number of safe reports until they realize they forgot to tell you about the <span title=\"I need to get one of these!\">Problem Dampener</span>.</p>\r\n<p>The Problem Dampener is a reactor-mounted module that lets the reactor safety systems <em>tolerate a single bad level</em> in what would otherwise be a safe report. It's like the bad level never happened!</p>\r\n<p>Now, the same rules apply as before, except if removing a single level from an unsafe report would make it safe, the report instead counts as safe.</p>\r\n<p>More of the above example's reports are now safe:</p>\r\n<ul>\r\n<li><code>7 6 4 2 1</code>: <em>Safe</em> without removing any level.</li>\r\n<li><code>1 2 7 8 9</code>: <em>Unsafe</em> regardless of which level is removed.</li>\r\n<li><code>9 7 6 2 1</code>: <em>Unsafe</em> regardless of which level is removed.</li>\r\n<li><code>1 <em>3</em> 2 4 5</code>: <em>Safe</em> by removing the second level, <code>3</code>.</li>\r\n<li><code>8 6 <em>4</em> 4 1</code>: <em>Safe</em> by removing the third level, <code>4</code>.</li>\r\n<li><code>1 3 6 7 9</code>: <em>Safe</em> without removing any level.</li>\r\n</ul>\r\n<p>Thanks to the Problem Dampener, <code><em>4</em></code> reports are actually <em>safe</em>!</p>\r\n<p>Update your analysis by handling situations where the Problem Dampener can remove a single level from unsafe reports. <em>How many reports are now safe?</em></p>\r\n</article>";

        public override async Task Solve(
            string input,
            Reporter reporter
        )
        {
            string[] lines = input.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);

            int safeCount = 0;

            foreach (string line in lines)
            {
                List<int> numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                bool safe = false;
                for (int i = 0; i < numbers.Count; i++)
                {
                    List<int> copy = [.. numbers];
                    copy.RemoveAt(i);
                    safe = CheckIncreasing(copy) || CheckDecreasing(copy);

                    if (safe)
                    {
                        safeCount++;
                        break;
                    }
                }

                reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [$"{line} = {safe}"]
                    }
                );
            }

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = safeCount.ToString()
                }
            );
        }

        private static bool CheckIncreasing(List<int> numbers)
        {
            for (int i = 1; i < numbers.Count; i++)
            {
                int diff = numbers[i] - numbers[i - 1];
                if (diff < 1 || diff > 3)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckDecreasing(List<int> numbers)
        {
            for (int i = 1; i < numbers.Count; i++)
            {
                int diff = numbers[i] - numbers[i - 1];
                if (diff < -3 || diff > -1)
                {
                    return false;
                }
            }

            return true;
        }
    }
}