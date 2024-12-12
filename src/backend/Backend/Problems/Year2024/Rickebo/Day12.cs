using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.Rickebo;

public class Day12 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 12);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Garden Groups";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(CalculateCost(Parser.ParseCharGrid(input).FlipY(), false, reporter));

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(CalculateCost(Parser.ParseCharGrid(input).FlipY(), true, reporter));
            return Task.CompletedTask;
        }
    }

    private static int CalculateCost(CharGrid grid, bool part2, Reporter reporter)
    {
        var regions = FindRegions(grid);
        return regions.Sum(
            region =>
            {
                if (part2)
                {
                    var sideCount = region.FindSides().Count();
                    var sum = region.Area * sideCount;
                    reporter.ReportLine($"{region.Value}: {region.Area} * {sideCount} = {sum}");
                    return sum;
                }
                else
                {
                    var sum = region.Area * region.Perimeter;
                    reporter.ReportLine($"{region.Value}: {region.Area} * {region.Perimeter} = {sum}");
                    return sum;
                }
            }
        );
    }

    private static HashSet<IntegerCoordinate<int>> FindNeighbours(CharGrid grid, HashSet<IntegerCoordinate<int>> region)
    {
        var neighbours = new HashSet<IntegerCoordinate<int>>();
        foreach (var pos in region.SelectMany(p => p.Neighbours))
        {
            if (!grid.Contains(pos) || region.Contains(pos))
                continue;

            neighbours.Add(pos);
        }

        return neighbours;
    }

    private static List<Region> FindRegions(CharGrid grid)
    {
        var visited = new HashSet<IntegerCoordinate<int>>();
        var regions = new List<Region>();

        foreach (var coordinate in grid.Coordinates)
        {
            if (visited.Contains(coordinate))
                continue;

            var region = new HashSet<IntegerCoordinate<int>>();
            var regionNeighbours = new HashSet<Edge>();
            SearchRegion(grid, coordinate, region, regionNeighbours);
            regions.Add(new Region(grid[coordinate], region, regionNeighbours));

            foreach (var v in region)
                visited.Add(v);
        }

        return regions;
    }

    private static void SearchRegion(
        CharGrid grid,
        IntegerCoordinate<int> source,
        HashSet<IntegerCoordinate<int>> region,
        HashSet<Edge> neighbours
    )
    {
        if (!region.Add(source))
            return;

        var srcType = grid[source];
        foreach (var neighbour in source.Neighbours)
        {
            if (!grid.Contains(neighbour) || grid[neighbour] != srcType)
                neighbours.Add(Edge.From(source, neighbour));
            else
                SearchRegion(grid, neighbour, region, neighbours);
        }
    }

    private record Edge(IntegerCoordinate<int> A, IntegerCoordinate<int> B)
    {
        public static Edge From(IntegerCoordinate<int> a, IntegerCoordinate<int> b)
        {
            var first = a.GetHashCode() < b.GetHashCode() ? a : b;
            var second = first == a ? b : a;

            return new Edge(first, second);
        }
    }

    private record Region(char Value, HashSet<IntegerCoordinate<int>> Coordinates, HashSet<Edge> Neighbours)
    {
        public int Area => Coordinates.Count;
        public int Perimeter => Neighbours.Count;

        private HashSet<IntegerCoordinate<int>> FindOutsideNeighbours()
        {
            var outside = new HashSet<IntegerCoordinate<int>>();
            foreach (var edge in Neighbours)
                outside.Add(Coordinates.Contains(edge.A) ? edge.B : edge.A);

            return outside;
        }

        public IEnumerable<HashSet<Edge>> FindSides()
        {
            var sides = new List<HashSet<Edge>>();
            var visited = new HashSet<Edge>();
            var outside = FindOutsideNeighbours();

            foreach (var edge in Neighbours)
            {
                if (visited.Contains(edge))
                    continue;

                var side = new HashSet<Edge>();
                SearchSide(edge, outside, side);

                sides.Add(side);

                foreach (var point in side)
                    visited.Add(point);
            }

            return sides;
        }

        private void SearchSide(
            Edge edge,
            HashSet<IntegerCoordinate<int>> outside,
            HashSet<Edge> side
        )
        {
            if (!side.Add(edge))
                return;

            var edgeInside = Coordinates.Contains(edge.A) ? edge.A : edge.B;
            var edgeOutside = edge.A == edgeInside ? edge.B : edge.A;

            foreach (var direction in DirectionExtensions.Cardinals)
            {
                var outsideNeighbour = edgeOutside.Move(direction);
                var insideNeighbour = edgeInside.Move(direction);

                if (!Coordinates.Contains(insideNeighbour) || !outside.Contains(outsideNeighbour))
                    continue;

                SearchSide(Edge.From(outsideNeighbour, insideNeighbour), outside, side);
            }
        }
    }
}