using Common;
using Lib.Math;
using Lib.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day10 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 10);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Factory";

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
        private readonly List<(string LightDiagram, List<int[]> Buttons, int[] JoltageReq)> _schematics = [];

        public Solver(string input, Reporter reporter, int _)
        {
            // Save for printing
            _reporter = reporter;

            // Parse input
            foreach (var line in input.SplitLines())
            {
                string lightDiagram = string.Empty;
                List<int[]> buttons = [];
                int[] joltageReq = [];
                foreach (var arg in line.Split())
                {
                    if (arg.StartsWith('['))
                        lightDiagram = arg[1..^1];
                    else if (arg.StartsWith('('))
                        buttons.Add(Parser.GetValues<int>(arg));
                    else if (arg.StartsWith('{'))
                        joltageReq = Parser.GetValues<int>(arg);
                }
                _schematics.Add((lightDiagram, buttons, joltageReq));
            }
        }

        public int PartOne()
        {
            int totalPresses = 0;

            foreach (var (LightDiagram, Buttons, _) in _schematics)
            {
                // Setup
                PriorityQueue<string, int> queue = new();
                Dictionary<string, int> cache = [];

                // Create first state (all lights off)
                string firstState = new('.', LightDiagram.Length);
                queue.Enqueue(firstState, 0);
                cache[firstState] = 0;

                while (queue.TryDequeue(out var state, out var presses))
                {
                    // Skip outdated states
                    if (!cache.TryGetValue(state, out var cachedPresses) || presses > cachedPresses)
                        continue;

                    // Check if solved
                    if (state == LightDiagram)
                    {
                        totalPresses += presses;
                        break;
                    }

                    // Press all buttons
                    foreach (var button in Buttons)
                    {
                        // Create new state
                        char[] newState = state.ToCharArray();
                        foreach (var index in button)
                            newState[index] = newState[index] == '#' ? '.' : '#';
                        string stateStr = new(newState);
                        int statePresses = presses + 1;

                        // Enqueue if better than cached
                        if (!cache.TryGetValue(stateStr, out cachedPresses) || statePresses < cachedPresses)
                        {
                            // Update cache
                            cache[stateStr] = statePresses;

                            // Queue up new state
                            queue.Enqueue(stateStr, statePresses);
                        }
                    }
                }
            }

            return totalPresses;
        }

        public int PartTwo()
        {
            int totalPresses = 0;

            foreach (var (_, Buttons, JoltageReq) in _schematics)
            {
                // Add buttons affecting joltage level indices
                List<int>[] buttonsAffectingJoltage = new List<int>[JoltageReq.Length];
                for (int i = 0; i < JoltageReq.Length; i++)
                    buttonsAffectingJoltage[i] = [];
                for (int i = 0; i < Buttons.Count; i++)
                {
                    foreach (var index in Buttons[i])
                    {
                        // Make sure index is valid
                        if (index < 0 || index >= JoltageReq.Length)
                            throw new ProblemException($"Button '{i}' has invalid joltage index '{index}'.");

                        // Add button
                        buttonsAffectingJoltage[index].Add(i);
                    }
                }

                // Guard against unsolvable input
                for (int i = 0; i < JoltageReq.Length; i++)
                {
                    if (JoltageReq[i] > 0 && buttonsAffectingJoltage[i].Count == 0)
                        throw new ProblemException($"No buttons affecting joltage index {i}.");
                    if (JoltageReq[i] < 0)
                        throw new ProblemException($"Joltage index {i} is negative.");
                }

                // Create ILP
                IntegerLinearProgram ilp = new();

                // Create button variables
                int[] variableIndices = new int[Buttons.Count];
                for (int b = 0; b < Buttons.Count; b++)
                {
                    // Guard against empty buttons
                    int[] indices = Buttons[b];
                    if (indices.Length == 0)
                        throw new ProblemException("Empty button.");

                    // Find button upper bound
                    int ub = int.MaxValue;
                    foreach (int index in indices)
                        ub = Math.Min(ub, JoltageReq[index]);

                    variableIndices[b] = ilp.AddVariable(lowerBound: 0, upperBound: ub, name: $"x_{b}");
                }

                // Create joltage level constraint terms
                for (int i = 0; i < JoltageReq.Length; i++)
                {
                    List<(int variableIndex, long coefficient)> terms = [];
                    foreach (var b in buttonsAffectingJoltage[i])
                        terms.Add((variableIndices[b], 1));
                    ilp.AddConstraint(terms, IntegerLinearConstraintRelation.Equal, JoltageReq[i]);
                }

                // Create objective terms
                List<(int variableIndex, long coefficient)> objectiveTerms = [];
                for (int i = 0; i < variableIndices.Length; i++)
                    objectiveTerms.Add((variableIndices[i], 1));
                ilp.Minimize(objectiveTerms);

                // Solve
                var solution = ilp.Solve(new IntegerLinearProgramOptions(SearchWorkers: 8));

                // Create button and joltage req strings
                string[] buttonStr = new string[Buttons.Count];
                for (int i = 0; i < Buttons.Count; i++)
                    buttonStr[i] = $"({string.Join(',', Buttons[i])})";
                string joltageReqStr = $"{{{string.Join(',', JoltageReq)}}}";

                // Infeasible or invalid solution
                if (solution.Status != IntegerLinearProgramStatus.Optimal && solution.Status != IntegerLinearProgramStatus.Feasible)
                    throw new ProblemException($"ILP infeasible or invalid '{string.Join(' ', buttonStr)} {joltageReqStr}'.");

                // Count and print solution
                int solutionPresses = 0;
                StringBuilder sb = new();
                sb.AppendLine(joltageReqStr);
                for (int i = 0; i < Buttons.Count; i++)
                {
                    int buttonPressCount = (int)solution.GetValue(variableIndices[i]);
                    sb.AppendLine($"{buttonStr[i]} : {buttonPressCount}");
                    solutionPresses += buttonPressCount;
                }
                sb.AppendLine($"Solution presses: {solutionPresses}");
                _reporter.ReportLine(sb.ToString());

                totalPresses += solutionPresses;
            }

            return totalPresses;
        }
    }
}
