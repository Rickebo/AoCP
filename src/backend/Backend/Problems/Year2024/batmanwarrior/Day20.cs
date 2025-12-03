using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Lib.Grids;
using Lib.Geometry;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day20 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 20);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Race Condition";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create race track
            RaceTrack raceTrack = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(raceTrack.Race(distance: 2, saved: 100)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create race track
            RaceTrack raceTrack = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(raceTrack.Race(distance: 20, saved: 100)));
            return Task.CompletedTask;
        }
    }

    public class RaceTrack
    {
        private readonly Reporter _reporter;
        private readonly CharGrid _grid;

        public RaceTrack(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Create race track
            _grid = new(input);
        }

        public int Race(int distance, int saved)
        {
            // Create queue
            PriorityQueue<IntegerCoordinate<int>, int> queue = new();

            // Queue start pos
            queue.Enqueue(_grid.Find(ch => ch == 'S'), 0);

            // Track visited tiles and their cost
            Dictionary<IntegerCoordinate<int>, int> tileCosts = [];

            // Navigate
            while (queue.TryDequeue(out IntegerCoordinate<int> pos, out int cost))
            {
                // Store cost for this tile
                tileCosts[pos] = cost;

                // Queue free neighbouring tiles that has not been visited yet
                foreach (IntegerCoordinate<int> neighbour in pos.Neighbours.Where(x => _grid[x] != '#' && !tileCosts.ContainsKey(x)))
                    queue.Enqueue(neighbour, cost + 1);
            }

            // Count ways to cheat over threshold
            int count = 0;
            foreach (var pair in tileCosts)
            {
                // Check if a skip is available
                for (int y = -distance; y <= distance; y++) {
                    for (int x = -distance; x <= distance; x++) {
                        IntegerCoordinate<int> currPos = new(pair.Key.X + x, pair.Key.Y + y);
                        Distance<int> dist = pair.Key.Distance(currPos);
                        if (dist.Manhattan() > distance || !_grid.Contains(currPos))
                            continue;

                        // Check if currpos is a tiled that has been visited before
                        if (!tileCosts.TryGetValue(currPos, out int val))
                            continue;

                        // If cost at this tile is higher than prev + 2 + saved
                        int currySaved = val - pair.Value;
                        if (currySaved >= (dist.Manhattan() + saved))
                            count++;
                    }
                }
            }

            return count;
        }
    }
}

