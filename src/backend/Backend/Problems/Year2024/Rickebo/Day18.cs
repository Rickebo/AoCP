using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Text;
using Lib.Geometry;
using Lib.Grids;
using Lib.Color;

namespace Backend.Problems.Year2024.Rickebo;

public class Day18 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 18);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "RAM Run";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(string.Join(",", Data.Parse(input, reporter).Part1(reporter)));

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(string.Join(",", Data.Parse(input).Part2(reporter)));

            return Task.CompletedTask;
        }
    }

    public record Data(
        IntGrid Grid,
        IntegerCoordinate<int>[] BytePositions,
        IntegerCoordinate<int> Source,
        IntegerCoordinate<int> Destination,
        int Bytes,
        int MaxBytes
    )
    {
        public static Data Parse(string input, Reporter? reporter = null)
        {
            var lines = input.SplitLines();
            var dim = lines.Length < 100 ? 7 : 71;
            var grid = new IntGrid(int.MaxValue, dim, dim);
            var result = new List<IntegerCoordinate<int>>();
            var index = 1;
            var bytes = dim < 70 ? 12 : 1024;

            var src = new IntegerCoordinate<int>(0, 0);
            var dst = new IntegerCoordinate<int>(dim - 1, dim - 1);

            var builder = GlyphGridUpdate.Builder()
                .WithHeight(dim)
                .WithWidth(dim)
                .WithEntry(
                    entryBuilder => entryBuilder
                        .WithCoordinate(src)
                        .WithGlyph("+")
                        .WithForeground(Color.From(blue: 1))
                )
                .WithEntry(
                    entryBuilder => entryBuilder
                        .WithCoordinate(dst)
                        .WithGlyph("+")
                        .WithForeground(Color.From(green: 1))
                );

            foreach (var line in lines)
            {
                var xy = Parser.GetValues<int>(line);
                if (xy.Length != 2)
                    throw new Exception("Expected two integer values per line.");

                var pos = new IntegerCoordinate<int>(xy[0], xy[1]);
                grid[pos] = index++;
                result.Add(pos);

                if (grid[pos] <= bytes)
                    builder = builder
                        .WithEntry(
                            entryBuilder => entryBuilder
                                .WithCoordinate(pos)
                                .WithForeground(Color.White * 0.5)
                                .WithGlyph("#")
                        );
            }

            reporter?.Report(builder.Build());

            return new Data(
                grid,
                result.ToArray(),
                src,
                dst,
                bytes,
                index
            );
        }

        private int CalculateScore(IntegerCoordinate<int> pos, int travelCost) =>
            travelCost + (pos - Destination).ManhattanLength();

        private record SearchEntry(IntegerCoordinate<int> Source, SearchEntry? Parent, int Distance);

        public int Part1(Reporter? reporter = null)
        {
            var path = FindShortestPath().ToArray();

            reporter?.ReportGlyphGridUpdate(
                builder => builder
                    .WithPath(path, foreground: Color.From(green: 1, blue: 1))
                    .WithEntry(
                        e => e
                            .WithCoordinate(Source)
                            .WithGlyph("+")
                            .WithForeground(Color.From(blue: 1))
                    )
                    .WithEntry(
                        e => e
                            .WithCoordinate(Destination)
                            .WithGlyph("+")
                            .WithForeground(Color.From(green: 1))
                    )
            );

            // Number of steps = positions - 1
            return path.Length - 1;
        }

        public string Part2(Reporter? reporter = null)
        {
            IntegerCoordinate<int>[]? prevPath = null;
            HashSet<IntegerCoordinate<int>>? prevPathSet = null;

            for (var i = Bytes; i < MaxBytes; i++)
            {
                var blocker = BytePositions[i];
                // If the previous best path was not broken, it is still the best path
                if (prevPathSet != null && !prevPathSet.Contains(blocker))
                    continue;

                var path = FindShortestPath(i).ToArray();
                // If there still is a path, update the prev path and continue
                if (path.Length != 0)
                {
                    prevPath = path;
                    prevPathSet = path.ToHashSet();
                    continue;
                }

                if (prevPath != null)
                    reporter?.ReportGlyphGridUpdate(
                        builder => builder
                            .WithWidth(Grid.Width)
                            .WithHeight(Grid.Height)
                            .WithEntries(
                                Grid.Coordinates.Where(p => Grid[p] <= i),
                                (gb, pos) => gb
                                    .WithCoordinate(pos)
                                    .WithGlyph("#")
                                    .WithForeground(Color.White * 0.5)
                            )
                            .WithPath(prevPath, foreground: Color.From(green: 1, blue: 1))
                            .WithEntry(
                                b => b
                                    .WithCoordinate(blocker)
                                    .WithForeground(Color.From(red: 1))
                                    .WithGlyph("#")
                            )
                            .WithEntry(
                                b => b
                                    .WithCoordinate(Source)
                                    .WithForeground(Color.From(blue: 1))
                                    .WithGlyph("+")
                            )
                            .WithEntry(
                                b => b
                                    .WithCoordinate(Destination)
                                    .WithForeground(Color.From(green: 1))
                                    .WithGlyph("+")
                            )
                    );

                return $"{blocker.X},{blocker.Y}";
            }

            throw new Exception("Path found for all amount of bytes.");
        }

        public IEnumerable<IntegerCoordinate<int>> FindShortestPath(int bytes = -1)
        {
            if (bytes < 1)
                bytes = Bytes;

            var frontier = new PriorityQueue<SearchEntry, int>(
                [
                    (new SearchEntry(Source, null, 0), CalculateScore(Source, 0))
                ]
            );

            var visited = new HashSet<IntegerCoordinate<int>>();

            while (frontier.TryDequeue(out var item, out _))
            {
                var (pos, _, travelCost) = item;
                if (!visited.Add(pos))
                    continue;

                if (pos == Destination)
                {
                    var current = item;
                    while (current != null)
                    {
                        yield return current.Source;
                        current = current.Parent;
                    }

                    yield break;
                }

                foreach (var neighbour in pos.Neighbours)
                {
                    if (visited.Contains(neighbour) || !Grid.Contains(neighbour) || Grid[neighbour] <= bytes)
                        continue;

                    var neighbourTravelCost = travelCost + 1;
                    var neighbourPriority = CalculateScore(neighbour, neighbourTravelCost);
                    frontier.Enqueue(new SearchEntry(neighbour, item, neighbourTravelCost), neighbourPriority);
                }
            }
        }
    }
}

