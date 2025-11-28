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
            reporter.ReportSolution(CalculateCost(new CharGrid(input).Flip(Axis.Y), false, reporter));

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(CalculateCost(new CharGrid(input).Flip(Axis.Y), true, reporter));
            return Task.CompletedTask;
        }
    }

    private static int CalculateCost(CharGrid grid, bool part2, Reporter reporter)
    {
        var regions = FindRegions(grid);
        return regions.Sum(
            region =>
            {
                var multiplicand = part2 ? region.FindSides().Count() : region.Perimeter;
                var sum = region.Area * multiplicand;
#if DEBUG
                reporter.ReportLine($"{region.Value}: {region.Area} * {multiplicand} = {sum}");
#endif
                return sum;
            }
        );
    }

    private static List<Region> FindRegions(CharGrid grid)
    {
        var regions = new List<Region>();
        var coordinates = new HashSet<IntegerCoordinate<int>>(grid.Coordinates);

        while (coordinates.Count > 0)
        {
            var coordinate = coordinates.Last();
            coordinates.Remove(coordinate);

            var region = new HashSet<IntegerCoordinate<int>>();
            var regionNeighbours = new HashSet<Edge>();
            SearchRegion(grid, coordinate, region, regionNeighbours, coordinates);
            regions.Add(new Region(grid[coordinate], region, regionNeighbours));
        }

        return regions;
    }

    private static void SearchRegion(
        CharGrid grid,
        IntegerCoordinate<int> source,
        HashSet<IntegerCoordinate<int>> region,
        HashSet<Edge> neighbours,
        HashSet<IntegerCoordinate<int>> remove
    )
    {
        if (!region.Add(source))
            return;

        remove.Remove(source);

        var srcType = grid[source];
        foreach (var neighbour in source.Neighbours)
        {
            if (!grid.Contains(neighbour) || grid[neighbour] != srcType)
                neighbours.Add(Edge.From(source, neighbour));
            else
                SearchRegion(grid, neighbour, region, neighbours, remove);
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
            var outside = FindOutsideNeighbours();

            var remaining = new HashSet<Edge>(Neighbours);
            while (remaining.Count > 0)
            {
                var edge = remaining.First();
                remaining.Remove(edge);

                var side = new HashSet<Edge>();
                SearchSide(edge, outside, side, remaining);

                yield return side;
            }
        }

        private void SearchSide(
            Edge edge,
            HashSet<IntegerCoordinate<int>> outside,
            HashSet<Edge> side,
            HashSet<Edge> remove
        )
        {
            if (!side.Add(edge))
                return;

            remove.Remove(edge);

            var edgeInside = Coordinates.Contains(edge.A) ? edge.A : edge.B;
            var edgeOutside = edge.A == edgeInside ? edge.B : edge.A;

            foreach (var direction in DirectionExtensions.Cardinals)
            {
                var outsideNeighbour = edgeOutside.Move(direction);
                var insideNeighbour = edgeInside.Move(direction);

                if (!Coordinates.Contains(insideNeighbour) || !outside.Contains(outsideNeighbour))
                    continue;

                SearchSide(Edge.From(outsideNeighbour, insideNeighbour), outside, side, remove);
            }
        }
    }
}