using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Http;

namespace Backend.Problems.Year2025Codelight.Rickebo;

public class Day25 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 25);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Caffeine Complexity";


    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";
        
        public override Task Solve(string input, Reporter reporter)
        {
            var lines = input.SplitLines().Select(Drink.From).OrderBy(x => x.Timestamp).ToArray();
            var solution = Solve(lines);
            
            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Solution = solution.ToString(),
                }
            );

            return Task.CompletedTask;
        }

        private int TypeComplexity(string name) =>
            name switch
            {
                "espresso" => 1,
                "americano" => 2,
                "cappuccino" => 3,
                "latte" => 3,
                "mocha" => 4,
                _ => throw new ArgumentOutOfRangeException()
            };

        private int GroupCost(List<Drink> drinks, int endIndex, Dictionary<int, int> cache)
        {
            if (endIndex <= 0)
                return 0;

            if (cache.TryGetValue(endIndex, out var groupCost))
                return groupCost;

            var best = int.MaxValue;

            var endDrink = drinks[endIndex - 1];
            var typeCost = TypeComplexity(endDrink.Name);

            for (var batchStart = endIndex; batchStart >= 1; batchStart--)
            {
                var earliest = drinks[batchStart - 1];
                var latest = endDrink;

                if (latest.Timestamp - earliest.Timestamp > 5)
                    break;

                var batchSize = endIndex - batchStart + 1;
                var cost = typeCost + (batchSize - 1) * (typeCost - 1);

                var candidate = GroupCost(drinks, batchStart - 1, cache) + cost;
                best = Math.Min(best, candidate);
            }

            cache[endIndex] = best;
            return best;
        }
        
        private int Solve(Drink[] lines) => 
            Categorize(lines)
                .Values
                .Select(items => GroupCost(items, items.Count, new Dictionary<int, int>()))
                .Sum();

        private static Dictionary<Category, List<Drink>> Categorize(Drink[] drinks)
        {
            var categorized = new Dictionary<Category, List<Drink>>();

            foreach (var drink in drinks)
            {
                if (categorized.TryGetValue(drink.Category, out var existing))
                    existing.Add(drink);
                else 
                    categorized[drink.Category] = [drink];
            }
            
            return categorized;
        }

        public record Category(string Name, string Size);
        
        public record Drink(string Name, string Size, int Timestamp)
        {
            public Category Category =>  new Category(Name, Size);
            
            public static Drink From(string line)
            {
                var components = line.Split(',');
                return new Drink(components[0], components[1], int.Parse(components[2]));
            }
        }
    }
}