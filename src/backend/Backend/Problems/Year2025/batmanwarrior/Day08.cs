using Common;
using Lib.Geometry;
using Lib.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day08 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 08);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Playground";

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
        private readonly List<Coordinate3D<long>> _junctions = [];

        public Solver(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            foreach (var line in input.SplitLines())
            {
                long[] vals = Parser.GetValues<long>(line);
                _junctions.Add(new(vals[0], vals[1], vals[2]));
            }
        }

        public long PartOne()
        {
            // Get list of sorted junction distances
            List<(Coordinate3D<long>, Coordinate3D<long>, long)> distances = Distances(_junctions);

            // Create connections between junctions
            Dictionary<Coordinate3D<long>, HashSet<Coordinate3D<long>>> connections = [];
            foreach (var junction in _junctions)
                connections.Add(junction, []);

            // Connect the shortest 1000 junction pairs
            for (int i = 0; i < 1000; i++)
            {
                if (i >= distances.Count)
                    throw new ProblemException("Not enough junction pairs.");

                Coordinate3D<long> A = distances[i].Item1;
                Coordinate3D<long> B = distances[i].Item2;
                connections[A].Add(B);
                connections[B].Add(A);
            }

            // Create circuits
            List<long> circuits = [.. Circuits(connections).OrderByDescending(x => x)];
            if (circuits.Count < 3)
                throw new ProblemException("Not enough circuits.");

            return circuits[0] * circuits[1] * circuits[2];
        }

        public long PartTwo()
        {
            // Get list of sorted junction distances
            List<(Coordinate3D<long>, Coordinate3D<long>, long)> distances = Distances(_junctions);

            // Create connections between junctions
            Dictionary<Coordinate3D<long>, HashSet<Coordinate3D<long>>> connections = [];
            foreach (var junction in _junctions)
                connections.Add(junction, []);

            // Connect junction pairs until they are all in one circuit
            for (int i = 0; i < distances.Count; i++)
            {
                Coordinate3D<long> A = distances[i].Item1;
                Coordinate3D<long> B = distances[i].Item2;
                connections[A].Add(B);
                connections[B].Add(A);

                // Create circuits brute force style
                if (Circuits(connections).Count == 1)
                    return A.X * B.X;
            }

            throw new ProblemException("No single circuit created.");
        }

        private static List<(Coordinate3D<long>, Coordinate3D<long>, long)> Distances(List<Coordinate3D<long>> junctions)
        {
            List<(Coordinate3D<long>, Coordinate3D<long>, long)> distances = [];
            for (int i = 0; i < junctions.Count - 1; i++)
            {
                for (int j = i + 1; j < junctions.Count; j++)
                {
                    long distance = (junctions[i].X - junctions[j].X) * (junctions[i].X - junctions[j].X);
                    distance += (junctions[i].Y - junctions[j].Y) * (junctions[i].Y - junctions[j].Y);
                    distance += (junctions[i].Z - junctions[j].Z) * (junctions[i].Z - junctions[j].Z);
                    distances.Add((junctions[i], junctions[j], distance));
                }
            }

            return [.. distances.OrderBy(x => x.Item3)];
        }

        private static List<long> Circuits(Dictionary<Coordinate3D<long>, HashSet<Coordinate3D<long>>> connections)
        {
            List<long> circuits = [];
            HashSet<Coordinate3D<long>> visited = [];
            foreach (var coord in connections.Keys)
            {
                // Create coord queue
                Queue<Coordinate3D<long>> queue = new();
                queue.Enqueue(coord);

                // Create circuit from all connected coords
                int count = 0;
                while (queue.TryDequeue(out var curr))
                {
                    // Skip visited
                    if (!visited.Add(curr))
                        continue;

                    count++;

                    // Queue connected
                    foreach (var connectedCoord in connections[curr])
                        queue.Enqueue(connectedCoord);
                }

                if (count > 0)
                    circuits.Add(count);
            }

            return circuits;
        }
    }
}

