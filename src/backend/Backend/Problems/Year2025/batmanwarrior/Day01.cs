using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lib.Geometry;
using Lib.Math;
using Lib.Text;

namespace Backend.Problems.Year2025.batmanwarrior;

public class Day01 : ProblemSet
{
    public override DateTime ReleaseTime { get; } = new(2025, 12, 01);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Secret Entrance";

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
        private readonly List<(Rotation, int)> _rotations = [];

        public Solver(string input, Reporter reporter, int _)
        {
            _reporter = reporter;

            foreach (var line in input.SplitLines())
            {
                var dir = RotationExtensions.Parse(line[0]);

                if (!int.TryParse(line[1..], out var amount))
                    throw new ProblemException("Invalid input.");

                _rotations.Add((dir, amount));
            }
        }

        public int PartOne()
        {
            var table = new TabularReport();
            table.AddColumn("Rotation", ColumnAlignment.Center);
            table.AddColumn("Direction", ColumnAlignment.Center);
            table.AddColumn("Amount", ColumnAlignment.Center);
            table.AddColumn("PrevPos", ColumnAlignment.Center);
            table.AddColumn("NewPos", ColumnAlignment.Center);
            table.AddColumn("Zero Hit", ColumnAlignment.Center);

            var pos = 50;
            var password = 0;
            var step = 1;
            foreach (var (dir, amount) in _rotations)
            {
                // Rotate
                int prevPos = pos;
                pos += dir == Rotation.Clockwise ? amount : -amount;
                pos = MathExtensions.Modulo(pos, 100);

                // Pointing at zero
                var zeroHit = pos == 0 ? 1 : 0;
                password += zeroHit;

                table.AddRow(step++, RotationExtensions.ToGlyph(dir), amount, prevPos, pos, zeroHit);
            }

            _reporter.ReportTable(table);

            return password;
        }

        public int PartTwo()
        {
            var table = new TabularReport();
            table.AddColumn("Rotation", ColumnAlignment.Center);
            table.AddColumn("Direction", ColumnAlignment.Center);
            table.AddColumn("Amount", ColumnAlignment.Center);
            table.AddColumn("PrevPos", ColumnAlignment.Center);
            table.AddColumn("NewPos", ColumnAlignment.Center);
            table.AddColumn("Zero Hits", ColumnAlignment.Center);

            var pos = 50;
            var password = 0;
            var step = 1;
            foreach (var (dir, amount) in _rotations)
            {
                // Distance to zero
                var distance = dir == Rotation.Clockwise ? 100 - pos : pos;

                // Zero hits
                var zeroHits = 0;
                if (distance == 0)
                    zeroHits = amount / 100;
                else if (amount >= distance)
                    zeroHits = 1 + (amount - distance) / 100;
                password += zeroHits;

                // Rotate
                int prevPos = pos;
                pos += dir == Rotation.Clockwise ? amount : -amount;
                pos = MathExtensions.Modulo(pos, 100);

                table.AddRow(step++, RotationExtensions.ToGlyph(dir), amount, prevPos, pos, zeroHits);
            }

            _reporter.ReportTable(table);

            return password;
        }
    }
}
