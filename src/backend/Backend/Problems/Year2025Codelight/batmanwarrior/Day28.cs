using Common;
using Common.Updates;
using Lib.Coordinate;
using Lib.Grid;
using Lib.Color;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            private readonly Reporter _reporter;
            private readonly IntGrid _grid;
            private readonly Dictionary<IntegerCoordinate<int>, IntegerCoordinate<int>> _path = [];
            private const int _minValue = 0;
            private const int _maxValue = 9;

            public DanceFloor(string input, Reporter reporter)
            {
                // Save for prints
                _reporter = reporter;

                // Parse input to grid
                _grid = new IntGrid(input);

                // Print heatmap
                _reporter.Report(StringGridUpdate.FromColorGrid(Color.Heatmap(
                    _grid, Colors.Blue, Colors.Red, _minValue, _maxValue)));
            }

            public int TotalLifts()
            {
                // Lift everyone so they can see DJ
                var totalLift = 0;
                Dictionary<IntegerCoordinate<int>, int> cache = [];
                foreach (var coord in _grid.Coordinates)
                    totalLift += LiftPerson(coord, cache);

                return totalLift;
            }

            private int LiftPerson(IntegerCoordinate<int> pos, Dictionary<IntegerCoordinate<int>, int> cache)
            {
                // Create queue and visited hashset 
                PriorityQueue<IntegerCoordinate<int>, int> queue = new();
                HashSet<IntegerCoordinate<int>> visited = [];

                // Clear previous path
                _path.Clear();

                // Add initial position
                var initialPos = pos;
                queue.Enqueue(initialPos, 0);

                // Try to find the way to edge with the least lift required
                while (queue.TryDequeue(out var currPos, out var lift))
                {
                    // Skip visited tiles
                    if (!visited.Add(currPos))
                        continue;

                    // Check if this position is cached
                    if (cache.TryGetValue(currPos, out var maxHeight))
                    {
                        // Return the 

                        // Get lift required from this point
                        lift += Math.Max(0, maxHeight - _grid[initialPos] + lift);

                        // Add backtrack to cache
                        BacktrackToCache(initialPos, currPos, cacheAdditionalLift, cache);

                        // Update height
                        _grid[initialPos] += lift;

                        // Print new height
                        _reporter.ReportStringGridUpdate(
                            initialPos,
                            Color.Between(Colors.Blue, Colors.Red, _grid[initialPos], _minValue, _maxValue
                        ));

                        // Return lift
                        return lift;
                    }

                    // If at edge of dance floor
                    if (_grid.OnOutline(currPos))
                    {
                        // Add backtrack to cache
                        BacktrackToCache(initialPos, currPos, 0, cache);

                        // Update height
                        _grid[initialPos] += lift;

                        // Print new height
                        _reporter.ReportStringGridUpdate(
                            initialPos,
                            Color.Between(Colors.Blue, Colors.Red, _grid[initialPos], _minValue, _maxValue
                        ));

                        // Return lift
                        return lift;
                    }

                    // Queue up neighbours
                    foreach (var neighbour in currPos.Neighbours)
                    {
                        if (!visited.Contains(neighbour))
                        {
                            queue.Enqueue(neighbour, Math.Max(lift, _grid[neighbour] - _grid[initialPos]));
                            _path[neighbour] = currPos;
                        }
                    }
                }

                throw new ProblemException("BFS did not find edge of dance floor.");
            }

            private static int LiftRequired(int from, int to) => Math.Max(0, to - from);

            private void BacktrackToCache(
                IntegerCoordinate<int> startPos, 
                IntegerCoordinate<int> currPos, 
                int additionalLift, 
                Dictionary<IntegerCoordinate<int>, (int, int)> cache)
            {
                // Get height at this position
                int currHeight = _grid[currPos];

                // Keep track of maximum heights along the way
                int maxHeight = currHeight;

                // Backtrack and set cache
                while (_grid.Contains(_path[currPos]) && _path[currPos] != startPos)
                {
                    // Update currpos
                    currPos = _path[currPos];

                    // Check if additional lift is required
                    additionalLift += LiftRequired(_grid[currPos], maxHeight);

                    // Update max height
                    maxHeight = Math.Max(maxHeight, _grid[currPos]);

                    // Set additional lift to cache
                    cache[currPos] = (_grid[currPos], additionalLift);
                }

                // Set initial pos cache to zero (as it is now lifted to see DJ)
                cache[startPos] = (currHeight + additionalLift, 0);
            }
        }
    }
}