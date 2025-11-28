using Common;
using Common.Updates;
using Lib.Coordinate;
using Lib.Enums;
using Lib.Extensions;
using Lib.Grid;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025Codelight.batmanwarrior;
public class Day21 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 21);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Coffee Quest";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create startup project
            Office office = new(input, reporter);

            // Get lowest path cost
            int lowestCost = office.LowestPathCost();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(lowestCost.ToString()));
            return Task.CompletedTask;
        }

        private class Office
        {
            private readonly Reporter _reporter;

            private readonly CharGrid _floor;
            private readonly ArrayGrid<IntegerCoordinate<int>?> _path;

            private readonly IntegerCoordinate<int> _startPos;
            private readonly IntegerCoordinate<int> _endPos;

            public Office(string input, Reporter reporter)
            {
                // Save reporter for printing
                _reporter = reporter;

                // Create floor grid from input
                _floor = new CharGrid(input);

                // Init path grid
                _path = new ArrayGrid<IntegerCoordinate<int>?>(_floor.Width, _floor.Height);
                foreach (IntegerCoordinate<int> pos in _floor.Coordinates)
                    _path[pos] = null;

                // Find start and end positions
                _startPos = _floor.Find(x => x == 'S');
                _endPos = _floor.Find(x => x == 'E');

                // Print office floor
                _reporter.ReportStringGridUpdate(
                    _floor,
                    (builder, coordinate, val) => builder
                        .WithCoordinate(coordinate)
                        .WithText(ColorCell(coordinate))
                );
            }

            private string ColorCell(IntegerCoordinate<int> coordinate)
            {
                // Color formating
                return _floor[coordinate] switch
                {
                    '#' => "#000000", // Obstacle
                    'S' => "#0000FF", // Start
                    'E' => "#00FF00", // End
                    _ => "#444444", // Floor
                };
            }

            public int LowestPathCost()
            {
                // Create prio queue
                PriorityQueue<CoffeSeeker, int> prioQueue = new();

                // Keep track of visited floor tiles
                HashSet<IntegerCoordinate<int>> visited = [];

                // Add coffe seeker at start position
                prioQueue.Enqueue(new CoffeSeeker(_startPos, Direction.East), 0);

                // Start looking for coffee
                int finalCost = 0;
                while (prioQueue.TryDequeue(out CoffeSeeker? seeker, out int cost))
                {
                    // Check if already visited
                    if (!visited.Add(seeker.Pos))
                        continue;

                    // Check if this position is a wall or obstacle
                    if (!_floor.Contains(seeker.Pos) || _floor[seeker.Pos] == '#')
                        continue;

                    // Check if end has been reached
                    if (seeker.Pos == _endPos)
                    {
                        finalCost = cost;
                        break;
                    }

                    // Color current position
                    if (seeker.Pos != _startPos)
                        _reporter.ReportStringGridUpdate(seeker.Pos, "#666666");

                    // Queue possible steps
                    IntegerCoordinate<int> newPos;
                    Direction newDir;

                    // Forward
                    newPos = seeker.Pos.Move(seeker.Dir);
                    prioQueue.Enqueue(new CoffeSeeker(newPos, seeker.Dir), cost + 1);
                    _path[newPos] = seeker.Pos;

                    // Right
                    newDir = seeker.Dir.RotateClockwise();
                    newPos = seeker.Pos.Move(newDir);
                    prioQueue.Enqueue(new CoffeSeeker(newPos, newDir), cost + 3);
                    _path[newPos] = seeker.Pos;

                    // Left
                    newDir = seeker.Dir.RotateCounterClockwise();
                    newPos = seeker.Pos.Move(newDir);
                    prioQueue.Enqueue(new CoffeSeeker(newPos, newDir), cost + 2);
                    _path[newPos] = seeker.Pos;
                }

                // End has been reached
                if (finalCost > 0)
                {
                    // Color path back to start
                    IntegerCoordinate<int>? walkedTile = _path[_endPos];
                    while (walkedTile != null && walkedTile != _startPos)
                    {
                        _reporter.ReportStringGridUpdate(walkedTile, "#AAAAAA");
                        walkedTile = _path[(IntegerCoordinate<int>)walkedTile];
                    }
                }

                return finalCost;
            }
        }

        public record CoffeSeeker(IntegerCoordinate<int> Pos, Direction Dir);
    }
}