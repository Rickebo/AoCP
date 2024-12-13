using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lib;
using Lib.Coordinate;

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

    public record ClawMachine(List<Claw> Buttons, Claw Prize)
    {
        public long SolveFast()
        {
            var a = Buttons.First(button => button.Type == ClawType.A);
            var b = Buttons.First(button => button.Type == ClawType.B);

            var xn = a.X * Prize.Y - a.Y * Prize.X;
            var xd = a.X * b.Y - a.Y * b.X;

            // If xn is not evenly divisible with xd, no solution exists
            if (xn % xd != 0)
                return 0;

            var x = xn / xd;
            var yn = Prize.X - b.X * x;

            // Same for yn and a.X
            if (yn % a.X != 0)
                return 0;

            var y = yn / a.X;

            return y * 3 + x;
        }

        public ClawMachine Part2() => this with
        {
            Prize = new Claw(
                Prize.X + 10000000000000,
                Prize.Y + 10000000000000,
                ClawType.Prize
            )
        };

        public long? Solve()
        {
            var a = Buttons.First(button => button.Type == ClawType.A);
            var b = Buttons.First(button => button.Type == ClawType.B);

            var maxA = Math.Min(Prize.X / a.X, Prize.Y / a.Y) + 1;
            var maxB = Math.Max(Prize.X / a.X, Prize.Y / a.Y) + 1;

            long? minScore = null;

            for (var i = 0L; i < maxA; i++)
            {
                var pos = a.Pos * i;
                var requiredBs = Prize.Pos - pos;

                var rbx = requiredBs.X / b.X;
                var rby = requiredBs.Y / b.Y;

                if (rbx != rby || requiredBs.X % b.X != 0 || requiredBs.Y % b.Y != 0)
                    continue;

                var score = i * a.Cost + rbx * b.Cost;
                if (minScore == null || score < minScore)
                    minScore = score;
            }

            return minScore;
        }

        public long? Solve(
            IntegerCoordinate<long> position,
            long presses = 0,
            Dictionary<IntegerCoordinate<long>, long?>? cache = null
        )
        {
            if (position.X > Prize.X || position.Y > Prize.Y)
                return null;

            if (position == Prize.Pos)
                return 0;

            cache ??= new Dictionary<IntegerCoordinate<long>, long?>();

            if (cache.TryGetValue(position, out var cachedScore))
                return cachedScore;

            long? minSolve = null;

            foreach (var button in Buttons)
            {
                var buttonPosition = position + button.Pos;
                var buttonResult = Solve(buttonPosition, presses + 1, cache);

                if (buttonResult == null)
                    continue;

                var actualButtonResult = buttonResult.Value + button.Cost;
                if (minSolve == null || actualButtonResult > minSolve)
                    minSolve = actualButtonResult;
            }

            return cache[position] = minSolve;
        }
    }

    public record Claw(long X, long Y, ClawType Type)
    {
        public IntegerCoordinate<long> Pos => new(X, Y);

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