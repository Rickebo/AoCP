using Common;
using Common.Updates;
using Lib.Extensions;
using Lib.Parsing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025Codelight.batmanwarrior;
public class Day17 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 17);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Palindromes, but stick to english!";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create collection
            Collection coll = new(input, reporter);

            // Count palindromes
            int count = coll.PalindromeCount();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(count.ToString()));
            return Task.CompletedTask;
        }

        private class Collection
        {
            private readonly Reporter _reporter;
            private readonly List<string> _candidates = [];

            public Collection(string input, Reporter reporter)
            {
                // Save for printing
                _reporter = reporter;

                // Split input to retrieve palindrome candidates
                string[] lines = input.SplitLines();

                // Remove unwanted characters and add to list
                StringBuilder sb = new();
                foreach (string line in lines)
                {
                    sb.Clear();
                    foreach (char c in line)
                        if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                            sb.Append(c);
                    string candidate = sb.ToString();
                    _candidates.Add(candidate);

                    // Print all candidates when added to collection
                    _reporter.ReportLine($"{line} -> {candidate}");
                }
            }

            public int PalindromeCount()
            {
                // Count palindrome candidates
                int count = 0;
                bool isPalindrome;
                foreach (string candidate in _candidates)
                {
                    isPalindrome = true;
                    for (int i = 0; i < candidate.Length / 2; i++)
                    {
                        // Compare front and back characters
                        if (candidate[i] != candidate[^(i + 1)])
                        {
                            isPalindrome = false;
                            break;
                        }
                    }

                    if (isPalindrome)
                        count++;
                }

                return count;
            }
        }
    }
}