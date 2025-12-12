using Common;
using Lib.Grids;
using Lib.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day12 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 12);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Christmas Tree Farm";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Send solution to frontend (Part 1)
            reporter.ReportSolution(new Solver(input, reporter, 1).PartOne());
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Send solution to frontend (Part 2)
            reporter.ReportSolution(new Solver(input, reporter, 2).PartTwo());
            return Task.CompletedTask;
        }
    }

    public class Solver
    {
        private readonly Reporter _reporter;
        private readonly List<(CharGrid Grid, int Occupied, int Free)> _shapes = [];
        private readonly List<(int Width, int Height, List<int> Quantities)> _regions = [];

        public Solver(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            string[] lines = input.SplitLines();

            // Shapes
            for (int i = 1; i < 24; i += 4)
            {
                CharGrid grid = new(string.Join('\n', lines[i..(i + 3)]));
                int occupied = grid.FindAll(c => c == '#').Count();
                int free = grid.Width * grid.Height - occupied;
                _shapes.Add((grid, occupied, free));
            }

            // Regions
            foreach (var line in lines[24..])
            {
                int[] vals = Parser.GetValues<int>(line);
                _regions.Add((Width: vals[0], Height: vals[1], [..vals[2..]]));
            }
        }

        public int PartOne()
        {
            int validRegions = 0;

            foreach (var (Width, Height, Quantities) in _regions)
            {
                // Check if occupied present space fits in region (lucky & ugly 'solution')
                int occupied = Quantities.Zip(_shapes, (q, shape) => q * shape.Occupied).Sum();
                if (occupied <= Width * Height)
                    validRegions++;
            }
            
            return validRegions;
        }

        public string PartTwo()
        {
            return "bruh";
        }
    }
}
