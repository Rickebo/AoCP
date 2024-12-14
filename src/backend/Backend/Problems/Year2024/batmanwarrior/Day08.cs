using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day08 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 08);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Resonant Collinearity";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create city
            City city = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(city.CountAntinodes(harmonics: false)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create city
            City city = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(city.CountAntinodes(harmonics: true)));
            return Task.CompletedTask;
        }
    }

    public class City
    {
        private readonly Reporter _reporter;
        private readonly CharGrid _grid;
        private readonly Dictionary<char, List<IntegerCoordinate<int>>> _antennas = [];

        public City(string input, Reporter reporter)
        {
            // Save reporter for frontend printing
            _reporter = reporter;

            // Init grid
            _grid = new(input);

            // Get all antennas
            foreach (IntegerCoordinate<int> pos in _grid.FindAll(x => x != '.'))
            {
                // Check if more antennas with same freq exist
                if (_antennas.TryGetValue(_grid[pos], out List<IntegerCoordinate<int>>? value))
                {
                    // Add to existing freq group
                    value.Add(pos);
                }
                else
                {
                    // Create new freq group
                    _antennas[_grid[pos]] = [pos];
                }

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"Antenna with frequency '{_grid[pos]}' found at location [{pos.X},{pos.Y}]"));
            }

            // Send to frontend
            _reporter.Report(GlyphGridUpdate.FromCharGrid(_grid, "#FFFFFF", "#000000"));
        }

        public int CountAntinodes(bool harmonics)
        {
            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"\nCounting antinodes {(harmonics ? "with" : "without")} harmonics:"));

            // Keep track of unique antinode locations
            HashSet<IntegerCoordinate<int>> antinodes = [];

            // Check all antenna freq groups
            foreach (KeyValuePair<char, List<IntegerCoordinate<int>>> group in _antennas)
            {
                // Compare antennas in group
                for (int i = 0; i < group.Value.Count; i++)
                {
                    // Get source antenna
                    IntegerCoordinate<int> A = group.Value[i];

                    // Compare to other antennas in group
                    for (int j = 0; j < group.Value.Count; j++)
                    {
                        // Skip source antenna
                        if (j == i) continue;

                        // Retrieve pos of second antenna
                        IntegerCoordinate<int> B = group.Value[j];

                        // Calculate distance between antennas
                        Distance<int> dist = A.Distance(B);

                        // Place antinodes
                        if (!harmonics)
                        {
                            // Place antinode one distance from B
                            AddAntinode(antinodes, B.Move(dist));
                        }
                        else
                        {
                            // Place antinode on B
                            AddAntinode(antinodes, B);

                            // Add antinodes until outside of grid
                            IntegerCoordinate<int> C = B.Move(dist);
                            while (_grid.Contains(C))
                            {
                                // Place antinode on C
                                AddAntinode(antinodes, C);

                                // Update antinode position
                                C = C.Move(dist);
                            }
                        }
                    }
                }
            }

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"\nTotal antinodes found: {antinodes.Count}"));

            return antinodes.Count;
        }

        private void AddAntinode(HashSet<IntegerCoordinate<int>> antinodes, IntegerCoordinate<int> pos)
        {
            // Check if not previously added and within grid
            if (_grid.Contains(pos) && antinodes.Add(pos))
            {
                // Print antinode as red '#'
                _reporter.ReportGlyphGridUpdate(
                    builder => builder.WithEntry(
                        b => b
                            .WithCoordinate(pos)
                            .WithChar('#')
                            .WithForeground("#FF0000")
                            .WithBackground("#000000")
                    )
                );

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"Antinode found at location [{pos.X},{pos.Y}]"));
            }
        }
    }
}