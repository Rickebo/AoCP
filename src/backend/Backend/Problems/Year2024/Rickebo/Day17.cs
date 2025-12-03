using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Lib.Text;

namespace Backend.Problems.Year2024.Rickebo;

public class Day17 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 17);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Chronospatial Computer";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(string.Join(",", Computer.Parse(input).Run(reporter)));

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(Computer.Parse(input).FindInstructionRec(reporter).Min());

            return Task.CompletedTask;
        }
    }

    private record Instruction(ulong Opcode, ulong Value);

    private class Computer
    {
        private Instruction[] Instructions { get; }
        private ulong[] InstructionValues { get; }
        private ulong[] OriginalRegisters { get; }
        private ulong[] Registers { get; set; }
        private ulong ProgramCounter { get; set; } = 0;
        private ulong InstructionCounter => ProgramCounter / 2;

        private const int A = 0;
        private const int B = 1;
        private const int C = 2;

        public Computer(ulong[] instructionValues, Instruction[] instructions, ulong[] registers)
        {
            Instructions = instructions;
            InstructionValues = (ulong[])instructionValues.Clone();
            OriginalRegisters = (ulong[])registers.Clone();
            Registers = (ulong[])registers.Clone();
        }

        public static Computer Parse(string input)
        {
            var registers = new ulong[3];
            var registerIndex = 0;
            var instructions = new List<Instruction>();
            ulong[]? instructionValues = null;

            foreach (var line in input.SplitLines())
            {
                var values = Parser.GetValues<ulong>(line);
                if (line.StartsWith("Register"))
                    registers[registerIndex++] = values[0];
                else
                {
                    for (var i = 0; i < values.Length; i += 2)
                        instructions.Add(new Instruction(values[i], values[i + 1]));

                    instructionValues = values;
                }
            }

            return new Computer(
                instructionValues ?? throw new Exception("Found no instruction values."),
                instructions.ToArray(),
                registers
            );
        }

        private ulong GetCombo(ulong literal) => literal switch
        {
            > 3 and < 7 => Registers[literal - 4],
            > 6 or < 0 => throw new Exception("Invalid combo value."),
            _ => literal
        };

        private string GetNamedCombo(ulong literal) => literal switch
        {
            4 => "A",
            5 => "B",
            6 => "C",
            > 6 or < 0 => throw new Exception("Invalid combo value."),
            _ => literal.ToString()
        };

        private static void ReportColumns(Reporter? reporter, params string[] columns)
        {
            if (reporter == null) return;

            var padding = 20;
            var sb = new StringBuilder();
            foreach (var col in columns)
            {
                sb.Append(col);
                if (col.Length < padding)
                    sb.Append(new string(' ', padding - col.Length));
            }

            reporter.ReportLine(sb.ToString());
        }

        public IEnumerable<ulong> FindInstructionRec(Reporter? reporter, int? instructionIndex = null)
        {
            var maxIndex = InstructionValues.Length - 1;
            instructionIndex ??= maxIndex;
            if (instructionIndex < 0)
            {
                yield return 0;
                yield break;
            }

            var processed = InstructionValues[^(instructionIndex.Value + 1)..];

            foreach (var sub in FindInstructionRec(reporter, instructionIndex - 1))
            {
                for (var i = 0ul; i < 8; i++)
                {
                    Reset();

                    var a = sub << 3 | i;
                    Registers[A] = a;

                    var res = Run();
                    if (!processed.SequenceEqual(res))
                        continue;

                    reporter?.ReportLine($"{a.ToString(),-20}{string.Join(",", processed)}");
                    yield return a;
                }
            }
        }

        public void Reset()
        {
            ProgramCounter = 0;
            Array.Copy(OriginalRegisters, Registers, Registers.Length);
        }

        public List<ulong> Run(Reporter? reporter = null)
        {
            ProgramCounter = 0;

            var result = new List<ulong>();
            while ((int)InstructionCounter < Instructions.Length)
            {
                var (op, literal) = Instructions[(int)InstructionCounter];

                var jump = false;

                var a = Registers[A];
                var b = Registers[B];
                var c = Registers[C];

                switch (op)
                {
                    case 0:
                        var combo = GetCombo(literal);
                        Registers[A] /= 1ul << (int)combo;
                        ReportColumns(
                            reporter,
                            $"A =",
                            $"A / 2^{GetNamedCombo(literal)} =",
                            $"{a} / {1 << (int)combo} =",
                            $"{Registers[A]}"
                        );
                        break;

                    case 1:
                        Registers[B] ^= literal;
                        ReportColumns(
                            reporter,
                            "B =",
                            "L ^ B =",
                            $"{b} ^ {literal} =",
                            $"{Registers[B]}"
                        );
                        break;

                    case 2:
                        Registers[B] = MathExtensions.Modulo(GetCombo(literal), 8ul);
                        ReportColumns(
                            reporter,
                            $"B =",
                            $"{GetNamedCombo(literal)} % 8 =",
                            $"{GetCombo(literal)} % {8} =",
                            $"{Registers[B]}"
                        );

                        break;

                    case 3:
                        if (Registers[A] == 0)
                        {
                            ReportColumns(reporter, "jnz", "skipped");
                            break;
                        }

                        ReportColumns(
                            reporter,
                            $"jnz -> {literal}",
                            $"A = {a}",
                            $"B = {b}",
                            $"C = {c}"
                        );
                        ProgramCounter = literal;
                        jump = true;
                        break;

                    case 4:
                        Registers[B] ^= Registers[C];
                        ReportColumns(
                            reporter,
                            "B =",
                            "b ^ c =",
                            $"{b} ^ {c} =",
                            $"{Registers[B]}"
                        );
                        break;

                    case 5:
                        result.Add(MathExtensions.Modulo(GetCombo(literal), 8ul));
                        ReportColumns(
                            reporter,
                            "out",
                            $"{GetNamedCombo(literal)} % 8 =",
                            $"{GetCombo(literal)} % 8 =",
                            $"{MathExtensions.Modulo(GetCombo(literal), 8ul)}"
                        );
                        break;

                    case 6:
                        combo = GetCombo(literal);
                        Registers[B] = Registers[A] / (1ul << (int)GetCombo(literal));

                        ReportColumns(
                            reporter,
                            $"B =",
                            $"A / 2^{GetNamedCombo(literal)} =",
                            $"{a} / {1ul << (int)combo} =",
                            $"{Registers[B]}"
                        );
                        break;

                    case 7:
                        combo = GetCombo(literal);
                        Registers[C] = Registers[A] / (1ul << (int)combo);

                        ReportColumns(
                            reporter,
                            $"C =",
                            $"A / 2^{GetNamedCombo(literal)} =",
                            $"{a} / {1ul << (int)combo} =",
                            $"{Registers[C]}"
                        );
                        break;
                }

                if (!jump)
                    ProgramCounter += 2;
            }

            reporter?.ReportLine("Solution: " + string.Join(",", result));

            return result;
        }
    }
}

