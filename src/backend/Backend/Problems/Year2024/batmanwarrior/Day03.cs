using Common;
using Common.Updates;
using System.Text;
using System.Text.RegularExpressions;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day03 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 03);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Mull It Over";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create multiplicator
            Multiplicator multiplicator = new(input, conditional: false, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(multiplicator.Mul(input, reporter)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Multiplication enabled from the start
            bool enabled = true;

            // Cursed for loop
            int sum = 0;
            for (;;)
            {
                if (enabled)
                {
                    // Divide remaining string at the next "don't()"
                    List<string> parts = DivideString(input, "don't()");

                    // Check if string got divided properly
                    if (parts.Count > 1)
                    {
                        // Add multiplications
                        sum += Multiplications(parts[0], reporter);
                        enabled = false;
                        input = parts[1];
                    }
                    else
                    {
                        // Add the rest
                        sum += Multiplications(input, reporter);
                        break;
                    }
                }
                else
                {
                    // Divide remaining string at the next "do()"
                    List<string> parts = DivideString(input, "do()");

                    // Check if string got divided properly
                    if (parts.Count > 1)
                    {
                        // Enable multiplications and update remaining string
                        enabled = true;
                        input = parts[1];
                    }
                    else
                    {
                        // The rest is not enabled
                        break;
                    }
                }
            }

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(sum));
            return Task.CompletedTask;
        }
    }

    

    public class Multiplicator
    {
        private readonly Reporter _reporter;
        private readonly string _programMemory;
        private const string doStr = "do()";
        private const string dontStr = "don't()";
        
        public Multiplicator(string input, bool conditional, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Retrieve program memory
            bool enabled = true;
            StringBuilder sb = new();
            for (;;)
            {
                int i = input.IndexOf(enabled ? dontStr : doStr);
                if (i == -1)
                {
                    // If the rest is enabled
                    if (enabled)
                        sb.Append(input);

                    // Program memory read
                    break;
                }
                else
                {
                    // If enabled
                    if (enabled)
                        sb.Append(input[..i]);

                    // Look for more memory
                    input = input[(i + dontStr.Length)..];
                }

                // Toggle instruction
                enabled = !enabled;
            }

            // Store memory for processing
            _programMemory = sb.ToString();
        }

        public static int Multiply()
        {
            // Extract multipliers
            Regex Regex = new(@"mul\(([0-9]{1,3}),([0-9]{1,3})\)", RegexOptions.Multiline);

            // Check matches
            int sum = 0;
            foreach (Match match in Regex.Matches(str))
            {
                // Add the mullimulls
                int num1 = int.Parse(match.Groups[1].Value);
                int num2 = int.Parse(match.Groups[2].Value);
                sum += num1 * num2;

                // Send to frontend
                reporter.Report(TextProblemUpdate.FromLine($"mul({num1},{num2})"));
            }

            return sum;
        }
    }
}