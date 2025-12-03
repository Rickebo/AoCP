using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day21 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 21);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Keypad Conundrum";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create starship
            Starship starship = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(starship.Complexitites(robots: 2)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create starship
            Starship starship = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(starship.Complexitites(robots: 25)));
            return Task.CompletedTask;
        }
    }

    public class Starship
    {
        private readonly Reporter _reporter;
        private readonly List<string> _codes = [];

        public Starship(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Retrieve door codes
            foreach (string row in input.SplitLines())
                _codes.Add(row);
        }

        public long Complexitites(int robots)
        {
            // Create cache
            Dictionary<(string, int), long> cache = [];

            // Accumulate the complexity for all door codes
            long totalComplexity = 0;
            foreach (string code in _codes)
            {
                // Calculate the complexity for each code
                char prev = 'A';
                long codeSequenceLength = 0;
                foreach (char c in code)
                {
                    // Accumulate the shortest sequence length for each button in code
                    long shortestLength = Int64.MaxValue;
                    foreach (string seq in NumpadLookup(prev, c))
                    {
                        // Get length
                        long length = SequenceLength(seq, robots, cache);

                        // If this length is the shortest
                        if (length < shortestLength)
                            shortestLength = length;
                    }

                    // Add to code sequence length and update prev for next button
                    codeSequenceLength += shortestLength;
                    prev = c;
                }

                // Retrieve the numeric val of code
                long codeNumericVal = long.Parse(code[.. (code.Length - 1)]);

                // Add code complexity to total
                totalComplexity += codeSequenceLength * codeNumericVal;
            }

            // Return the total complexity of all door codes
            return totalComplexity;
        }

        private static long SequenceLength(string sequence, int robotDepth, Dictionary<(string, int), long> cache)
        {
            // If robot depth reached the human operator
            if (robotDepth == 0)
                return sequence.Length;

            // If cache already has the sequence length for this sequence at this robot depth
            if (cache.ContainsKey((sequence, robotDepth)))
                return cache[(sequence, robotDepth)];

            // Check sequence length of all buttons in sequence up to the human operator
            char prev = 'A';
            long totalLength = 0;
            foreach (char c in sequence)
            {
                // Accumulate the shortest sequence length
                long shortestLength  = Int64.MaxValue;
                foreach (string seq in DirpadLookup(prev, c))
                {
                    // Get length
                    long length = SequenceLength(seq, robotDepth - 1, cache);

                    // If this length is the shortest
                    if (length < shortestLength)
                        shortestLength = length;
                }

                // Add to total sequence length and update prev for next move
                totalLength += shortestLength;
                prev = c;
            }

            // Store the sequence length for this sequence at this robot depth for later
            cache[(sequence, robotDepth)] = totalLength;

            // Return total sequence length
            return totalLength;
        }

        private static string[] NumpadLookup(char from, char to)
        {
            // Wonderful lookup table of DOOM^2
            return from switch
            {
                'A' => to switch
                {
                    'A' => ["A"],
                    '0' => ["<A"],
                    '1' => ["^<<A"],
                    '2' => ["<^A", "^<A"],
                    '3' => ["^A"],
                    '4' => ["^^<<A"],
                    '5' => ["<^^A", "^^<A"],
                    '6' => ["^^A"],
                    '7' => ["^^^<<A"],
                    '8' => ["<^^^A", "^^^<A"],
                    '9' => ["^^^A"],
                    _ => throw new NotImplementedException(),
                },
                '0' => to switch
                {
                    'A' => [">A"],
                    '0' => ["A"],
                    '1' => ["^<A"],
                    '2' => ["^A"],
                    '3' => [">^A", "^>A"],
                    '4' => ["^^<A"],
                    '5' => ["^^A"],
                    '6' => [">^^A", "^^>A"],
                    '7' => ["^^^<A"],
                    '8' => ["^^^A"],
                    '9' => [">^^^A", "^^^>A"],
                    _ => throw new NotImplementedException(),
                },
                '1' => to switch
                {
                    'A' => [">>vA"],
                    '0' => [">vA"],
                    '1' => ["A"],
                    '2' => [">A"],
                    '3' => [">>A"],
                    '4' => ["^A"],
                    '5' => [">^A", "^>A"],
                    '6' => [">>^A", "^>>A"],
                    '7' => ["^^A"],
                    '8' => [">^^A", "^^>A"],
                    '9' => [">>^^A", "^^>>A"],
                    _ => throw new NotImplementedException(),
                },
                '2' => to switch
                {
                    'A' => [">vA"],
                    '0' => ["vA"],
                    '1' => ["<A"],
                    '2' => ["A"],
                    '3' => [">A"],
                    '4' => ["<^A", "^<A"],
                    '5' => ["^A"],
                    '6' => [">^A", "^>A"],
                    '7' => ["<^^A", "^^<A"],
                    '8' => ["^^A"],
                    '9' => [">^^A", "^^>A"],
                    _ => throw new NotImplementedException(),
                },
                '3' => to switch
                {
                    'A' => ["vA"],
                    '0' => ["<vA", "v<A"],
                    '1' => ["<<A"],
                    '2' => ["<A"],
                    '3' => ["A"],
                    '4' => ["<<^A", "^<<A"],
                    '5' => ["<^A", "^<A"],
                    '6' => ["^A"],
                    '7' => ["<<^^A", "^^<<A"],
                    '8' => ["<^^A", "^^<A"],
                    '9' => ["^^A"],
                    _ => throw new NotImplementedException(),
                },
                '4' => to switch
                {
                    'A' => [">>vvA"],
                    '0' => [">vvA"],
                    '1' => ["vA"],
                    '2' => [">vA", "v>A"],
                    '3' => [">>vA", "v>>A"],
                    '4' => ["A"],
                    '5' => [">A"],
                    '6' => [">>A"],
                    '7' => ["^A"],
                    '8' => [">^A", "^>A"],
                    '9' => [">>^A", "^>>A"],
                    _ => throw new NotImplementedException(),
                },
                '5' => to switch
                {
                    'A' => [">vvA", "vv>A"],
                    '0' => ["vvA"],
                    '1' => ["<vA", "v<A"],
                    '2' => ["vA"],
                    '3' => [">vA", "v>A"],
                    '4' => ["<A"],
                    '5' => ["A"],
                    '6' => [">A"],
                    '7' => ["<^A", "^<A"],
                    '8' => ["^A"],
                    '9' => [">^A", "^>A"],
                    _ => throw new NotImplementedException(),
                },
                '6' => to switch
                {
                    'A' => ["vvA"],
                    '0' => ["<vvA", "vv<A"],
                    '1' => ["<<vA", "v<<A"],
                    '2' => ["<vA", "v<A"],
                    '3' => ["vA"],
                    '4' => ["<<A"],
                    '5' => ["<A"],
                    '6' => ["A"],
                    '7' => ["<<^A", "^<<A"],
                    '8' => ["<^A", "^<A"],
                    '9' => ["^A"],
                    _ => throw new NotImplementedException(),
                },
                '7' => to switch
                {
                    'A' => [">>vvvA"],
                    '0' => [">vvvA"],
                    '1' => ["vvA"],
                    '2' => [">vvA", "vv>A"],
                    '3' => [">>vvA", "vv>>A"],
                    '4' => ["vA"],
                    '5' => [">vA", "v>A"],
                    '6' => [">>vA", "v>>A"],
                    '7' => ["A"],
                    '8' => [">A"],
                    '9' => [">>A"],
                    _ => throw new NotImplementedException(),
                },
                '8' => to switch
                {
                    'A' => [">vvvA", "vvv>A"],
                    '0' => ["vvvA"],
                    '1' => ["<vvA", "vv<A"],
                    '2' => ["vvA"],
                    '3' => [">vvA", "vv>A"],
                    '4' => ["<vA", "v<A"],
                    '5' => ["vA"],
                    '6' => [">vA", "v>A"],
                    '7' => ["<A"],
                    '8' => ["A"],
                    '9' => [">A"],
                    _ => throw new NotImplementedException(),
                },
                '9' => to switch
                {
                    'A' => ["vvvA"],
                    '0' => ["<vvvA", "vvv<A"],
                    '1' => ["<<vvA", "vv<<A"],
                    '2' => ["<vvA", "vv<A"],
                    '3' => ["vvA"],
                    '4' => ["<<vA", "v<<A"],
                    '5' => ["<vA", "v<A"],
                    '6' => ["vA"],
                    '7' => ["<<A"],
                    '8' => ["<A"],
                    '9' => ["A"],
                    _ => throw new NotImplementedException(),
                },
                _ => throw new NotImplementedException(),
            };
        }

        private static string[] DirpadLookup(char from, char to)
        {
            // Wonderful lookup table of DOOM
            return from switch
            {
                'A' => to switch
                {
                    'A' => ["A"],
                    '^' => ["<A"],
                    '>' => ["vA"],
                    'v' => ["<vA", "v<A"],
                    '<' => ["v<<A"],
                    _ => throw new NotImplementedException(),
                },
                '^' => to switch
                {
                    'A' => [">A"],
                    '^' => ["A"],
                    '>' => [">vA", "v>A"],
                    'v' => ["vA"],
                    '<' => ["v<A"],
                    _ => throw new NotImplementedException(),
                },
                '>' => to switch
                {
                    'A' => ["^A"],
                    '^' => ["<^A", "^<A"],
                    '>' => ["A"],
                    'v' => ["<A"],
                    '<' => ["<<A"],
                    _ => throw new NotImplementedException(),
                },
                'v' => to switch
                {
                    'A' => [">^A", "^>A"],
                    '^' => ["^A"],
                    '>' => [">A"],
                    'v' => ["A"],
                    '<' => ["<A"],
                    _ => throw new NotImplementedException(),
                },
                '<' => to switch
                {
                    'A' => [">>^A"],
                    '^' => [">^A"],
                    '>' => [">>A"],
                    'v' => [">A"],
                    '<' => ["A"],
                    _ => throw new NotImplementedException(),
                },
                _ => throw new NotImplementedException(),
            };
        }
    }
}

