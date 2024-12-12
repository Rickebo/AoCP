using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day12 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 12);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Garden Groups";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create garden
            Garden garden = new(input, reporter);

            // Get regions of garden
            garden.GetRegions();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(garden.FencingCost(discount: false)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create garden
            Garden garden = new(input, reporter);

            // Get regions of garden
            garden.GetRegions();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(garden.FencingCost(discount: true)));
            return Task.CompletedTask;
        }
    }

    public class Garden
    {
        private readonly Reporter _reporter;
        private readonly CharGrid _grid;
        private readonly List<Region> _regions = [];

        public Garden(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Create garden
            _grid = new(input);

            // Print grid
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_grid, "#FFFFFF", "#000000"));
        }

        public void GetRegions()
        {
            // Keep track of visited
            HashSet<IntegerCoordinate<int>> visited = [];

            foreach (var pos in _grid.Coordinates)
            {
                // If this pos has been added to a region already
                if (visited.Contains(pos))
                    continue;

                // Create new region 
                Region region = new(_grid[pos], _reporter);

                // Queue first pos of new region
                Queue<IntegerCoordinate<int>> queue = [];
                queue.Enqueue(pos);

                while (queue.TryDequeue(out var currPos))
                {
                    // Seen before
                    if (!visited.Add(currPos))
                        continue;

                    // If position is outside grid or not of same type
                    if (!_grid.Contains(currPos) || _grid[currPos] != region.Type)
                        continue;

                    // Check neighbours
                    foreach (var neighbour in currPos.Neighbours)
                    {
                        // Check if tile is outside region or not part of same region type
                        if (!_grid.Contains(neighbour) || _grid[currPos] != _grid[neighbour])
                        {
                            // Add fence
                            region.AddEdge(currPos, neighbour);
                            continue;
                        }
                        else
                        {
                            // Part of same region
                            queue.Enqueue(neighbour);
                        }
                    }

                    // Add plot to region
                    region.AddPlot(currPos);
                }

                // Add region
                _regions.Add(region);
            }
        }

        public long FencingCost(bool discount)
        {
            long sum = 0;
            foreach (Region region in _regions)
                sum += region.Cost(discount);

            return sum;
        }
    }
    
    public class Region(char type, Reporter reporter)
    {
        private readonly Reporter _reporter = reporter;
        public char Type = type;
        public List<IntegerCoordinate<int>> Plots = [];
        public HashSet<Edge> Edges = [];

        public void AddPlot(IntegerCoordinate<int> pos)
        {
            if (!Plots.Contains(pos))
                Plots.Add(pos);
        }

        public void AddEdge(IntegerCoordinate<int> insidePos, IntegerCoordinate<int> outsidePos)
        {
            Edges.Add(new(insidePos, outsidePos));
        }

        public int Sides()
        {
            HashSet<Edge> visited = [];
            List<HashSet<Edge>> sides = [];

            // Look at every edge
            foreach (var leadEdge in Edges)
            {
                if (visited.Contains(leadEdge))
                    continue;

                Queue<Edge> queue = [];
                HashSet<Edge> side = [];
                queue.Enqueue(leadEdge);

                while (queue.TryDequeue(out var currEdge))
                {
                    // Side edge has been visited
                    if (!side.Add(currEdge) || !visited.Add(currEdge))
                        continue;

                    // Check if edge has neighbouring edges
                    foreach (var dir in DirectionExtensions.Cardinals)
                    {
                        Edge newEdge = new(currEdge.InsidePos.Move(dir), currEdge.OutsidePos.Move(dir));
                        if (Edges.Contains(newEdge))
                        {
                            // Add to queue
                            queue.Enqueue(newEdge);
                        }
                    }
                }

                sides.Add(side);
            }

            return sides.Count;
        }

        public long Cost(bool discount)
        {
            return discount ? Plots.Count * Sides() : Plots.Count * Edges.Count;
        }
    }

    public record Edge(IntegerCoordinate<int> InsidePos, IntegerCoordinate<int> OutsidePos);
}