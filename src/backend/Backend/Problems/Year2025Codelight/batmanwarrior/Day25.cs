using Common;
using Common.Updates;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025Codelight.batmanwarrior;
public class Day25 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 25);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Caffeine Complexity";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create office cafe
            OfficeCafe cafe = new(input);

            // Get minimum order complexity
            int minimumComplexity = cafe.MinimumOrderComplexity();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(minimumComplexity.ToString()));
            return Task.CompletedTask;
        }

        private class OfficeCafe
        {
            private static readonly Dictionary<string, int> _complexity = new()
            {
                {"espresso", 1},
                {"americano", 2},
                {"cappuccino", 3},
                {"latte", 3},
                {"mocha", 4}
            };
            private readonly List<(string drink, string size, int time)> _orders = [];

            public OfficeCafe(string input)
            {
                // Parse input
                foreach (string line in input.SplitLines())
                {
                    // Add order
                    string[] args = line.Split(',');
                    string drink = args[0];
                    string size = args[1];
                    int time = int.Parse(args[2]);
                    _orders.Add((drink, size, time));
                }
            }

            public int MinimumOrderComplexity()
            {
                // Group by drink type + size
                Dictionary<(string drink, string size), List<int>> groups = _orders
                    .GroupBy(order => (order.drink, order.size))
                    .ToDictionary(group => group.Key, group => group.Select(order => order.time).OrderBy(time => time).ToList());

                // Sum group complexities
                int totalComplexity = 0;
                foreach (KeyValuePair<(string drink, string size), List<int>> pair in groups)
                {
                    List<int> timestamps = pair.Value;
                    int C = _complexity[pair.Key.drink];

                    totalComplexity += MinComplexityForGroup(timestamps, C);
                }

                return totalComplexity;
            }

            private static int MinComplexityForGroup(List<int> timestamps, int drinkComplexity)
            {
                int len = timestamps.Count;
                int[] complexity = new int[len];

                // Start looking at timestamps
                for (int i = 0; i < len; i++)
                {
                    // Store complexity for this timestamp
                    complexity[i] = int.MaxValue;

                    // Try to group as many previous drinks as possible in this group
                    for (int j = i; j >= 0; j--)
                    {
                        // Time window too large
                        if (timestamps[i] - timestamps[j] > 5)
                            break;

                        // Determine how many drinks this group of orders contain
                        int groupSize = i - j + 1;

                        // Calculate complexity of group
                        int groupComplexity = drinkComplexity + (groupSize - 1) * (drinkComplexity - 1);

                        // Compare the complexity of this group to previous group
                        int previousComplexity = j > 0 ? complexity[j - 1] : 0;
                        complexity[i] = Math.Min(complexity[i], previousComplexity + groupComplexity);
                    }
                }

                return complexity[^1];
            }
        }
    }
}

