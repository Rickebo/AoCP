using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day11 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 11);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Plutonian Pebbles";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create stone collection
            StoneCollection stoneCollection = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(stoneCollection.Blink(25)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create stone collection
            StoneCollection stoneCollection = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(stoneCollection.Blink(75)));
            return Task.CompletedTask;
        }
    }

    public class StoneCollection
    {
        private readonly Reporter _reporter;
        private Dictionary<long, long> _stones = [];
        public StoneCollection(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Retrieve stones from input
            foreach (long stone in Parser.GetValues<long>(input))
            {
                // Add stone to collection
                AddOrSet(_stones, stone, 1);
            }

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"Initial collection: {string.Join(" ", _stones.Keys)}\n"));
        }
        
        public long Blink(int blinks)
        {
            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine("Blink | Stone types | Stones"));

            // Blink
            for (int i = 0; i < blinks; i++)
            {
                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"{i, -5} | {_stones.Count, -11} | {_stones.Values.Sum()}"));

                // Create new collection to store stones after blink
                Dictionary<long, long> newStones = [];

                // Loop through last collection
                foreach ((long stone, long count) in _stones)
                {
                    if (stone == 0)
                    {
                        // Stones of type 0 turns into type 1
                        AddOrSet(newStones, 1, count);
                    }
                    else if (stone.ToString().Length % 2 == 0)
                    {
                        // Split stone into two new stones
                        string stoneStr = stone.ToString();
                        long stone1 = long.Parse(stoneStr[..(stoneStr.Length / 2)]);
                        long stone2 = long.Parse(stoneStr[(stoneStr.Length / 2)..]);

                        // Add stones to collection
                        AddOrSet(newStones, stone1, count);
                        AddOrSet(newStones, stone2, count);
                    }
                    else
                    {
                        // If no rule applies, multiply stone with 2024
                        long number = stone * 2024;

                        // Add to collection
                        AddOrSet(newStones, number, count);
                    }
                }

                // Replace old collection with new
                _stones = newStones;
            }

            // Send to frontend
            _reporter.Report(TextProblemUpdate.FromLine($"{blinks, -5} | {_stones.Count, -11} | {_stones.Values.Sum()}"));

            // Sum all values for each stone type
            return _stones.Values.Sum();
        }

        private static void AddOrSet(Dictionary<long, long> dict, long key, long val)
        {
            // Add if dictionary contains key otherwise set key = val
            if (dict.ContainsKey(key))
                dict[key] += val;
            else dict[key] = val;
        }

        public long StoneCount() => _stones.Values.Sum();
    }
}