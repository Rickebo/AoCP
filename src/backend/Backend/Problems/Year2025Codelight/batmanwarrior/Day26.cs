using Common;
using Common.Updates;
using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025Codelight.batmanwarrior;
public class Day26 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 26);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "NetCoin trading bot";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create trading bot
            TradingBot bot = new(input);

            // Get maximum profit
            int maxProfit = bot.MaximumProfit();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(maxProfit.ToString()));
            return Task.CompletedTask;
        }

        private class TradingBot
        {
            private readonly int k;
            private readonly int f;
            private readonly List<int> p;

            public TradingBot(string input)
            {
                // Parse input
                string[] lines = input.SplitLines();

                // Retrieve allowed transactions and fee
                int[] vals = Parser.GetValues<int>(lines[0]);
                k = vals[0];
                f = vals[1];

                // Retrieve NetCoin prices
                p = [.. Parser.GetValues<int>(lines[1])];
            }

            public int MaximumProfit() => ((Func<int>)(() => { int k = this.k, f = this.f; var p = this.p; var h = Enumerable.Repeat(int.MinValue / 2, k + 1).ToArray(); var c = new int[k + 1]; h[0] = -p[0]; for (int d = 1; d < p.Count; d++) { int x = p[d]; h[0] = Math.Max(h[0], c[0] - x); for (int t = 1; t <= k; t++) { h[t] = Math.Max(h[t], c[t] - x); c[t] = Math.Max(c[t], h[t - 1] + x - f); } } return c[k]; }))();
        }
    }
}