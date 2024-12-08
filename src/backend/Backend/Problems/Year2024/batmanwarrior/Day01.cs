using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day01 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 01);

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
            // Get number lists from input string
            (List<int> left, List<int> right) = GetValueLists(input);

            // Sort lists
            left.Sort();
            right.Sort();

            // Retrieve total distance between the lists
            int distance = 0;
            for (int i = 0; i < left.Count; i++)
            {
                // Absolute value
                int currDistance = Math.Abs(left[i] - right[i]);

                // Send to frontend
                reporter.Report(TextProblemUpdate.FromLine($"|{left[i]} - {right[i]}| = {currDistance}"));

                // Accumulate distance
                distance += currDistance;
            }

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(distance));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Get number lists from input string
            (List<int> left, List<int> right) = GetValueLists(input);

            // Retrieve total similarity score between the lists
            int similarity = 0;
            for (int i = 0; i < left.Count; i++)
            {
                // Count occurences of each number in the right list
                int occurences = right.Where(x => x.Equals(left[i])).Count();

                // Calculate similarity score
                int currSimilarity = left[i] * occurences;

                // Send to frontend
                reporter.Report(TextProblemUpdate.FromLine($"{left[i]} * {occurences} = {currSimilarity}"));

                // Accumulate similarity score
                similarity += currSimilarity;
            }

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(similarity));
            return Task.CompletedTask;
        }
    }

    private static (List<int>, List<int>) GetValueLists(string input)
    {
        // Retrieve both number columns
        List<int> left = [], right = [];
        foreach (string row in input.SplitLines())
        {
            // Parse numbers and populate lists
            int[] numbers = Parser.GetValues<int>(row);
            left.Add(numbers[0]);
            right.Add(numbers[1]);
        }
        return (left, right);
    }
}