using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Enums;
using Lib.Grid;

namespace Backend.Problems.Year2024.Rickebo;

public class Day20 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 20);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Race Condition";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(
                RaceTrack.Parse(input).CountCheatPathsSlow(input.Length < 500 ? 2 : 100, 2, reporter)
            );

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            var cutoff = input.Length < 500 ? 50 : 100;
            var distance = 20;
            reporter.ReportSolution(RaceTrack.Parse(input).CountCheatPathsSlow(cutoff, distance, reporter));

            return Task.CompletedTask;
        }
    }

    public record RaceTrack(CharGrid Grid, IntegerCoordinate<int> Source, IntegerCoordinate<int> Destination)
    {
        public static RaceTrack Parse(string input)
        {
            var grid = new CharGrid(input).Flip(Axis.Y);
            return new RaceTrack(
                grid,
                grid.Find(x => x == 'S'),
                grid.Find(x => x == 'E')
            );
        }

        public int CountCheatPathsSlow(int cutoff, int distance, Reporter? reporter = null)
        {
            var scores = new Dictionary<IntegerCoordinate<int>, int?>();
            var visited = new HashSet<IntegerCoordinate<int>>();
            var solver = new Thread(() => FindPath(Source, scores, visited), 1000 * 1000 * 1000);
            solver.Start();
            solver.Join();

            return CountCheatScores(visited, scores, distance, cutoff);
        }

        private int CountCheatScores(
            IEnumerable<IntegerCoordinate<int>> visited,
            Dictionary<IntegerCoordinate<int>, int?> cache,
            int distance,
            int cutoff
        )
        {
            var result = 0;
            var mask = GetMask(distance);
            
            foreach (var src in visited)
            {
                if (!cache.TryGetValue(src, out var score) || score == null)
                    continue;

                foreach (var offset in mask)
                {
                    var pos = src + offset;

                    if (!Grid.Contains(pos) || !cache.TryGetValue(pos, out var postScore) || postScore == null)
                        continue;

                    var saving = score.Value - postScore.Value - (pos - src).ManhattanLength();

                    if (saving >= cutoff)
                        result++;
                }
            }

            return result;
        }

        private List<IntegerCoordinate<int>> GetMask(int size)
        {
            var mask = new List<IntegerCoordinate<int>>();

            for (var x = -size; x <= size; x++)
            {
                for (var y = -size; y <= size; y++)
                {
                    if ((x == 0 && y == 0) || Math.Abs(x) + Math.Abs(y) > size)
                        continue;

                    mask.Add(new IntegerCoordinate<int>(x, y));
                }
            }

            return mask;
        }

        public int? FindPath(
            IntegerCoordinate<int> pos,
            Dictionary<IntegerCoordinate<int>, int?> cache,
            HashSet<IntegerCoordinate<int>> visited,
            int score = 0
        )
        {
            if (!visited.Add(pos))
                return null;

            if (pos == Destination)
                return cache[pos] = score;

            if (cache.TryGetValue(pos, out var cached))
                return cached;


            int? best = null;
            foreach (var neighbour in pos.Neighbours)
            {
                if (!Grid.Contains(neighbour))
                    continue;

                if (Grid[neighbour] == '#')
                    continue;

                var sub = FindPath(neighbour, cache, visited, score + 1);

                if (best == null || sub < best)
                    best = sub + 1;
            }

            return cache[pos] = best;
        }
    }
}