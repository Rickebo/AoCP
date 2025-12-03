using Common;
using Common.Updates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025Codelight.batmanwarrior;
public class Day18 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 18);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Panic at the Parentheses";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create parentheses police
            ParenthesesPolice police = new();

            // Police score
            int score = police.GetScore(input);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(score.ToString()));
            return Task.CompletedTask;
        }

        private class ParenthesesPolice
        {
            private readonly char[] _openingBrackets = ['(', '[', '{'];
            private readonly char[] _closingBrackets = [')', ']', '}'];

            public int GetScore(string input)
            {
                int score = 0;
                Stack<char> stack = new();
                for (int i = 0; i < input.Length; i++)
                {
                    int openingIndex = Array.IndexOf(_openingBrackets, input[i]);
                    int closingIndex = Array.IndexOf(_closingBrackets, input[i]);

                    // Opening bracket
                    if (openingIndex >= 0)
                        stack.Push(input[i]);

                    // Closing bracket, check last opening bracket
                    else if (closingIndex >= 0)
                    {
                        if (Array.IndexOf(_openingBrackets, stack.Pop()) == closingIndex)
                            score++;
                        else
                            return score;
                    }
                }

                return score + 1;
            }
        }
    }
}

