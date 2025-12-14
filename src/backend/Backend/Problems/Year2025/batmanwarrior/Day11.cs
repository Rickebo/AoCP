using Common;
using Lib.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day11 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 11);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Reactor";

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
        private readonly Dictionary<string, List<string>> _devices = [];

        public Solver(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            foreach (var line in input.SplitLines())
            {
                string[] args = line.Split();
                _devices.Add(args[0][..^1], [.. args.Skip(1)]);
            }
        }

        public long PartOne()
        {
            // Add start device
            Queue<string> queue = new();
            queue.Enqueue("you");

            long paths = 0;
            while (queue.TryDequeue(out var currLabel))
            {
                // Target device reached
                if (currLabel == "out")
                {
                    paths++;
                    continue;
                }

                // Queue up all outputs
                if (_devices.TryGetValue(currLabel, out var outputs))
                    foreach (var outputLabel in outputs)
                        queue.Enqueue(outputLabel);
            }

            return paths;
        }

        public long PartTwo() =>
            PathFinder(new("svr", [], false, false), "out", []);

        private long PathFinder(
            Path path,
            string target,
            Dictionary<Path, long> cache)
        {
            bool dac = path.DAC;
            bool fft = path.FFT;

            // Check if path is cached
            if (cache.TryGetValue(path, out var validPaths))
                return validPaths;

            // Visiting DAC
            else if (path.Label == "dac")
                dac = true;

            // Visiting FFT
            else if (path.Label == "fft")
                fft = true;

            // Target reached
            else if (path.Label == target)
                return dac && fft ? 1 : 0;

            // Invalidate loops
            if (!path.Visited.Add(path.Label))
                return 0;

            // Valid device output paths
            validPaths = 0;
            foreach (var outputLabel in _devices[path.Label])
                validPaths += PathFinder(new(outputLabel, path.Visited, dac, fft), target, cache);

            // Remove this device from visited and cache result
            path.Visited.Remove(path.Label);
            cache[path] = validPaths;
            return validPaths;
        }
    }

    private record Path(string Label, HashSet<string> Visited, bool DAC, bool FFT);
}

