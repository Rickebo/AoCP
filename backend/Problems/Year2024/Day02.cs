using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024;

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
            string[] rows = Parser.SplitBy(input, ["\r\n", "\r", "\n"]);

            int safeCount = 0;

            foreach (string row in rows)
            {
                int[] numbers = Parser.GetValues<int>(row);

                bool safe = CheckIncreasing(numbers) || CheckDecreasing(numbers);

                reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [$"{row} = {safe}"]
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
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            string[] rows = Parser.SplitBy(input, ["\r\n", "\r", "\n"]);

            int safeCount = 0;

            foreach (string row in rows)
            {
                int[] numbers = Parser.GetValues<int>(row);

                bool safe = false;
                for (int i = 0; i < numbers.Length; i++)
                {
                    List<int> copy = [.. numbers];
                    copy.RemoveAt(i);

                    safe = CheckIncreasing([.. copy]) || CheckDecreasing([.. copy]);

                    if (safe)
                    {
                        safeCount++;
                        break;
                    }
                }

                reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [$"{row} = {safe}"]
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
            return Task.CompletedTask;
        }
    }

    private static bool CheckIncreasing(int[] numbers)
    {
        for (int i = 1; i < numbers.Length; i++)
        {
            int diff = numbers[i] - numbers[i - 1];
            if (diff < 1 || diff > 3)
            {
                return false;
            }
        }

        return true;
    }

    private static bool CheckDecreasing(int[] numbers)
    {
        for (int i = 1; i < numbers.Length; i++)
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