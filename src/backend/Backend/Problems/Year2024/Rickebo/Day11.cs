using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.Rickebo;

public class Day11 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 11);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Plutonian Pebbless";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            var data = Parser.GetValues<ulong>(input);

            reporter.ReportSolution(SimulateBlinks(25, data));

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            var data = Parser.GetValues<ulong>(input);

            reporter.ReportSolution(SimulateBlinks(75, data));
            return Task.CompletedTask;
        }
    }

    private static ulong SimulateBlinks(int blinks, IEnumerable<ulong> stones)
    {
        var cache = new Dictionary<CacheKey, ulong>();
        var sum = 0UL;
        foreach (var s in stones)
            sum += SimulateBlinks(blinks, s, cache);

        return sum;
    }

    private static ulong SimulateBlinks(int blinks, ulong stone, Dictionary<CacheKey, ulong> cache)
    {
        if (blinks <= 0)
            return 1;

        var key = new CacheKey(blinks, stone);
        if (cache.TryGetValue(key, out var cached))
            return cached;

        if (stone == 0)
            return cache[key] = SimulateBlinks(blinks - 1, 1, cache);

        var log = MathExtensions.CeilLog10(stone + 1);
        
        // If the ceil log 10 is even, the number has an odd number of digits
        if (log < 1 || log % 2 == 1)
            return cache[key] = SimulateBlinks(blinks - 1, stone * 2024, cache);    

        // ReSharper disable once PossibleLossOfFraction
        var half = MathExtensions.Pow10(log / 2);
        var right = stone % half;
        var left = (stone - right) / half;

        return cache[key] = SimulateBlinks(blinks - 1, left, cache) +
                            SimulateBlinks(blinks - 1, right, cache);
    }

    private readonly record struct CacheKey(int Blinks, ulong Stone);
}