using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2025Codelight.Rickebo;

public class Day18 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 18);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Panic at the Parentheses";

    private static bool IsAllowed(char ch) => ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    
    
    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";


        public override Task Solve(string input, Reporter reporter)
        {
            var stack = new Stack<char>();

            var opening = "([{";
            var opposites = new Dictionary<char, char>()
            {
                { '{', '}' },
                { '[', ']' },
                { '(', ')' },
                { '}', '{' },
                { ']', '[' },
                { ')', '(' }
            };

            var pairs = 0;
            var valid = true;
            
            foreach (var ch in input)
            {
                if (!opposites.TryGetValue(ch, out var opposite))
                    continue;

                if (opening.Contains(ch))
                    stack.Push(ch);
                else {
                    if (stack.Pop() != opposite)
                    {
                        valid = false;
                        break;
                    }

                    pairs++;
                }
            }

            var score = valid ? pairs + 1 : pairs;

            reporter.ReportLine(!valid 
                ? $"{score} ({pairs} valid pairs before mismatched)"
                : $"{score} ({pairs} valid pairs + 1 for error-free)");
            
            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Solution = score.ToString(),
                }
            );
            
            return Task.CompletedTask;
        }
    }
}