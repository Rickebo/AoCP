using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;

namespace Backend.Problems.Year2025Codelight.Rickebo;

public class Day17 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 17);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Palindromes, but stick to english!";

    private static bool IsAllowed(char ch) => ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    
    private static List<string> Parse(string input) => input
        .SplitLines()
        .Select(line => new string(line
            .Where(IsAllowed)
            .ToArray()
        ))
        .ToList();
    
    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";


        public override Task Solve(string input, Reporter reporter)
        {
            var words = Parse(input);

            var palindromes = words.Count(word => word.Equals(new string(word.Reverse().ToArray()), StringComparison.OrdinalIgnoreCase));
            
            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Solution = palindromes.ToString(),
                }
            );
            
            return Task.CompletedTask;
        }
    }
}

