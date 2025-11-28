using Common;
using Common.Updates;
using Lib;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Lib.Extensions;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day13 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 13);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Claw Contraption";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Spawn clawmachine
            ClawMachine clawMachine = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(clawMachine.FewestTokens(correction: false)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Spawn clawmachine
            ClawMachine clawMachine = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(clawMachine.FewestTokens(correction: true)));
            return Task.CompletedTask;
        }
    }

    public class ClawMachine
    {
        private readonly Reporter _reporter;
        private readonly List<PrizeConfiguration> _prizeConfigs = [];
        private const long _aCost = 3;
        private const long _bCost = 1;
        private const int _maxPresses = 100;
        private const long _correctionValue = 10000000000000;

        public ClawMachine(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Retrieve prize configurations
            string[] rows = input.SplitLines();
            for (int i = 0; i < rows.Length - 2; i += 3)
            {
                // Configuration data is 3 lines
                long[] aVals = Parser.GetValues<long>(rows[i]);
                long[] bVals = Parser.GetValues<long>(rows[i + 1]);
                long[] pVals = Parser.GetValues<long>(rows[i + 2]);

                // Add to list
                _prizeConfigs.Add(new((aVals[0], aVals[1]), (bVals[0], bVals[1]), (pVals[0], pVals[1])));
            }
        }

        private static long TokenCost(PrizeConfiguration pC, bool correction)
        {
            // Check if prize location needs to be corrected
            if (correction)
            {
                // Add correction
                pC.PX += _correctionValue;
                pC.PY += _correctionValue;

                // Calculate factor 1 & 2
                long factor1 = (pC.PX * pC.BY - pC.PY * pC.BX);
                long factor2 = (pC.AX * pC.BY - pC.AY * pC.BX);

                // Calculate A button presses
                long aP = factor1 / factor2;

                // Calculate factor 3
                long factor3 = (pC.PY - aP * pC.AY);

                // Calculate B button presses
                long bP = factor3 / pC.BY;

                // Faulty combination
                if (aP < 0 || bP < 0)
                    return 0;

                // Check if adding up
                if (aP * pC.AX + bP * pC.BX == pC.PX && aP * pC.AY + bP * pC.BY == pC.PY)
                    return aP * _aCost + bP * _bCost;

                return 0;
            }
            else
            {
                // The way of the brute
                long cost = Int64.MaxValue;
                for (int i = 0; i <= _maxPresses; i++) // Bruh
                {
                    for (int j = 0; j <= _maxPresses; j++) // Hurb
                    {
                        // **Puts square into round hole**
                        if ((pC.AX * i + pC.BX * j) == pC.PX && (pC.AY * i + pC.BY * j) == pC.PY)
                        {
                            // Great success
                            long currCost = i * _aCost + j * _bCost;

                            // Update cost when needed
                            if (currCost < cost)
                                cost = currCost;
                        }
                    }
                }

                return cost == Int64.MaxValue ? 0 : cost;
            }
        }

        public long FewestTokens(bool correction) => _prizeConfigs.Select(x => TokenCost(x, correction)).Sum();
    }

    public class PrizeConfiguration((long, long) buttonA, (long, long) buttonB, (long, long) prizeLocation)
    {
        public long AX = buttonA.Item1;
        public long AY = buttonA.Item2;
        public long BX = buttonB.Item1;
        public long BY = buttonB.Item2;
        public long PX = prizeLocation.Item1;
        public long PY = prizeLocation.Item2;
    }
}