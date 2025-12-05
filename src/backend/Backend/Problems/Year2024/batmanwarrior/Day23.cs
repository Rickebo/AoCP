using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Text;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day23 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 23);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "LAN Party";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create LAN party
            LANParty lanParty = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(lanParty.SetsOfThreeWithChief()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create LAN party
            LANParty lanParty = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(lanParty.Password()));
            return Task.CompletedTask;
        }
    }

    public class LANParty
    {
        private record ConnectionSet(string One, string Two, string Three);
        private readonly Reporter _reporter;
        private readonly Dictionary<string, List<string>> _connections = [];

        public LANParty(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Retrieve computer connections
            foreach (string[] comps in input.SplitLines().Select(x => x.Split('-')))
            {
                // Get computers in connection
                string first = comps[0];
                string second = comps[1];

                // Add first
                if (_connections.TryGetValue(first, out List<string>? firstVals))
                {
                    if (!firstVals.Contains(second))
                        firstVals.Add(second);
                }
                else
                {
                    _connections[first] = [second];
                }

                // Add second
                if (_connections.TryGetValue(second, out List<string>? secondVals))
                {
                    if (!secondVals.Contains(first))
                        secondVals.Add(first);
                }
                else
                {
                    _connections[second] = [first];
                }
            }
        }

        public long SetsOfThreeWithChief()
        {
            // Save unique LAN sets
            HashSet<ConnectionSet> uniqueSets = [];
            foreach (var pair in _connections)
            {
                // Only look at chief LAN parties
                if (!pair.Key.StartsWith('t'))
                    continue;

                // Find connection of three computers
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    for (int j = 0; j < pair.Value.Count; j++)
                    {
                        // Ignore connection with self
                        if (j == i)
                            continue;

                        // If connection of three is found
                        if (_connections[pair.Value[j]].Contains(pair.Key) && _connections[pair.Value[j]].Contains(pair.Value[i]))
                        {
                            // Create list of computers and sort them
                            List<string> sortedSet = [pair.Key, pair.Value[i], pair.Value[j]];
                            sortedSet.Sort();

                            // Add to unique sets
                            uniqueSets.Add(new(sortedSet[0], sortedSet[1], sortedSet[2]));
                        }
                    }
                }
            }

            return uniqueSets.Count;
        }

        public string Password()
        {
            return "korv";
        }
    }
}

