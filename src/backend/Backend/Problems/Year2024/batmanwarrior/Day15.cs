using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib.Grid;
using System.Text;
using Lib;
using Lib.Coordinate;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day15 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 15);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Warehouse Woes";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create warehouse
            Warehouse warehouse = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(warehouse.MoveAround()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create warehouse
            Warehouse warehouse = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(""));
            return Task.CompletedTask;
        }
    }

    public class Warehouse
    {
        private record Robot(IntegerCoordinate<int> Pos, Direction dir);
        private readonly Reporter _reporter;
        private readonly CharGrid _map;
        private readonly string _moves;

        public Warehouse(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Split map and moves
            StringBuilder sb1 = new();
            StringBuilder sb2 = new();

            foreach (string row in input.SplitLines())
            {
                if (row.Contains('#'))
                    sb1.AppendLine(row);
                else
                    sb2.Append(row);
            }

            // Create map
            _map = new(sb1.ToString());

            // Save moves
            _moves = sb2.ToString();

            // Print grid
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_map, "#FFFFFF", "#000000"));
        }

        public int MoveAround()
        {
            // Move around with robot
            int i;
            for (i = 0; i < _moves.Length; i++)
            {
                // Find robot
                IntegerCoordinate<int> robot = _map.Find(ch => ch == '@');

                // Get direction
                Direction dir = DirectionExtensions.Parse(_moves[i]);

                // If nothing is ahead of robot
                if (_map[robot.Move(dir)] == '.')
                {
                    _map[robot.Move(dir)] = '@';
                    _map[robot] = '.';
                    continue;
                }

                // If wall is ahead of robot
                if (_map[robot.Move(dir)] == '#')
                    continue;

                // Move until wall or empty space
                IntegerCoordinate<int> checkPos = robot.Move(dir);
                while (_map[checkPos] == 'O')
                    checkPos = checkPos.Move(dir);

                // Check if empty space or wall
                if (_map[checkPos] == '.')
                {
                    // Move back to robot and push objects
                    checkPos = checkPos.Move(dir.Opposite());
                    while (_map[checkPos] != '@')
                    {
                        _map[checkPos.Move(dir)] = 'O';
                        _map[checkPos] = '.';

                        checkPos = checkPos.Move(dir.Opposite());
                    }

                    _map[robot.Move(dir)] = '@';
                    _map[robot] = '.';
                }
                else
                {
                    // Cant be pushed further
                    continue;
                }
            }

            // Print grid
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_map, "#FFFFFF", "#000000"));

            int sum = 0;
            foreach (IntegerCoordinate<int> pos in _map.FindAll(x => x == 'O'))
                sum += 100 * (_map.Height - 1 - pos.Y) + pos.X;

            return sum;
        }
    }
}