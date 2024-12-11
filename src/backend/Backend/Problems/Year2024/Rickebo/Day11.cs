using System.Runtime.InteropServices.Marshalling;
using System.Text;
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

    public override string Name => "Plutonian Pebbles";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            var data = Parser.GetValues<long>(input);

            reporter.ReportSolution(SimulateBlinks(25, data));

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            var data = Parser.GetValues<long>(input);

            reporter.ReportSolution(SimulateBlinks(75, data));
            return Task.CompletedTask;
        }
    }

    private static long SimulateBlinks(int blinks, IEnumerable<long> stones)
    {
        var cache = new Dictionary<CacheKey, long>();
        return stones.Sum(s => SimulateBlinks(blinks, s, cache));
    }
    
    private static long SimulateBlinks(int blinks, long stone, Dictionary<CacheKey, long> cache)
    {
        if (blinks <= 0)
            return 1;

        var key = new CacheKey(blinks, stone);
        if (cache.TryGetValue(key, out var cached))
            return cached;

        if (stone == 0)
            return cache[key] = SimulateBlinks(blinks - 1, 1, cache);

        var stoneStr = stone.ToString();
        if (stoneStr.Length % 2 == 0)
        {
            var left = long.Parse(stoneStr[..(stoneStr.Length / 2)]);
            var right = long.Parse(stoneStr[(stoneStr.Length / 2)..]);

            return cache[key] = SimulateBlinks(blinks - 1, left, cache) +
                                 SimulateBlinks(blinks - 1, right, cache);
        }

        return cache[key] = SimulateBlinks(blinks - 1, stone * 2024, cache);
    }

    private record CacheKey(int Blinks, long Stone);
}