using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib.Coordinate;
using Lib.Parsing;
using Lib.Grid;
using Lib.Extensions;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day14 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 14);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Restroom Redoubt";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create bathroom
            Bathroom bathroom = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(bathroom.SafetyFactor(seconds: 100)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create bathroom
            Bathroom bathroom = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(bathroom.EasterEggTime()));
            return Task.CompletedTask;
        }
    }

    public class Bathroom
    {
        private readonly Reporter _reporter;
        private readonly List<Robot> _robots = [];
        private readonly int _width;
        private readonly int _height;
        private readonly bool _evenRows;
        private readonly bool _evenCols;

        public Bathroom(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Parse robots
            foreach (string row in input.SplitLines())
            {
                // Robot values
                int[] values = Parser.GetValues<int>(row);
                _robots.Add(new(new(values[0], values[1]), values[2], values[3]));

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"Robot added at position [{values[0]},{values[1]}] with velocity [{values[2]},{values[3]}]"));

                // Keep track of bathroom size
                _width = Math.Max(_width, values[0] + 1);
                _height = Math.Max(_height, values[1] + 1);
            }

            // Save for later
            _evenRows = _width % 2 == 0;
            _evenCols = _height % 2 == 0;
        }

        public long SafetyFactor(int seconds)
        {
            // Let X seconds elapse
            for (int i = 0; i < seconds; i++)
            {
                // Move robots
                foreach (Robot robot in _robots)
                    Move(robot);
            }

            // Count robots in quadrants
            long RQ1 = 0, RQ2 = 0, RQ3 = 0, RQ4 = 0;
            foreach (Robot robot in _robots)
            {
                if (WithinQ1(robot)) RQ1++;
                else if (WithinQ2(robot)) RQ2++;
                else if (WithinQ3(robot)) RQ3++;
                else if (WithinQ4(robot)) RQ4++;
            }

            // Calculate safety factor
            long safetyFactor = RQ1 * RQ2 * RQ3 * RQ4;

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"\nQuadrant | Robots\n{1,-11}{RQ1}\n{2,-11}{RQ2}\n{3,-11}{RQ3}\n{4,-11}{RQ4}\n\nSafety factor: {RQ1} * {RQ2} * {RQ3} * {RQ4} = {safetyFactor}"));

            // Return safety factor
            return safetyFactor;
        }

        public long EasterEggTime()
        {
            long elapsed = 0;
            for(;;)
            {
                // Robot positions to check if stacked
                HashSet<IntegerCoordinate<int>> robotPositions = [];

                // Move robots
                foreach (Robot robot in _robots)
                    Move(robot);

                // One second has passed
                elapsed++;

                // Check if all robots are on separate tiles
                bool easterEgg = true;
                foreach (Robot robot in _robots)
                {
                    // Tile occupied
                    if (!robotPositions.Add(robot.Pos))
                    {
                        easterEgg = false;
                        break;
                    }
                }

                // All robots are standing on separate tiles
                if (easterEgg)
                {
                    // Create grid
                    CharGrid grid = new(' ', _width, _height);

                    // Add robots
                    foreach (Robot robot in _robots)
                        grid[robot.Pos] = 'O';

                    // Send to frontend
                    _reporter.Report(GlyphGridUpdate.FromCharGrid(grid, "#FFFFFF", "#000000"));
                    _reporter.Report(TextProblemUpdate.FromLine($"\nEaster egg found after {elapsed} seconds"));

                    return elapsed;
                }
            }
        }

        private bool WithinQ1(Robot robot) => (_evenCols ? robot.Pos.X < (_width - 1) / 2 : robot.Pos.X < _width / 2) && 
            (_evenRows ? robot.Pos.Y < (_height - 1) / 2 : robot.Pos.Y < _height / 2);

        private bool WithinQ2(Robot robot) => (_evenCols ? robot.Pos.X > _width / 2 : robot.Pos.X > (_width - 1) / 2) &&
            (_evenRows ? robot.Pos.Y < (_height - 1) / 2 : robot.Pos.Y < _height / 2);

        private bool WithinQ3(Robot robot) => (_evenCols ? robot.Pos.X < (_width - 1) / 2 : robot.Pos.X < _width / 2) &&
            (_evenRows ? robot.Pos.Y > _height / 2 : robot.Pos.Y > (_height - 1) / 2);

        private bool WithinQ4(Robot robot) => (_evenCols ? robot.Pos.X > _width / 2 : robot.Pos.X > (_width - 1) / 2) &&
            (_evenRows ? robot.Pos.Y > _height / 2 : robot.Pos.Y > (_height - 1) / 2);

        private void Move(Robot robot)
        {
            // Move robot
            robot.Pos = new(robot.Pos.X + robot.vX, robot.Pos.Y + robot.vY);

            // Wrap around x
            while (robot.Pos.X < 0)
                robot.Pos = new(_width + robot.Pos.X, robot.Pos.Y);

            while (robot.Pos.X >= _width)
                robot.Pos = new(robot.Pos.X - _width, robot.Pos.Y);

            // Wrap around y
            while (robot.Pos.Y < 0)
                robot.Pos = new(robot.Pos.X, _height + robot.Pos.Y);

            while (robot.Pos.Y >= _height)
                robot.Pos = new(robot.Pos.X, robot.Pos.Y - _height);
        }
    }

    public class Robot(IntegerCoordinate<int> pos, int vx, int vy)
    {
        public IntegerCoordinate<int> Pos = pos;
        public readonly int vX = vx;
        public readonly int vY = vy;
    }
}