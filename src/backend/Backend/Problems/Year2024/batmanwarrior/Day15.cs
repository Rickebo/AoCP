using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib.Grids;
using System.Text;
using Lib.Geometry;
using System.Linq;
using Lib.Geometry;

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
            Warehouse warehouse = new(input, wide: false, reporter);

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
            Warehouse warehouse = new(input, wide: true, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(warehouse.MoveAround()));
            return Task.CompletedTask;
        }
    }

    public class Warehouse
    {
        private readonly Reporter _reporter;
        private readonly CharGrid _map;
        private readonly string _moves;

        public Warehouse(string input, bool wide, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Split map and moves
            StringBuilder sb1 = new();
            StringBuilder sb2 = new();
            foreach (string row in input.SplitLines())
            {
                // Map rows always start with '#'
                if (row[0] == '#')
                    sb1.AppendLine(!wide ? row : row.Replace("#", "##").Replace("O", "[]").Replace(".", "..").Replace("@", "@."));
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
            // Get robot initial position
            IntegerCoordinate<int> robotPos = _map.Find(ch => ch == '@');

            // Move around with robot
            for (int i = 0; i < _moves.Length; i++)
            {
                // Check if push possible
                HashSet<IntegerCoordinate<int>> pushed = [];
                Dictionary<IntegerCoordinate<int>, char> result = [];
                Direction dir = DirectionExtensions.Parse(_moves[i]);
                if (Possible(robotPos, dir, pushed, result))
                {
                    // Perform push result
                    foreach (KeyValuePair<IntegerCoordinate<int>, char> pair in result)
                    {
                        // Result of push
                        _map[pair.Key] = pair.Value;

                        // Result tile does not need to be cleared
                        pushed.Remove(pair.Key);
                    }

                    // Clear the remaining pushed tiles
                    foreach (IntegerCoordinate<int> pos in pushed)
                        _map[pos] = '.';

                    // Move robot pos
                    robotPos = robotPos.Move(dir);
                }
            }

            // Print grid
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_map, "#FFFFFF", "#000000"));

            // Return result to frontend
            return _map.FindAll(ch => ch == 'O' || ch == '[').Aggregate(0, (sum, pos) => sum += 100 * (_map.Height - 1 - pos.Y) + pos.X);
        }

        private bool Possible(IntegerCoordinate<int> pos, Direction dir, HashSet<IntegerCoordinate<int>> pushed, Dictionary<IntegerCoordinate<int>, char> result)
        {
            // If wall ahead
            if (_map[pos.Move(dir)] == '#')
                return false;

            // Store pushed tile
            pushed.Add(pos);

            // Store push result
            result[pos.Move(dir)] = _map[pos];

            // If nothing ahead
            if (_map[pos.Move(dir)] == '.')
                return true;

            // If pushing large boxes vertically
            if (dir == Direction.North || dir == Direction.South)
            {
                // Make sure the whole box can be pushed
                if (_map[pos.Move(dir)] == '[')
                    return Possible(pos.Move(dir), dir, pushed, result) && Possible(pos.Move(dir).Move(Direction.East), dir, pushed, result);
                else if (_map[pos.Move(dir)] == ']')
                    return Possible(pos.Move(dir), dir, pushed, result) && Possible(pos.Move(dir).Move(Direction.West), dir, pushed, result);
            }

            // Small and wide boxes are treated the same horizontally
            return Possible(pos.Move(dir), dir, pushed, result);
        }
    }
}

