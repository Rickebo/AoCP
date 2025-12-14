using Common;
using Lib.Geometry;
using Lib.Grids;
using Lib.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day09 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 09);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Movie Theater";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Send solution to frontend (Part 1)
            reporter.ReportSolution(new Solver(input, reporter, 1).PartOne());
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Send solution to frontend (Part 2)
            reporter.ReportSolution(new Solver(input, reporter, 2).PartTwo());
            return Task.CompletedTask;
        }
    }

    public class Solver
    {
        private readonly Reporter _reporter;
        private readonly InfiniteGrid<char, long> _grid;
        private readonly List<IntegerCoordinate<long>> _redTiles = [];
        private readonly long width;
        private readonly long height;

        public Solver(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            List<(long X, long Y)> inputCoords = [];
            foreach (var line in input.SplitLines())
            {
                int[] vals = Parser.GetValues<int>(line);
                if (vals.Length != 2)
                    throw new ProblemException("Invalid input.");

                inputCoords.Add((X: vals[0], Y: vals[1]));
                minX = Math.Min(minX, vals[0]);
                maxX = Math.Max(maxX, vals[0]);
                minY = Math.Min(minY, vals[1]);
                maxY = Math.Max(maxY, vals[1]);
            }
            if (inputCoords.Count < 2)
                throw new ProblemException("Invalid input.");

            // Init print grid
            width = maxX - minX + 1;
            height = maxY - minY + 1;
            _reporter.ReportGlyphGridUpdate(
                builder => builder
                    .WithWidth((int)width)
                    .WithHeight((int)height)
                    .WithClear()
            );

            // Populate data with adjusted coords
            _grid = new();
            foreach (var (X, Y) in inputCoords)
            {
                IntegerCoordinate<long> adjusted = new(X - minX, maxY - 1 - (Y - minY));
                _redTiles.Add(new(adjusted.X, adjusted.Y));
                _grid[adjusted] = '#';

                // Print
                //_reporter.ReportStringGridUpdate(adjusted, ColorHex.Red);
            }
        }

        public long PartOne()
        {
            long max = 0;

            for (int i = 0; i < _redTiles.Count - 1; i++)
            {
                for (int j = i + 1; j < _redTiles.Count; j++)
                {
                    Distance<long> distance = _redTiles[i].Distance(_redTiles[j]);
                    long area = (Math.Abs(distance.X) + 1) * (Math.Abs(distance.Y) + 1);
                    if (area > max)
                        max = area;
                }
            }

            return max;
        }

        public long PartTwo()
        {
            long max = 0;

            // Create outline
            for (int i = 0; i < _redTiles.Count; i++)
            {
                IntegerCoordinate<long> start = i == 0 ? _redTiles[^1] : _redTiles[i - 1];
                IntegerCoordinate<long> end = _redTiles[i];

                foreach (var coord in start.MoveTo(end).Skip(1))
                {
                    _grid[coord] = '#';
                    //_reporter.ReportStringGridUpdate(coord, ColorHex.Green);
                }
            }

            for (int i = 0; i < _redTiles.Count - 1; i++)
            {
                for (int j = i + 1; j < _redTiles.Count; j++)
                {
                    IntegerCoordinate<long> A = _redTiles[i];
                    IntegerCoordinate<long> B = _redTiles[j];
                    Distance<long> distance = A.Distance(B);
                    long area = (Math.Abs(distance.X) + 1) * (Math.Abs(distance.Y) + 1);
                    if (area > max)
                    {
                        // Rectangle bounds
                        long minX = Math.Min(A.X, B.X);
                        long maxX = Math.Max(A.X, B.X);
                        long minY = Math.Min(A.Y, B.Y);
                        long maxY = Math.Max(A.Y, B.Y);

                        // Check if rectangle contains red tiles (vertices)
                        bool containsRedTile = false;
                        foreach (var tile in _redTiles)
                        {
                            // Skip rectangle corners
                            if (tile == A || tile == B)
                                continue;

                            // Vertice within rectangle
                            if (tile.X > minX && tile.X < maxX &&
                                tile.Y > minY && tile.Y < maxY)
                            {
                                containsRedTile = true;
                                break;
                            }
                        }
                        if (containsRedTile)
                            continue;

                        // Scan vertically to see if rectangle is inside
                        bool verticalScan = true;
                        for (long x = minX; x <= maxX; x++)
                        {
                            // Start outside
                            bool inside = false;
                            Direction enterDir = Direction.None;
                            bool enterState = false;
                            for (long y = -1; y <= maxY; y++)
                            {
                                IntegerCoordinate<long> pos = new(x, y);

                                // Entering #
                                if (_grid.Contains(pos))
                                {
                                    // Save entering state
                                    if (enterDir == Direction.None)
                                    {
                                        if (_grid.Contains(pos.Move(Direction.West)))
                                            enterDir |= Direction.West;
                                        if (_grid.Contains(pos.Move(Direction.East)))
                                            enterDir |= Direction.East;
                                        enterState = inside;
                                    }

                                    // Border is always inside
                                    inside = true;
                                }

                                // Leaving #
                                else if (!_grid.Contains(pos) && enterDir != Direction.None)
                                {
                                    // Exit dir
                                    Direction exitDir = _grid.Contains(pos.Move(Direction.South).Move(Direction.West)) ? Direction.West : Direction.East;

                                    // Check enter dir against exit dir
                                    if (enterDir == (Direction.West | Direction.East) ||
                                        enterDir == Direction.West && exitDir == Direction.East ||
                                        enterDir == Direction.East && exitDir == Direction.West)
                                    {
                                        // Toggle enter state
                                        inside = !enterState;
                                    }
                                    else
                                    {
                                        // Keep enter state
                                        inside = enterState;
                                    }

                                    // Reset entering states
                                    enterDir = Direction.None;
                                    enterState = false;
                                }

                                // If inside rectangle, check if inside is not true
                                if (y >= minY && !inside)
                                {
                                    verticalScan = false;
                                    break;
                                }
                            }

                            if (!verticalScan)
                                break;
                        }

                        if (!verticalScan)
                            continue;

                        // Scan horizontally to see if rectangle is inside
                        bool horizontalScan = true;
                        for (long y = minY; y <= maxY; y++)
                        {
                            // Start outside
                            bool inside = false;
                            Direction enterDir = Direction.None;
                            bool enterState = false;
                            for (long x = -1; x <= maxX; x++)
                            {
                                IntegerCoordinate<long> pos = new(x, y);

                                // Entering #
                                if (_grid.Contains(pos))
                                {
                                    // Save entering state
                                    if (enterDir == Direction.None)
                                    {
                                        if (_grid.Contains(pos.Move(Direction.North)))
                                            enterDir |= Direction.North;
                                        if (_grid.Contains(pos.Move(Direction.South)))
                                            enterDir |= Direction.South;
                                        enterState = inside;
                                    }

                                    // Border is always inside
                                    inside = true;
                                }

                                // Leaving #
                                else if (!_grid.Contains(pos) && enterDir != Direction.None)
                                {
                                    // Exit dir
                                    Direction exitDir = _grid.Contains(pos.Move(Direction.West).Move(Direction.North)) ? Direction.North : Direction.South;

                                    // Check enter dir against exit dir
                                    if (enterDir == (Direction.North | Direction.South) ||
                                        enterDir == Direction.North && exitDir == Direction.South ||
                                        enterDir == Direction.South && exitDir == Direction.North)
                                    {
                                        // Toggle enter state
                                        inside = !enterState;
                                    }
                                    else
                                    {
                                        // Keep enter state
                                        inside = enterState;
                                    }

                                    // Reset entering dir
                                    enterDir = Direction.None;
                                }

                                // If inside rectangle, check if inside is not true
                                if (x >= minX && !inside)
                                {
                                    horizontalScan = false;
                                    break;
                                }
                            }

                            if (!horizontalScan)
                                break;
                        }

                        if (horizontalScan && verticalScan)
                        {
                            max = area;
                            _reporter.ReportLine($"i: {i}, j: {j}, max: {max}");
                        }
                    }
                }
            }

            return max;
        }
    }
}

