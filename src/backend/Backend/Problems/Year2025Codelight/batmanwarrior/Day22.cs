using Common;
using Common.Updates;
using Lib.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025Codelight.batmanwarrior;
public class Day22 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 22);

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
            // Create workshop circuit
            Circuit circuit = new(input);

            // Get highest circular impact
            int highestImpact = circuit.HighestImpact();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(highestImpact.ToString()));
            return Task.CompletedTask;
        }

        private class Circuit(string input)
        {
            private readonly int[] _workshops = Parser.GetValues<int>(input);

            public int HighestImpact()
            {
                // Attend first workshop (removing last workshop from circuit)
                int impactFirst = WorkshopImpact(_workshops[..^1], 0, []);

                // Attend second workshop (skipping first, keeping circuit as is)
                int impactSecond = WorkshopImpact(_workshops, 1, []);

                return Math.Max(impactFirst, impactSecond);
            }

            private static int WorkshopImpact(int[] workshops, int pos, Dictionary<int, int> cache)
            {
                // End of circuit reached
                if (pos >= workshops.Length)
                    return 0;

                // Get remaining workshops
                int left = workshops.Length - pos;

                // Check if cache contains solution for this pos
                int key = (pos << 16) ^ left;
                if (cache.TryGetValue(key, out int value))
                    return value;

                // Attend this workshop
                int attending = workshops[pos] + WorkshopImpact(workshops, pos + 2, cache);

                // Skip this workshop
                int skipping = WorkshopImpact(workshops, pos + 1, cache);

                // Determine max impact
                int maxImpact = Math.Max(attending, skipping);

                // Store the maximum impact in cache
                cache[key] = maxImpact;

                return maxImpact;
            }
        }
    }
}

