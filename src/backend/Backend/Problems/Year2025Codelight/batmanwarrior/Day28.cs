using Common;
using Common.Updates;
using Lib.Geometry;
using Lib.Grids;
using Lib.Color;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Backend.Problems.Year2025Codelight.batmanwarrior;
public class Day28 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 28);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Peek at the DJ!";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create dance floor
            DanceFloor danceFloor = new(input, reporter);

            // Get lifts required for everyone to see DJ
            int lifts = danceFloor.TotalLifts();

            // Send solution to frontend
            reporter.ReportSolution(lifts);
            return Task.CompletedTask;
        }

        private class DanceFloor
        {
            private readonly IntGrid _grid;
            private readonly Reporter _reporter;
            private readonly HashSet<IntegerCoordinate<int>> _locked = [];
            private int _lifts = 0;
            private readonly int _minHeight = int.MaxValue;
            private readonly int _maxHeight = int.MinValue;

            public DanceFloor(string input, Reporter reporter)
            {
                // Save for prints
                _reporter = reporter;

                // Parse string input
                var rows = input.SplitLines();
                var height = rows.Length;
                var width = rows[0].Length;

                // Create grid and find min max
                var gridValues = new int[width, height];
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var val = rows[^(y + 1)][x] - '0';
                        _minHeight = Math.Min(val, _minHeight);
                        _maxHeight = Math.Max(val, _maxHeight);
                        gridValues[x, y] = val;
                    }
                }
                _grid = new(gridValues);

                // Print heatmap
                _reporter.Report(StringGridUpdate.FromColorGrid(Color.Heatmap(
                    _grid, Colors.Blue, Colors.Red, _minHeight, _maxHeight)));
            }

            public int TotalLifts()
            {
                // Lift everyone so they can see DJ
                for (int i = _minHeight; i < _maxHeight; i++)
                    Lift(i);

                return _lifts;
            }

            private void Lift(int height)
            {
                // Retrieve all persons with this height that are not already locked
                HashSet<IntegerCoordinate<int>> personsAtHeight = [.. _grid.FindAll((p) => p == height).Where((p) => !_locked.Contains(p))];

                // Flood fill
                while (personsAtHeight.Count > 0)
                {
                    // Create queue and visited hashset
                    PriorityQueue<IntegerCoordinate<int>, int> queue = new();
                    HashSet<IntegerCoordinate<int>> visited = [];

                    // Get person
                    IntegerCoordinate<int> person = personsAtHeight.Last();

                    // Add person to queue
                    queue.Enqueue(person, 0);

                    // Remove from list
                    personsAtHeight.Remove(person);

                    while (queue.TryDequeue(out var pos, out var lift))
                    {
                        if (lift > 0)
                        {
                            foreach (var visitedPos in visited)
                            {
                                _grid[visitedPos] += lift;
                                _lifts += lift;

                                personsAtHeight.Remove(visitedPos);

                                // Print new height to heatmap
                                _reporter.ReportStringGridUpdate(
                                    visitedPos,
                                    Color.Between(Colors.Blue, Colors.Red, _grid[visitedPos], _minHeight, _maxHeight
                                ));
                            }

                            break;
                        }

                        // If position is locked
                        if (_locked.Contains(pos))
                        {
                            // Lock visited tiles
                            foreach (var visitedPos in visited)
                            {
                                _locked.Add(visitedPos);
                                personsAtHeight.Remove(visitedPos);
                            }

                            break;
                        }

                        // Add visited pos
                        visited.Add(pos);

                        // If at the edge
                        if (_grid.OnOutline(pos))
                        {
                            // Lock visited tiles
                            foreach (var visitedPos in visited)
                            {
                                _locked.Add(visitedPos);
                                personsAtHeight.Remove(visitedPos);
                            }

                            break;
                        }

                        // Queue up neighbours
                        foreach (var neighbour in pos.Neighbours())
                            if (!visited.Contains(neighbour))
                                queue.Enqueue(neighbour, Math.Max(0, _grid[neighbour] - _grid[pos]));
                    }
                }
            }
        }
    }
}

