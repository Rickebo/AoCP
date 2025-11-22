using Common;
using Common.Updates;
using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025Codelight.batmanwarrior;
public class Day20 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 20);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Predicting App User Growth";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create startup project
            StartupProject project = new(input, reporter);

            // Get users after X amount of days
            long users = project.UsersAfterDays(365);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(users.ToString()));
            return Task.CompletedTask;
        }

        private class StartupProject
        {
            private readonly long[] userBase = new long[10];
            private readonly Reporter _reporter;

            public StartupProject(string input, Reporter reporter)
            {
                // Save reporter for printing
                _reporter = reporter;

                // Get initial users and timer states
                int[] timerStates = Parser.GetValues<int>(input);

                // Add initual users
                foreach (int state in timerStates)
                    userBase[state]++;
            }

            public void Step()
            {
                // Step one day (decrement timer states and invite more users)
                long inviters = userBase[0];
                for (int i = 0; i < userBase.Length - 1; i++)
                    userBase[i] = userBase[i + 1];
                userBase[6] += inviters;
                userBase[9] = inviters;
            }

            public long UsersAfterDays(int days)
            {
                for (int i = 0; i < days; i++)
                {
                    Step();

                    // Print
                    _reporter.ReportLine($"Day{i + 1}: [{string.Join(", ", userBase.Select(x => x.ToString()))}] ({userBase.Sum()} users)");
                }

                return userBase.Sum();
            }
        }
    }
}