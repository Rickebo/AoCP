using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Text;

namespace Backend.Problems.Year2025Codelight.Rickebo;

public class Day19 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 19);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Trio at the Bistro";

    private static bool IsAllowed(char ch) => ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z';


    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";


        public override Task Solve(string input, Reporter reporter)
        {

            var regex = new Regex("^(?<h>\\d{2}):(?<m>\\d{2})\\s+(?<person>[A-Za-z]+)\\s+(?<way>IN|OUT)$");
            var events = new Dictionary<TimeStamp, List<PersonEvent>>();

            foreach (var line in input.SplitLines())
            {
                var match = regex.Match(line);
                var h = int.Parse(match.Groups["h"].Value);
                var m = int.Parse(match.Groups["m"].Value);

                var name = match.Groups["person"].Value;
                var way = match.Groups["way"].Value;

                var ts = new TimeStamp(h, m);
                var e = new PersonEvent(name, way);

                if (!events.TryGetValue(ts, out var es))
                    es = events[ts] = [];
                
                es.Add(e);
            }

            var ordered = events.Keys.OrderBy(x => x);
            var inside = new HashSet<string>();
            var count = 0;

            TimeStamp? start = null;
            
            foreach (var ts in ordered)
            {
                var es = events[ts];
                var was = inside.Count;
                var startNames = string.Join(", ", inside);

                foreach (var e in es.Where(ex => ex.Way == "IN"))
                    inside.Add(e.Person);

                foreach (var e in es.Where(ex => ex.Way == "OUT"))
                    inside.Remove(e.Person);

                var isNow = inside.Count;

                if (isNow == 3 && was != 3)
                    start = ts;
                else if (isNow != 3 && was == 3)
                {
                    if (start is not null)
                    {
                        var duration = ts.TimeBetween(start);
                        reporter.ReportLine($"{start}-{ts} -> {startNames} = {duration} minutes");
                        count += duration;
                    }
                }
                     
            }

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Solution = count.ToString(),
                }
            );

            return Task.CompletedTask;
        }

        private record Event(string Mode);
        
        private record PersonEvent(string Person, string Way);

        private record TimeStamp(int Hour, int Minute) : IComparable<TimeStamp>
        {
            public int CompareTo(TimeStamp? other)
            {
                if (ReferenceEquals(this, other)) 
                    return 0;
                if (other is null) 
                    return 1;
                
                var hourComparison = Hour.CompareTo(other.Hour);
                
                if (hourComparison != 0) 
                    return hourComparison;
                
                return Minute.CompareTo(other.Minute);
            }

            public int TimeBetween(TimeStamp other)
            {
                var thisMinutes = Hour * 60 + Minute;
                var otherMinutes = other.Hour * 60 + other.Minute;

                return Math.Abs(thisMinutes - otherMinutes);
            }

            public override string ToString() => $"{Hour.ToString().PadLeft(2, '0')}:{Minute.ToString().PadLeft(2, '0')}";
        }
    }
}

