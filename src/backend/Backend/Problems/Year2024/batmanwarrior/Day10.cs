using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Coordinate;
using Lib.Enums;
using Lib.Extensions;
using Lib.Grid;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day10 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 10);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Hoof It";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create map
            Map map = new(input, reporter);

            // Check trails
            map.CheckTrails(distinct: false);

            // Report solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(map.TrailScores()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create map
            Map map = new(input, reporter);

            // Check trails
            map.CheckTrails(distinct: true);

            // Report solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(map.TrailScores()));
            return Task.CompletedTask;
        }
    }

    public class Map(string input, Reporter reporter)
    {
        private readonly IntGrid _grid = new(input);
        private readonly Reporter _reporter = reporter;
        private readonly Dictionary<IntegerCoordinate<int>, int> _scores = [];

        public void CheckTrails(bool distinct)
        {
            // Retrieve all trail starts
            Queue<Hiker> queue = [];
            foreach (IntegerCoordinate<int> pos in _grid.FindAll(x => x == 0))
            {
                queue.Enqueue(new(pos, pos));
                _scores[pos] = 0;
            }

            // See where trail leads
            HashSet<Hiker> visited = [];
            while (queue.TryDequeue(out Hiker? hiker))
            {
                // Check if the hiker has visited this spot before
                if (!distinct && !visited.Add(hiker))
                    continue;

                // If the trail end is reached
                if (_grid[hiker.CurrPos] == 9)
                {
                    _scores[hiker.StartPos] += 1;
                    continue;
                }

                // Look at cardinal directions
                foreach (Direction dir in DirectionExtensions.Cardinals)
                {
                    // Check if direction is valid
                    IntegerCoordinate<int> next = hiker.CurrPos.Move(dir);
                    if (_grid.Contains(next) && (_grid[next] - _grid[hiker.CurrPos]) == 1)
                    {
                        // Add to queue
                        queue.Enqueue(new(next, hiker.StartPos));
                    }
                }
            }
        }

        public int TrailScores() => _scores.Sum(x => x.Value);

        private record Hiker(IntegerCoordinate<int> CurrPos, IntegerCoordinate<int> StartPos);
    }
}