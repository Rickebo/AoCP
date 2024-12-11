using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;

namespace Backend.Problems.Year2023.batmanwarrior;

public class Day1 : ProblemSet
{
    public override DateTime ReleaseTime => new(2023, 12, 01);

    public override List<Problem> Problems =>
    [
        new PartOne(),
        new PartTwo()
    ];

    public override string Name => "Trebuchet!?";

    private class PartOne : Problem
    {
        public override Task Solve(string input, Reporter reporter)
        {
            var rows = input.SplitLines();
            var result = rows.Select(CalibrationValue).Sum();
            
            reporter.ReportSolution(result);
            return Task.CompletedTask;
        }
    }

    private class PartTwo : Problem
    {
        public override Task Solve(string input, Reporter reporter)
        {
            var rows = input.SplitLines();
            var result = rows
                .Select(WordsToNumbers)
                .Select(CalibrationValue)
                .Sum();

            reporter.ReportSolution(result);
            return Task.CompletedTask;
        }
    }

    private static int CalibrationValue(string str, int y)
    {
        int sum = 0, pos1 = 0, pos2 = 0;
        for (int j = 0; j < str.Length; j++)
        {
            if (Char.IsNumber(str[j]))
            {
                sum += 10 * (str[j] - '0');
                pos1 = j;
                break;
            }
        }

        for (int j = str.Length; j-- > 0;)
        {
            if (Char.IsNumber(str[j]))
            {
                sum += str[j] - '0';
                pos2 = j;
                break;
            }
        }

        return sum;
    }

    private static string WordsToNumbers(string str)
    {
        foreach (var entry in letterNumbers)
        {
            str = str.Replace(entry.Key, entry.Value);
        }

        return str;
    }

    private static readonly Dictionary<string, string> letterNumbers = new()
    {
        { "one", "o1e" },
        { "two", "t2o" },
        { "three", "t3e" },
        { "four", "f4r" },
        { "five", "f5e" },
        { "six", "s6x" },
        { "seven", "s7n" },
        { "eight", "e8t" },
        { "nine", "n9e" }
    };
}