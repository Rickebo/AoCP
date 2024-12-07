using Common;
using Common.Updates;
using System.Text.RegularExpressions;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day03 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 03, 0, 0, 0);

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
            int sum = Multis(input, reporter);

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = sum.ToString()
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
            // Input starts active
            bool active = true;
            var sum = 0;

            // Cursed for loop
            for (;;)
            {
                List<string> parts;

                if (active)
                {
                    parts = DivideString(input, "don't()");
                    if (parts.Count > 1)
                    {
                        // Add up to don't()
                        sum += Multis(parts[0], reporter);
                        active = false;
                        input = parts[1];
                    }
                    else
                    {
                        // Add the rest (don't() not found)
                        sum += Multis(input, reporter);
                        break;
                    }
                }
                else
                {
                    parts = DivideString(input, "do()");
                    if (parts.Count > 1)
                    {
                        // Set active and update input string
                        active = true;
                        input = parts[1];
                    }
                    else
                    {
                        // The rest is not active
                        break;
                    }
                }
            }

            // Send to frontend
            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Solution = sum.ToString()
                }
            );
            return Task.CompletedTask;
        }
    }

    private static int Multis(string str, Reporter rep)
    {
        // Extract multipliers
        Regex Regex = new(@"mul\(([0-9]{1,3}),([0-9]{1,3})\)", RegexOptions.Multiline);
        var matches = Regex.Matches(str);

        // Check matches
        var sum = 0;
        foreach (Match match in matches)
        {
            // Add the mullimulls
            int num1 = int.Parse(match.Groups[1].Value);
            int num2 = int.Parse(match.Groups[2].Value);
            sum += num1 * num2;

            // Report that shit
            rep.Report(
                new TextProblemUpdate()
                {
                    Lines = [$"mul({num1},{num2})"]
                }
            );
        }

        return sum;
    }

    private static List<string> DivideString(string str, string divider)
    {
        List<string> res = [];

        // Check if divider is ahead
        int i = str.IndexOf(divider);

        if (i != -1)
        {
            // Split string at divider
            res.Add(str[..i]);
            res.Add(str[(i + divider.Length)..]);
            return res;
        }

        // No divider found
        res.Add(str);
        return res;
    }
}