using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib;
using System.Reflection;
using static Backend.Problems.Year2024.batmanwarrior.Day17;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day17 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 17);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Chronospatial Computer";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create computer
            Computer computer = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(computer.ProgramResult()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create computer
            Computer computer = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(computer.LowestPossibleA()));
            return Task.CompletedTask;
        }
    }

    public class Computer
    {
        private readonly Reporter _reporter;
        private readonly Program _program;

        public Computer(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Split input into rows
            string[] rows = input.SplitLines();

            // Retrieve register values
            int A = Parser.GetValues<int>(rows[0])[0];
            int B = Parser.GetValues<int>(rows[1])[0];
            int C = Parser.GetValues<int>(rows[2])[0];

            // Retrieve op codes
            int[] opCodes = Parser.GetValues<int>(rows[3]);

            // Create program
            _program = new(A, B, C, opCodes);
        }

        public string ProgramResult()
        {
            _program.Run();
            return _program.OutputToStr();
        }

        public long LowestPossibleA()
        {
            string target = _program.OpCodesToStr();

            long currA = 0;
            for (int i = _program.OpCodes.Length - 1; i >= 0; i--)
            {
                while (true)
                {
                    _program.Reset((int)currA);
                    _program.Run();
                    if (_program.Output.Count == _program.OpCodes.Length && _program.Output[i] == _program.OpCodes[i])
                        break;

                    currA <<= (3 + _program.OpCodes.Length - 1 - i);
                }
            }

            return currA;
            //int currA = 0;

            /*
            int i = _program.A & 7;
            for (; ; )
            {
                _program.Reset(i);
                _program.Run();

                if (_program.OutputToStr() == target)
                    return i;

                i += 8;
            }*/

            /*
            [0] B = A & 7
            [2] B = B ^ 1
            [4] C = A >> B
            [6] A = A >> 3
            [8] B = B ^ 4
            [10] B = B ^ C
            [12] out B & 7
            [14] jnz A to 0
            */

            // B = A & 7
            // B = (A & 7) ^ 1
            // C = A >> ((A & 7) ^ 1)
            // A = A >> 3
            // B = (A & 7) ^ 4
            // B = ((A & 7) ^ 4) ^ (A >> ((A & 7) ^ 1))
            // out (((A & 7) ^ 4) ^ (A >> ((A & 7) ^ 1))) & 7

            _program.RunTest();
            //_program.Run();

            _reporter.Report(TextProblemUpdate.FromLine(_program.OutputToStr()));

            // Print instructions performed
            foreach (string str in _program.Instructions)
                _reporter.Report(TextProblemUpdate.FromLine(str));

            return 0;
        }
    }

    public class Program(int a, int b, int c, int[] opCodes)
    {
        public readonly int[] OpCodes = opCodes;
        public int A = a;
        public int B = b;
        public int C = c;
        public int pointer = 0;
        public List<string> Instructions = [];
        public List<int> Output = [];

        public string OpCodesToStr() => string.Join(",", OpCodes);

        public void Reset(int a)
        {
            A = a;
            B = 0;
            C = 0;
            pointer = 0;
            Instructions.Clear();
            Output.Clear();
        }

        public void Run()
        {
            while (pointer >= 0 && pointer < OpCodes.Length)
            {
                switch (OpCodes[pointer])
                {
                    case 0: // Division adv
                        A >>= ComboOperand(pointer);
                        Instructions.Add($"[{pointer}] A = A >> {ComboToStr(pointer)}");
                        break;
                    case 1: // Bitwise XOR bxl
                        B ^= OpCodes[pointer + 1];
                        Instructions.Add($"[{pointer}] B = B ^ {OpCodes[pointer + 1]}");
                        break;
                    case 2: // Modulo 8 bst
                        B = ComboOperand(pointer) & 7;
                        Instructions.Add($"[{pointer}] B = {ComboToStr(pointer)} & 7");
                        break;
                    case 3: // Jump if A != 0 jnz
                        Instructions.Add($"[{pointer}] jnz A to {OpCodes[pointer + 1]}");
                        if (A != 0) pointer = OpCodes[pointer + 1] - 2;
                        break;
                    case 4: // Bitwise XOR bxc
                        B ^= C;
                        Instructions.Add($"[{pointer}] B = B ^ C");
                        break;
                    case 5: // Output out
                        Output.Add(ComboOperand(pointer) & 7);
                        Instructions.Add($"[{pointer}] out {ComboToStr(pointer)} & 7");
                        break;
                    case 6: // Division bdv
                        B = A >> ComboOperand(pointer);
                        Instructions.Add($"[{pointer}] B = A >> {ComboToStr(pointer)}");
                        break;
                    case 7: // Division cdv
                        C = A >> ComboOperand(pointer);
                        Instructions.Add($"[{pointer}] C = A >> {ComboToStr(pointer)}");
                        break;
                }

                pointer += 2;
            }
        }

        public void RunTest()
        {
            while (A != 0)
            {
                C = A >> ((A & 7) ^ 1);
                B = (((A & 7) ^ 1) ^ 4) ^ C;
                Output.Add(((((A & 7) ^ 4) ^ (A >> ((A & 7) ^ 1))) & 7));
                A >>= 3;
            }
        }

        public string OutputToStr() => string.Join(",", Output);

        private int ComboOperand(int pointer) => OpCodes[pointer + 1] switch
        {
            0 or 1 or 2 or 3 => OpCodes[pointer + 1],
            4 => A,
            5 => B,
            6 => C,
            _ => throw new ArgumentException(),
        };

        private string ComboToStr(int pointer) => OpCodes[pointer + 1] switch
        {
            0 or 1 or 2 or 3 => OpCodes[pointer + 1].ToString(),
            4 => "A",
            5 => "B",
            6 => "C",
            _ => throw new ArgumentException(),
        };
    }
}