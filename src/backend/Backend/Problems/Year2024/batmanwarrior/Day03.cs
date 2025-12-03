using System;
using System.Collections.Generic;
using Common;
using Common.Updates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            reporter.Report(FinishedProblemUpdate.FromSolution(multiplicator.Multiply()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create multiplicator
            Multiplicator multiplicator = new(input, conditional: true, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(multiplicator.Multiply()));
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

            // If not conditional, store full input
            if (!conditional)
            {
                _programMemory = input;
                return;
            }

            // Retrieve program memory
            bool enabled = true;
            StringBuilder sb = new();
            for (;;)
            {
                // Look ahead for condition
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
                    // If enabled, add substring up to condition
                    if (enabled)
                        sb.Append(input[..i]);

                    // Look for more memory
                    input = input[(i + (enabled ? dontStr.Length : doStr.Length))..];
                }

                // Toggle instruction
                enabled = !enabled;
            }

            // Store memory for processing
            _programMemory = sb.ToString();
        }

        public long Multiply()
        {
            // Regex to match multipliers
            Regex Regex = new(@"mul\(([0-9]{1,3}),([0-9]{1,3})\)", RegexOptions.Multiline);

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"Multiplication | Accumulated Sum"));

            // Check matches
            long sum = 0;
            foreach (Match match in Regex.Matches(_programMemory))
            {
                // Add the mullimulls
                long num1 = long.Parse(match.Groups[1].Value);
                long num2 = long.Parse(match.Groups[2].Value);
                sum += num1 * num2;

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"mul({num1},{num2})".PadRight(17) + sum));
            }

            return sum;
        }
    }
}

