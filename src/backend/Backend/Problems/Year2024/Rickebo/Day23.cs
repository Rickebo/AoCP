using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lib.Graphs;
using Lib.Text;

namespace Backend.Problems.Year2024.Rickebo;

public class Day23 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 23);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "LAN Party";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(
                Data.Parse(input).CountLanGroups("t", reporter)
            );

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(Data.Parse(input).Part2());

            return Task.CompletedTask;
        }
    }

    public record Data(Dictionary<string, Computer> Computers)
    {
        public IEnumerable<IReadOnlyCollection<Computer>> FindLoops(
            Computer source,
            int depth,
            Stack<Computer>? path = null
        )
        {
            path ??= new();
            if (depth < 0)
                yield break;

            if (path.Count > 0 && path.Last() == source)
            {
                yield return path;
                yield break;
            }

            try
            {
                path.Push(source);

                foreach (var neighbour in source.Neighbours)
                {
                    foreach (var subPath in FindLoops(neighbour, depth - 1, path))
                        yield return subPath;
                }
            }
            finally
            {
                if (path.Pop() != source)
                    throw new Exception();
            }
        }

        public int CountLanGroups(string prefix, Reporter? reporter = null)
        {
            var groups = new HashSet<LanGroup>();

            foreach (var src in Computers.Values)
            {
                foreach (var path in FindLoops(src, 3))
                {
                    if (path.Count != 3)
                        continue;

                    if (!path.Any(node => node.Name.StartsWith(prefix)))
                        continue;

                    if (!groups.Add(LanGroup.From(path)))
                        continue;
                }
            }

            foreach (var group in groups.OrderBy(g => g.A.Name))
                reporter?.ReportLine(string.Join(" -> ", group.Computers().Select(entry => entry.Name)));

            return groups.Count;
        }

        public string Part2()
        {
            HashSet<Computer>? group = null;
            foreach (var node in Computers.Values)
            {
                var currentBiggest = FindBiggestGroup(node);
                if (group == null || (currentBiggest != null && currentBiggest.Count >= group.Count))
                    group = currentBiggest;
            }

            if (group == null)
                throw new Exception("Failed to find biggest group");

            return string.Join(",", group.OrderBy(x => x.Name).Select(x => x.Name));
        }

        public HashSet<Computer>? FindBiggestGroup(Computer node)
        {
            var neighbours = node.Neighbours.ToArray();
            var max = 1 << neighbours.Length;
            HashSet<Computer>? group = null;

            var set = new HashSet<Computer>();
            var included = new HashSet<Computer>();

            for (var i = 0; i < max; i++)
            {
                set.Clear();
                included.Clear();
                
                set.UnionWith(node.Group);
                included.Add(node);

                for (var n = 0; n < neighbours.Length; n++)
                {
                    if (((1 << n) & i) != 0)
                    {
                        set.IntersectWith(neighbours[n].Group);
                        included.Add(neighbours[n]);
                    }
                }

                set.IntersectWith(included);

                if (group == null || group.Count < set.Count)
                    group = set.ToHashSet();
            }

            return group;
        }

        public HashSet<Computer>? FindGroup(IEnumerable<Computer> nodes)
        {
            HashSet<Computer>? set = null;
            foreach (var node in nodes)
            {
                if (set == null)
                    set = node.Group.ToHashSet();
                else
                {
                    foreach (var neighbour in node.Neighbours)
                        set.IntersectWith(neighbour.Group);
                }
            }

            return set;
        }

        public IEnumerable<Computer> Explore(Computer source, HashSet<Computer>? visited = null)
        {
            visited ??= [];

            var frontier = new Queue<Computer>([source]);

            while (frontier.TryDequeue(out var current))
            {
                if (!visited.Add(current))
                    continue;

                yield return current;

                foreach (var neighbour in current.Neighbours)
                    frontier.Enqueue(neighbour);
            }
        }

        public static Data Parse(string input)
        {
            var computers = new Dictionary<string, Computer>();
            foreach (var line in input.SplitLines())
            {
                var ends = line.Split('-');
                if (!computers.TryGetValue(ends[0], out var a))
                    computers[ends[0]] = a = new Computer(ends[0], []);
                if (!computers.TryGetValue(ends[1], out var b))
                    computers[ends[1]] = b = new Computer(ends[1], []);

                a.AddNeighbour(b);
                b.AddNeighbour(a);
            }

            return new Data(computers);
        }
    }

    public record LanGroup(Computer A, Computer B, Computer C)
    {
        public static LanGroup From(IEnumerable<Computer> computers)
        {
            var ordered = computers.OrderBy(comp => comp.Name).ToArray();
            if (ordered.Length != 3)
                throw new ArgumentException("Invalid number of computers for lan group.");

            return new LanGroup(ordered[0], ordered[1], ordered[2]);
        }

        public IEnumerable<Computer> Computers()
        {
            yield return A;
            yield return B;
            yield return C;
        }
    }

    public class Computer
    {
        public string Name { get; }
        public HashSet<Computer> Neighbours { get; }
        public HashSet<Computer> Group { get; }

        public Computer(string name, HashSet<Computer> neighbours)
        {
            Name = name;
            Neighbours = neighbours;
            Group = [.. neighbours];
            Group.Add(this);
        }

        public void AddNeighbour(Computer neighbour)
        {
            Neighbours.Add(neighbour);
            Group.Add(neighbour);
        }

        public override bool Equals(object? other) =>
            other is Computer otherComputer && Equals(otherComputer);

        protected bool Equals(Computer other) => Name == other.Name;
        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator ==(Computer left, Computer right) => Equals(left, right);

        public static bool operator !=(Computer left, Computer right) => !(left == right);
    }

    public record Connection(Computer From, Computer To) : IEdge<Computer>;
}

