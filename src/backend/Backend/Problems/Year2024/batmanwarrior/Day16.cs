using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day16 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 16);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Temp 16";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create temp
            TEMP temp = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(""));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create temp
            TEMP temp = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(""));
            return Task.CompletedTask;
        }
    }

    public class TEMP
    {
        private readonly Reporter _reporter;

        public TEMP(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;
        }
    }
}