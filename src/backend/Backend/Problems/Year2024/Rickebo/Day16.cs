using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.Rickebo;

public class Day16 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 16);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Reindeer Maze";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            var data = Data.Parse(input).FindBestScore(reporter);

            reporter.ReportSolution(data);

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            var data = Data.Parse(input).CountBestPositions(reporter);

            reporter.ReportSolution(data);

            return Task.CompletedTask;
        }
    }


    private static IEnumerable<(int, DirectionPosition[])> FindBestPath(
        CharGrid grid,
        IntegerCoordinate<int> position,
        IntegerCoordinate<int> destination,
        Direction initialDirection = Direction.East,
        bool all = false
    )
    {
        var frontier = new PriorityQueue<SearchEntry, int>(
            [
                (new SearchEntry(new DirectionPosition(position, initialDirection), null),
                    0)
            ]
        );

        int? bestScore = null;
        var cache = new Dictionary<DirectionPosition, int>();

        while (frontier.TryDequeue(out var current, out var currentScore))
        {
            if (currentScore > bestScore)
                continue;

            if (current.Pos.Position == destination)
            {
                if (bestScore < currentScore)
                    yield break;

                bestScore = currentScore;

                yield return (currentScore,
                    current.GetPath().Select(x => x.Pos).ToArray());

                if (all)
                    continue;

                yield break;
            }

            cache[current.Pos] = currentScore;

            var direction = current.Pos.Direction;
            foreach (var dir in direction.Neighbours())
            {
                var penalty = 1 + (dir != direction ? 1000 : 0);
                var nextPos = current.Pos.Position.Move(dir);
                var nextDp = new DirectionPosition(nextPos, dir);

                if (!grid.Contains(nextPos) || grid[nextPos] == '#')
                    continue;

                var nextScore = currentScore + penalty;
                if (cache.TryGetValue(nextDp, out var cachedNextScore) &&
                    cachedNextScore < nextScore)
                    continue;

                var ne = new SearchEntry(
                    nextDp,
                    current
                );

                frontier.Enqueue(
                    ne,
                    currentScore + penalty
                );
            }
        }
    }

    private record DirectionPosition(
        IntegerCoordinate<int> Position,
        Direction Direction
    );

    private record SearchEntry(
        DirectionPosition Pos,
        SearchEntry? Previous
    )
    {
        public IEnumerable<SearchEntry> GetPath(bool includeCurrent = true)
        {
            var c = this;
            while (c != null)
            {
                if (includeCurrent)
                    yield return c;

                includeCurrent = true;
                c = c.Previous;
            }
        }
    }

    private record Data(
        CharGrid Grid,
        IntegerCoordinate<int> Start,
        IntegerCoordinate<int> End
    )
    {
        public static Data Parse(string input)
        {
            var grid = Parser.ParseCharGrid(input).FlipY();
            grid.Replace('.', ' ');
            var src = grid.Find(cell => cell == 'S');
            var dst = grid.Find(cell => cell == 'E');

            return new Data(grid, src, dst);
        }

        public int FindBestScore(Reporter? reporter = null)
        {
            var solution = FindBestPath(Grid, Start, End) ??
                           throw new Exception("Found no path to end point.");

            var (score, path) = solution.First();

            reporter?.ReportGlyphGridUpdate(
                builder => builder
                    .WithWidth(Grid.Width)
                    .WithHeight(Grid.Height)
                    .WithEntries(
                        Grid.Coordinates.Where(c => Grid[c] == '#'),
                        (gb, c) => gb
                            .WithForeground(Color.White * 0.5)
                            .WithCoordinate(c)
                            .WithGlyph("#")
                    )
                    .WithPath(
                        path.Select(dp => dp.Position),
                        foreground: Color.From(red: 1)
                    )
            );

            return score;
        }

        public int CountBestPositions(Reporter? reporter)
        {
            var solutions = FindBestPath(Grid, Start, End, all: true);

            var positions = new HashSet<IntegerCoordinate<int>>();
            foreach (var (_, path) in solutions)
            {
                reporter?.ReportLine($"Found path of length {path.Length}");
                foreach (var pos in path)
                    positions.Add(pos.Position);
            }

            reporter?.ReportGlyphGridUpdate(
                builder => builder
                    .WithWidth(Grid.Width)
                    .WithHeight(Grid.Height)
                    .WithEntries(
                        Grid.Coordinates.Where(c => Grid[c] == '#'),
                        (gb, c) => gb
                            .WithForeground(Color.White * 0.5)
                            .WithCoordinate(c)
                            .WithGlyph("#")
                    )
                    .WithPath(
                        positions,
                        foreground: Color.From(red: 1)
                    )
            );


            return positions.Count;
        }
    }
}