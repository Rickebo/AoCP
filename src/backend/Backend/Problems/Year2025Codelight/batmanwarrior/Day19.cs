using Common;
using Common.Updates;
using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025Codelight.batmanwarrior;
public class Day19 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 19);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Trio at the Bistro";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create bistro
            Bistro bistro = new(input, reporter);

            // Count minutes with trios
            int trioMinutes = bistro.GetTrioMinutes();

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(trioMinutes.ToString()));
            return Task.CompletedTask;
        }

        private class Bistro
        {
            private readonly Reporter _reporter;
            private readonly SortedDictionary<int, List<(string, string)>> _log = [];
            private readonly Dictionary<string, int> _present = [];

            public Bistro(string input, Reporter reporter)
            {
                // Save for printing
                _reporter = reporter;

                // Add log entries to sorted dictionary
                foreach (string line in input.SplitLines())
                {
                    string[] args = line.Split(' ');
                    int timeValue = TimeToValue(args[0]);
                    if (_log.TryGetValue(timeValue, out List<(string, string)>? value))
                        value.Add((args[1], args[2]));
                    else
                        _log[timeValue] = [(args[1], args[2])];
                }

                // Print sorted log
                foreach (var pair in _log)
                {
                    string timeStampStr = ValueToTime(pair.Key);
                    for (int i = 0; i < pair.Value.Count; i++)
                        _reporter.ReportLine($"{(i == 0 ? timeStampStr : "     ")} {pair.Value[i].Item1} {pair.Value[i].Item2}");
                }
            }

            public int GetTrioMinutes()
            {
                int minutes = 0;
                int? lastTime = null;
                Dictionary<string, int> entering = [];
                Dictionary<string, int> leaving = [];
                foreach (KeyValuePair<int, List<(string, string)>> logPair in _log)
                {
                    // Handle gaps in log
                    if (lastTime.HasValue)
                    {
                        int elapsed = logPair.Key - lastTime.Value;
                        if (elapsed > 1 && _present.Values.Sum() == 3)
                            minutes += elapsed - 1;
                    }
                    lastTime = logPair.Key;

                    // Check all entries for this minute
                    entering.Clear();
                    leaving.Clear();
                    foreach (var entry in logPair.Value)
                    {
                        // Retrieve entering and leaving persons
                        if (entry.Item2 == "IN")
                            entering.Increment(entry.Item1);
                        else if (entry.Item2 == "OUT")
                            leaving.Increment(entry.Item1);
                    }

                    // Process
                    ProcessLogEntry(entering, leaving);

                    // If three persons are present
                    if (_present.Values.Sum() == 3)
                        minutes++;
                }
                return minutes;
            }

            private void ProcessLogEntry(Dictionary<string, int> entering, Dictionary<string, int> leaving)
            {
                // Process entering persons first
                foreach (KeyValuePair<string, int> pair in entering)
                    _present.AddOrUpdate(pair.Key, pair.Value);

                // Process leaving persons
                foreach (KeyValuePair<string, int> pair in leaving)
                    _present.AddOrUpdate(pair.Key, -pair.Value, min: 0);
            }

            private static int TimeToValue(string time)
            {
                string[] args = time.Split(':');
                return int.Parse(args[0]) * 60 + int.Parse(args[1]);
            }

            private static string ValueToTime(int value)
                => $"{(value / 60).ToString().PadLeft(2, '0')}:{(value % 60).ToString().PadLeft(2, '0')}";
        }
    }
}