using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lib.Text;

namespace Backend.Problems.Year2024.Rickebo;

public class Day13 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 13);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Claw Contraption";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(
                Parse(input)
                    .Sum(machine => machine.SolveFast())
            );

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(
                Parse(input)
                    .Select(machine => machine.Part2())
                    .Sum(machine => machine.SolveFast())
            );

            return Task.CompletedTask;
        }
    }

    public static List<ClawMachine> Parse(string input)
    {
        var result = new List<ClawMachine>();
        var machine = new List<Claw>();

        foreach (var line in input.SplitLines())
        {
            var current = Claw.Parse(line);

            if (current.Type == ClawType.Prize)
            {
                result.Add(new ClawMachine(machine, current));
                machine = [];
            }
            else
                machine.Add(current);
        }

        return result;
    }

    public class ClawMachine(List<Claw> buttons, Claw prize)
    {
        private const long Offset = 10000000000000;
        public List<Claw> Buttons { get; } = buttons;
        public Claw Prize { get; } = prize;

        public long SolveFast()
        {
            var a = Buttons.First(button => button.Type == ClawType.A);
            var b = Buttons.First(button => button.Type == ClawType.B);

            // Online gauss eliminator (https://matrixcalc.org/slu.html) yields:
            var qn = a.X * Prize.Y - a.Y * Prize.X;
            var qd = a.X * b.Y - a.Y * b.X;

            // If xn is not evenly divisible with xd, no solution exists
            if (qn % qd != 0)
                return 0;

            var q = qn / qd;
            var pn = Prize.X - b.X * q;

            // Same for yn and a.X
            if (pn % a.X != 0)
                return 0;

            var p = pn / a.X;

            return p * a.Cost + q * b.Cost;
        }

        public ClawMachine Part2() => new(
            Buttons,
            new Claw(Prize.X + Offset, Prize.Y + Offset, 0)
        );
    }

    public readonly struct Claw(long x, long y, ClawType type)
    {
        public long X { get; } = x;
        public long Y { get; } = y;
        public ClawType Type { get; } = type;

        public static Claw Parse(string text)
        {
            var values = Parser.GetValues<long>(text);
            var type = text.StartsWith("Prize")
                ? ClawType.Prize
                : text.StartsWith("Button A")
                    ? ClawType.A
                    : ClawType.B;

            return new Claw(values[0], values[1], type);
        }

        public long Cost => Type switch
        {
            ClawType.A => 3,
            ClawType.B => 1,
            _ => throw new Exception("Prize does not have a cost."),
        };
    }

    public enum ClawType
    {
        A,
        B,
        Prize
    }
}

