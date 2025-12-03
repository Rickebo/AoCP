using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;

namespace Backend.Problems.Year2025Codelight.Rickebo;

public class Day24 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 24);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "The Workshop Impact Circuit";


    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";
        
        public override Task Solve(string input, Reporter reporter)
        {
            var numbers = input.Split(' ').Select(int.Parse).ToArray();
            var solution = Solve(numbers, 0, numbers.Length, new Dictionary<int, int>());
            
            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Solution = solution.ToString(),
                }
            );

            return Task.CompletedTask;
        }

        private int Solve(int[] offices, int offset, int length, Dictionary<int, int> cache)
        {
            if (offset >= length)
                return 0;

            var key = offset << 16 | length;
            if (cache.TryGetValue(key, out var result))
                return result;

            var n = offices[offset];
            var a = Solve(offices, offset + 2, offset == 0 ? length - 1 : length, cache) + n;
            var b = Solve(offices, offset + 1, length, cache);

            var max = Math.Max(a, b);
            cache[key] = max;

            return max;
        }
    }
}

