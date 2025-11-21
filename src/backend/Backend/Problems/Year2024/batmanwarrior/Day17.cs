using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib;
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
            reporter.Report(FinishedProblemUpdate.FromSolution(computer.LowestCopyA()));
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
            // Run program
            _program.Run();

            // Return output
            return _program.OutputToStr();
        }

        public long LowestCopyA()
        {
            // Get program opcodes
            long[] opcodes = [.. _program.OpCodes];

            // Reverse opcodes
            long[] reversed = opcodes.Reverse().ToArray();

            // Go backwards
            List<long> validVals = [0];
            for (int i = 0; i < reversed.Length; i++)
            {
                // Calculate new possible values of A
                List<long> newValsA = [];
                foreach (long val in validVals)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        // Make room for bits
                        long newA = val << 3;

                        // Add bits
                        newA += j;

                        // Re-run program
                        _program.Reset(newA);
                        _program.Run();

                        // Check if program output number match reversed opcode array
                        if (_program.Output[0] == reversed[i])
                            newValsA.Add(newA);
                    }
                }
                
                // Store possible values of A for next opcode number
                validVals = [.. newValsA];
            }

            // Return lowest possible A for a copy
            return validVals.Order().First();
        }
    }

    public class Program(int a, int b, int c, int[] opCodes)
    {
        public readonly int[] OpCodes = opCodes;
        public long A = a;
        public long B = b;
        public long C = c;
        public int pointer = 0;
        public List<long> Output = [];

        public string OpCodesToStr() => string.Join(",", OpCodes);

        public void Reset(long a)
        {
            // Reset program values for a new run
            A = a;
            B = 0;
            C = 0;
            pointer = 0;
            Output.Clear();
        }

        public void Run()
        {
            // While program not finished
            while (pointer >= 0 && pointer < OpCodes.Length)
            {
                // Check what operation to perform
                switch (OpCodes[pointer])
                {
                    case 0: // Division adv
                        A = (long)Math.Floor(A / Math.Pow(2, ComboOperand(pointer)));
                        break;
                    case 1: // Bitwise XOR bxl
                        B ^= OpCodes[pointer + 1];
                        break;
                    case 2: // Modulo 8 bst
                        B = ComboOperand(pointer) & 7;
                        break;
                    case 3: // Jump if A != 0 jnz
                        if (A != 0) pointer = OpCodes[pointer + 1] - 2;
                        break;
                    case 4: // Bitwise XOR bxc
                        B ^= C;
                        break;
                    case 5: // Output out
                        Output.Add(ComboOperand(pointer) & 7);
                        break;
                    case 6: // Division bdv
                        B = (long)Math.Floor(A / Math.Pow(2, ComboOperand(pointer)));
                        break;
                    case 7: // Division cdv
                        C = (long)Math.Floor(A / Math.Pow(2, ComboOperand(pointer)));
                        break;
                }

                // Go to next operation
                pointer += 2;
            }
        }

        public string OutputToStr() => string.Join(",", Output);

        private long ComboOperand(int pointer) => OpCodes[pointer + 1] switch
        {
            0 or 1 or 2 or 3 => OpCodes[pointer + 1],
            4 => A,
            5 => B,
            6 => C,
            _ => throw new NotImplementedException(),
        };
    }
}