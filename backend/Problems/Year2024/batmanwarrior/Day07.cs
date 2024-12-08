using Common;
using Common.Updates;
using Lib;
using System.Text;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day07 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 07);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Bridge Repair";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Spawn obunga
            Obunga obunga = new(input, ['+', '*'], reporter);

            // Run from obunga
            reporter.Report(FinishedProblemUpdate.FromSolution(obunga.Chase()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Spawn obunga
            Obunga obunga = new(input, ['+', '*', '|'], reporter);

            // Run from obunga
            reporter.Report(FinishedProblemUpdate.FromSolution(obunga.Chase()));
            return Task.CompletedTask;
        }
    }

    public class Obunga
    {
        private readonly List<Equation> _equations = [];
        private readonly Reporter _reporter;
        private readonly int _printLen;

        public Obunga(string input, char[] operators, Reporter reporter)
        {
            // Frontend
            _reporter = reporter;

            // Parse equations
            foreach (string row in input.SplitLines())
            {
                _equations.Add(new(row, operators));
                _printLen = Math.Max(_printLen, row.Length);
            }
        }

        public long Chase()
        {
            // Loop over all equations
            long _calibrationResult = 0;
            foreach (Equation equation in _equations)
            {
                // Try to solve the equation
                equation.TrySolve();

                // Format print text
                string text = $"{equation.Raw.PadRight(_printLen)} <> {equation.ToPrintStr()}";

                // Update calibration result
                _calibrationResult += equation.Solved ? equation.Target : 0;

                // Send to frontend
                _reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [text]
                    }
                );
            }
            
            return _calibrationResult;
        }
    }

    public class Equation
    {
        public readonly string Raw;
        public readonly long Target;
        public readonly long[] Numbers;
        public readonly char[] Operators;
        public readonly long Combinations;
        public int[] OperatorArray;
        public bool Solved = false;

        public Equation(string row, char[] operators)
        {
            // Store raw string
            Raw = row;

            // Parse values
            long[] values = Parser.GetValues<long>(row);

            // Set target
            Target = values[0];

            // Skip target number and retrieve the rest
            Numbers = values.Skip(1).ToArray();

            // Set operator characters
            Operators = operators;

            // Create operator array for different combinations later
            OperatorArray = new int[Numbers.Length - 1];

            // Calculate how many combinations of operators there are
            Combinations = (long)Math.Pow(Operators.Length, OperatorArray.Length);
        }

        public void TrySolve()
        {
            // Check all possible combinations
            for (long i = 0; i < Combinations; i++)
            {
                // Accumulate sum
                long sum = Numbers[0];
                for (int j = 1; j < Numbers.Length; j++)
                {
                    // Operator action
                    switch (Operators[OperatorArray[j - 1]])
                    {
                        case '+':
                            sum += Numbers[j];
                            break;
                        case '*':
                            sum *= Numbers[j];
                            break;
                        case '|':
                            sum = ConcatInts(sum, Numbers[j]);
                            break;
                        default:
                            throw new ArgumentException();
                    }

                    // Cancel early if overshooting target
                    if (sum > Target)
                    {
                        break;
                    }
                }

                // Return if target reached with current combination
                if (sum == Target)
                {
                    Solved = true;
                    return;
                }

                // Try next combination
                NextCombination();
            }
        }

        public void NextCombination()
        {
            // Increment combination array
            for (int i = 0; i < OperatorArray.Length; i++)
            {
                if (++OperatorArray[i] < Operators.Length)
                {
                    break;
                }
                else
                {
                    // Rollover
                    OperatorArray[i] = 0;
                }
            }
        }

        public static long ConcatInts(long a, long b)
        {
            // Black magic deluxe
            return a * (long)Math.Pow(10, Math.Ceiling(Math.Log10(b + 1))) + b;
        }

        public string ToPrintStr()
        {
            // Return ? if not solved
            if (!Solved)
            {
                return "?";
            }
                
            // Create string builder
            StringBuilder sb = new();
            sb.Append(Numbers[0]);

            // Add operators between numbers
            for (int i = 1; i < Numbers.Length; i++)
            {
                sb.Append($" {Operators[OperatorArray[i - 1]]} ");
                sb.Append(Numbers[i]);
            }

            // Add product
            sb.Append($" = {Target}");

            // Return equation string representation
            return sb.ToString();
        }
    }
}