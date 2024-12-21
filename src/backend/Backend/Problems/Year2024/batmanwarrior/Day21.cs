using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib.Coordinate;
using Lib.Grid;
using System.Text;
using Lib;
using System.Linq;

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
            reporter.Report(FinishedProblemUpdate.FromSolution(starship.Complexitites()));
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
            reporter.Report(FinishedProblemUpdate.FromSolution("?"));
            return Task.CompletedTask;
        }
    }

    public class Starship
    {
        // WRONG1: 259336 (too high)
        // WRONG2: 254554 (too high)
        private readonly Reporter _reporter;
        private readonly List<string> _codes = [];
        private readonly Dictionary<string, string> _compare = new Dictionary<string, string>() {
            {"029A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A"},
            {"980A", "<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A"},
            {"179A", "<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"},
            {"456A", "<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A"},
            {"379A", "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"},
        };

        public Starship(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Retrieve door codes
            foreach (string row in input.SplitLines())
                _codes.Add(row);
        }

        public long Complexitites()
        {
            long totalComplexity = 0;
            foreach (string code in _codes)
            {
                string sequence = Dirpad.Sequence(Dirpad.Sequence(Numpad.Sequence(code)));
                long codeVal = long.Parse(code[.. (code.Length - 1)]);
                int presses = sequence.Count(c => c == 'A');
                int dirs = sequence.Count(c => c == '<' || c == '^' || c == '>' || c == 'v');
                int pressCompare = _compare[code].Count(c => c == 'A');
                int dirCompare = _compare[code].Count(c => c == '<' || c == '^' || c == '>' || c == 'v');
                totalComplexity += sequence.Length * codeVal;
            }

            return totalComplexity;
        }
    }

    public class Robot(RobotKeypad keyPad)
    {
        public readonly RobotKeypad KeyPad = keyPad;
    }

    public enum RobotKeypad
    {
        Numeric,
        Directional
    }

    public static class Numpad
    {
        // 7 8 9
        // 4 5 6
        // 1 2 3
        // # 0 A
        private static readonly CharGrid _grid = new(new char[,] {
            { '#', '1', '4', '7' },
            { '0', '2', '5', '8' },
            { 'A', '3', '6', '9' },
        });

        public static string Sequence(string seq)
        {
            // Create stringbuilder
            StringBuilder sb = new();

            // Get start position
            IntegerCoordinate<int> pos = _grid.Find(val => val == 'A');

            foreach (char c in seq)
            {
                // Get target position
                IntegerCoordinate<int> target = _grid.Find(val => val == c);

                // If already on target, press
                if (target == pos)
                {
                    sb.Append('A');
                    continue;
                }

                // Go to target
                if (target.Y == pos.Y)
                {
                    // Target on same row, go horizontal
                    sb.Append(new string(target.X > pos.X ? '>' : '<', Math.Abs(target.X - pos.X)));
                }
                else if (target.X == pos.X)
                {
                    // Target on same col, go vertical
                    sb.Append(new string(target.Y > pos.Y ? '^' : 'v', Math.Abs(target.Y - pos.Y)));
                }
                else if (pos.Y == 0 && target.X == 0)
                {
                    // Start vertical to avoid gap
                    sb.Append(new string(target.Y > pos.Y ? '^' : 'v', Math.Abs(target.Y - pos.Y)));
                    sb.Append(new string(target.X > pos.X ? '>' : '<', Math.Abs(target.X - pos.X)));
                }
                else
                {
                    // All other paths can start horizontal
                    sb.Append(new string(target.X > pos.X ? '>' : '<', Math.Abs(target.X - pos.X)));
                    sb.Append(new string(target.Y > pos.Y ? '^' : 'v', Math.Abs(target.Y - pos.Y)));
                }

                // Press and update position
                sb.Append('A');
                pos = target;
            }

            // Return sequence string
            return sb.ToString();
        }
    }

    public static class Dirpad
    {
        // # ^ A
        // < v >
        private static readonly CharGrid _grid = new(new char[,] {
            { '<', '#' },
            { 'v', '^' },
            { '>', 'A' },
        });

        public static string Sequence(string seq)
        {
            // Create stringbuilder
            StringBuilder sb = new();
            
            // Optimize order (group moves)
            string[] moveSequences = seq.Split('A');
            foreach (string moveSeq in moveSequences)
            {
                sb.Append(new string('<', moveSeq.Count(c => c == '<')));
                sb.Append(new string('^', moveSeq.Count(c => c == '^')));
                sb.Append(new string('v', moveSeq.Count(c => c == 'v')));
                sb.Append(new string('>', moveSeq.Count(c => c == '>')));
                sb.Append('A');
            }

            // Create new seq BUGGED BRUH
            //string temp = sb.ToString();
            //seq = temp[.. (temp.Length - 1)];

            // Clear stringbuilder
            sb.Clear();

            // Get start position
            IntegerCoordinate<int> pos = _grid.Find(val => val == 'A');

            foreach (char c in seq)
            {
                // Get target position
                IntegerCoordinate<int> target = _grid.Find(val => val == c);

                // If already on target, press
                if (target == pos)
                {
                    sb.Append('A');
                    continue;
                }

                // Go to target
                if (target.Y == pos.Y)
                {
                    // Target on same row, go horizontal
                    sb.Append(new string(target.X > pos.X ? '>' : '<', Math.Abs(target.X - pos.X)));
                }
                else if (target.X == pos.X)
                {
                    // Target on same col, go vertical
                    sb.Append(new string(target.Y > pos.Y ? '^' : 'v', Math.Abs(target.Y - pos.Y)));
                }
                else if (pos.Y == 1 && target.X == 0)
                {
                    // Start vertical to avoid gap
                    sb.Append(new string(target.Y > pos.Y ? '^' : 'v', Math.Abs(target.Y - pos.Y)));
                    sb.Append(new string(target.X > pos.X ? '>' : '<', Math.Abs(target.X - pos.X)));
                }
                else
                {
                    // All other paths can start horizontal
                    sb.Append(new string(target.X > pos.X ? '>' : '<', Math.Abs(target.X - pos.X)));
                    sb.Append(new string(target.Y > pos.Y ? '^' : 'v', Math.Abs(target.Y - pos.Y)));
                }

                // Press and update position
                sb.Append('A');
                pos = target;
            }

            // Return sequence string
            return sb.ToString();
        }
    }
}