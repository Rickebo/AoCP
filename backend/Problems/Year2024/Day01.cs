using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024;

public class Day01 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 01, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Historian Hysteria";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            (List<int> left, List<int> right) = GetValueLists(input);

            left.Sort();
            right.Sort();

            int diff = 0;

            for (int i = 0; i < left.Count; i++)
            {
                int currDiff = Math.Abs(left[i] - right[i]);

                reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [$"|{left[i]} - {right[i]}| = {currDiff}"]
                    }
                );

                diff += currDiff;
            }

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = diff.ToString()
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
            (List<int> left, List<int> right) = GetValueLists(input);

            int sim = 0;

            for (int i = 0; i < left.Count; i++)
            {
                int multiplier = right.Where(x => x.Equals(left[i])).Count();
                int currSim = left[i] * multiplier;

                reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [$"{left[i]} * {multiplier} = {currSim}"]
                    }
                );

                sim += currSim;
            }

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = sim.ToString()
                }
            );
            return Task.CompletedTask;
        }
    }

    private static (List<int>, List<int>) GetValueLists(string input)
    {
        List<int> left = [];
        List<int> right = [];
        foreach (string row in Parser.SplitBy(input, ["\r\n", "\r", "\n"]))
        {
            int[] numbers = Parser.GetValues<int>(row);
            left.Add(numbers[0]);
            right.Add(numbers[1]);
        }
        return (left, right);
    }
}