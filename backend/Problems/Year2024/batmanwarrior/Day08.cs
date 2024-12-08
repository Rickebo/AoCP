using Common;
using Common.Updates;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day08 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 08, 0, 0, 0);

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

            // Check for antinodes
            city.CheckAntinodes(harmonics: false);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(city.CountAntinodes()));
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

            // Check for antinodes
            city.CheckAntinodes(harmonics: true);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(city.CountAntinodes()));
            return Task.CompletedTask;
        }
    }

    public class City
    {
        private readonly CharGrid _grid;
        private readonly HashSet<IntegerCoordinate<int>> _antinodes = [];
        private readonly Reporter _reporter;
        private readonly List<AntennaGroup> _antennas = [];

        public City(string input, Reporter reporter)
        {
            // Save reporter for frontend printing
            _reporter = reporter;

            // Init grid
            _grid = new(input);

            // Scan for antennas
            ScanForAntennas();
            
            // Paint grid with antennas
            _reporter.ReportStringGridUpdate(
                _grid,
                (builder, coordinate, val) => builder
                    .WithCoordinate(coordinate)
                    .WithText(ColorCell(coordinate))
            );
        }

        private void ScanForAntennas()
        {
            // Check all locations
            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    // Get object at pos x y
                    char c = _grid[x, y];

                    // Check if not empty
                    if (c != '.')
                    {
                        // Search antenna group list
                        bool exists = false;
                        foreach (AntennaGroup group in _antennas)
                        {
                            // Group for that antenna exists
                            if (group.Type == c)
                            {
                                // Add antenna
                                group.AddAntenna(new(x, y));
                                exists = true;
                                break;
                            }
                        }

                        // Antenna group does not exist (yet)
                        if (!exists)
                        {
                            // Create group and add antenna
                            AntennaGroup group = new(c, _reporter);
                            group.AddAntenna(new(x, y));
                            _antennas.Add(group);
                        }
                    }
                }
            }
        }

        public void CheckAntinodes(bool harmonics)
        {
            // Check all antenna groups
            foreach (AntennaGroup group in _antennas)
            {
                // Skip groups with single or no antenna
                if (group.Positions.Count <= 1) continue;

                // Compare antennas in group
                for (int i = 0; i < group.Positions.Count; i++)
                {
                    // Get source antenna
                    IntegerCoordinate<int> A = group.Positions[i];

                    // Compare to other antennas in group
                    for (int j = 0; j < group.Positions.Count; j++)
                    {
                        // Skip source antenna
                        if (j == i) continue;

                        // Retrieve pos of antenna
                        IntegerCoordinate<int> B = group.Positions[j];

                        // Calculate distance between antennas
                        Distance<int> dist = A.Distance(B);

                        // Place antinodes
                        if (!harmonics)
                        {
                            // Place antinode one distance from B
                            AddAntinode(B.Move(dist));
                        }
                        else
                        {
                            // Place antinode on B
                            AddAntinode(B);

                            // Add antinodes until outside of grid
                            IntegerCoordinate<int> C = B.Move(dist);
                            while (_grid.Contains(C))
                            {
                                // Place antinode on C
                                AddAntinode(C);

                                // Update antinode position
                                C = C.Move(dist);
                            }
                        }
                    }
                }
            }
        }

        private void AddAntinode(IntegerCoordinate<int> pos)
        {
            // Check if not previously added and within grid
            if (_grid.Contains(pos) && _antinodes.Add(pos))
            {
                // Only print when needed
                _reporter.ReportStringGridUpdate(pos, "#66FF66");

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"New antinode at pos [{pos.X},{pos.Y}]"));
            }
        }

        public int CountAntinodes() => _antinodes.Count;

        private string ColorCell(IntegerCoordinate<int> coordinate)
        {
            // Coloring city grid
            return _grid[coordinate] switch
            {
                '.' => "#B3B3B3", // Nothing
                _ => "#0066FF" // Antenna
            };
        }
    }

    public class AntennaGroup(char type, Reporter reporter)
    {
        private readonly Reporter _reporter = reporter;
        public char Type = type;
        public List<IntegerCoordinate<int>> Positions = [];

        public void AddAntenna(IntegerCoordinate<int> pos)
        {
            // Check if antenna is added already
            if (!Positions.Contains(pos))
            {
                // Add antenna
                Positions.Add(pos);

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"Group '{Type}': New antenna at pos [{pos.X},{pos.Y}]"));
            }
        }
    }
}