using System.Text;
using System.Text.RegularExpressions;
using Common.Updates;

namespace Backend.Problems.Year2024.Rickebo;

public class Day3 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 03, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Mull It Over";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = Operation.Parse(input).Select(op => op.Product()).Sum().ToString()
                }
            );

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            var sum = 0;
            var on = true;
            foreach (var op in Operation.Parse(input))
            {
                if (op.Modifier != null)
                    on = op.Modifier.Value;

                if (on) sum += op.Product();
            }
            
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = sum.ToString()
                }
            );

            return Task.CompletedTask;
        }
    }

    private class Operation(bool? modifier, int? left = null, int? right = null)
    {
        public bool? Modifier { get; } = modifier;

        public static IEnumerable<Operation> Parse(string input)
        {
            var pattern = new Regex("(mul\\((\\d{1,3}),(\\d{1,3})\\))|don't\\(\\)|do\\(\\)");
            var minIndex = 0;

            foreach (Match match in pattern.Matches(input))
            {
                var index = match.Index;

                if (index < minIndex) continue;

                minIndex = index + match.Length;

                var fullMatch = match.Groups[0].Value;

                var op = fullMatch switch
                {
                    "don't()" => (bool?)false,
                    "do()" => (bool?)true,
                    _ => (bool?)null
                };

                var leftValue = op == null ? (int?)int.Parse(match.Groups[2].Value) : null;
                var rightValue = op == null ? (int?)int.Parse(match.Groups[3].Value) : null;

                yield return new Operation(modifier: op, left: leftValue, right: rightValue);
            }
        }

        public int Product() => left != null && right != null ? left.Value * right.Value : 0;
    }
}