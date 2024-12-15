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
            Warehouse warehouse = new(input, wide: false, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(warehouse.MoveAround(wide: false, -1)));
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
            reporter.Report(FinishedProblemUpdate.FromSolution(warehouse.MoveAround(wide: true, -1)));
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
                if (row.Contains('#'))
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

        public int MoveAround(bool wide, int steps)
        {
            // Move around with robot
            for (int i = 0; i < _moves.Length; i++)
            {
                // Find robot
                IntegerCoordinate<int> pos = _map.Find(ch => ch == '@');

                // Get direction
                Direction dir = DirectionExtensions.Parse(_moves[i]);

                // Check if push possible
                List<IntegerCoordinate<int>> tiles = [];
                Dictionary<IntegerCoordinate<int>, char> result = [];
                if (Possible(pos, dir, tiles, result))
                {
                    // Erase all tiles
                    foreach (IntegerCoordinate<int> tilePos in tiles)
                        _map[tilePos] = '.';

                    // Perform push result
                    foreach (KeyValuePair<IntegerCoordinate<int>, char> pair in result)
                        _map[pair.Key] = pair.Value;
                }

                // For debugging
                steps--;
                if (steps == 0)
                    break;
            }

            // Print grid
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_map, "#FFFFFF", "#000000"));

            int sum = 0;
            foreach (IntegerCoordinate<int> pos in _map.FindAll(x => x == 'O' || x == '['))
                sum += 100 * (_map.Height - 1 - pos.Y) + pos.X;

            return sum;
        }

        private bool Possible(IntegerCoordinate<int> pos, Direction dir, List<IntegerCoordinate<int>> tiles, Dictionary<IntegerCoordinate<int>, char> result)
        {
            // If wall ahead
            if (_map[pos.Move(dir)] == '#')
                return false;

            // Store tile
            tiles.Add(pos);

            // Store move
            result[pos.Move(dir)] = _map[pos];

            // If nothing ahead
            if (_map[pos.Move(dir)] == '.')
                return true;

            // If large box ahead, going vertically
            if (_map[pos.Move(dir)] == '[' && (dir == Direction.North || dir == Direction.South))
                return Possible(pos.Move(dir), dir, tiles, result) && Possible(pos.Move(dir).Move(Direction.East), dir, tiles, result);

            // If large box ahead, go deeper
            if (_map[pos.Move(dir)] == ']' && (dir == Direction.North || dir == Direction.South))
                return Possible(pos.Move(dir), dir, tiles, result) && Possible(pos.Move(dir).Move(Direction.West), dir, tiles, result);

            // Small and wide boxes are treated the same horizontally
            return Possible(pos.Move(dir), dir, tiles, result);
        }
    }
}