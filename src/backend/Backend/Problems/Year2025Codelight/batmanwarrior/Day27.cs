using Common;
using Common.Updates;
using Lib.Geometry;
using Lib.Geometry;
using Lib.Grids;
using Lib.Color;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025Codelight.batmanwarrior;
public class Day27 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 27);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Storage Crisis";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create storage
            Storage storage = new(input, reporter);

            // Get largest rectangular free shelf space
            int largestSpace = storage.LargestSpace();

            // Send solution to frontend
            reporter.ReportSolution(largestSpace);
            return Task.CompletedTask;
        }

        private class Storage
        {
            private readonly Reporter _reporter;
            private readonly CharGrid _grid;

            public Storage(string input, Reporter reporter)
            {
                // Save for prints
                _reporter = reporter;

                // Parse input to grid
                _grid = new CharGrid(input);

                // Print storage matrix
                _reporter.ReportStringGridUpdate(_grid, (builder, coordinate, val) =>
                builder.WithCoordinate(coordinate).WithColor(_grid[coordinate] switch
                {
                    '#' => Colors.Black,    // Occupied
                    _ => Colors.Gray,       // Free
                }));
            }

            public int LargestSpace()
            {
                // Check for rectangle spaces everywhere on the grid
                TotalMax totalMax = new(new(0, 0), 0, 0, 0);
                foreach (IntegerCoordinate<int> pos in _grid.Coordinates)
                {
                    // Skip occupied spots
                    if (_grid[pos] == '#')
                        continue;

                    // Get maximum possible space for this coord
                    int localMaxWidth = _grid.CountRepeating(pos, Direction.East);
                    int localMaxHeight = _grid.CountRepeating(pos, Direction.North);

                    // Check if worth looking
                    if (localMaxWidth * localMaxHeight <= totalMax.Area)
                        continue;

                    // Local vars
                    var area = Math.Max(localMaxWidth, localMaxHeight);
                    var width = localMaxWidth > localMaxHeight ? localMaxWidth : 1;
                    var height = localMaxHeight > localMaxWidth ? localMaxHeight : 1;
                    LocalMax localMax = new(area, width, height);

                    // Calculate possible rectangles
                    bool keepLooking = true;
                    for (int y = 1; y < localMaxHeight; y++)
                    {
                        for (int x = 1; x < localMaxWidth; x++)
                        {
                            if (_grid[pos.X + x, pos.Y + y] == '#')
                            {
                                localMaxWidth = x;

                                // Local max area got smaller, check if less than total max
                                if (localMaxWidth * localMaxHeight <= totalMax.Area)
                                    keepLooking = false;

                                break;
                            }

                            // Determine area and check against local max
                            var w = x + 1;
                            var h = y + 1;
                            var a = w * h;
                            if (a > localMax.Area)
                            {
                                localMax.Area = a;
                                localMax.Width = w;
                                localMax.Height = h;
                            }
                        }

                        if (!keepLooking)
                            break;
                    }

                    // Check against total max
                    if (localMax.Area > totalMax.Area)
                    {
                        totalMax.Pos = pos;
                        totalMax.Area = localMax.Area;
                        totalMax.Width = localMax.Width;
                        totalMax.Height = localMax.Height;
                    }
                }
                
                // Paint the largest rectangle
                _reporter.Report(StringGridUpdate.FromRect(
                    totalMax.Pos, 
                    totalMax.Width, 
                    totalMax.Height, 
                    Colors.IndianRed
                ));

                // Paint rectangle origin
                _reporter.ReportStringGridUpdate(totalMax.Pos, ColorHex.LightGreen);

                return totalMax.Area;
            }

            private struct TotalMax(IntegerCoordinate<int> pos, int area, int width, int height)
            {
                public IntegerCoordinate<int> Pos = pos;
                public int Area = area;
                public int Width = width;
                public int Height = height;
            }

            private struct LocalMax(int area, int width, int height)
            {
                public int Area = area;
                public int Width = width;
                public int Height = height;
            }
        }
    }
}

