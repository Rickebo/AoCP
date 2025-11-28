using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Lib.Extensions;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day22 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 22);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Monkey Market";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create monkey market
            MonkeyMarket monkeyMarket = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(monkeyMarket.SumGeneratedSecrets(times: 2000)));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create monkey market
            MonkeyMarket monkeyMarket = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(monkeyMarket.GetMostBananas(times: 2000)));
            return Task.CompletedTask;
        }
    }

    public class MonkeyMarket
    {
        private readonly Reporter _reporter;
        private readonly List<Buyer> _buyers = [];

        public MonkeyMarket(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Retrieve buyer secret numbers
            foreach (string row in input.SplitLines())
                _buyers.Add(new(long.Parse(row)));
        }

        public long SumGeneratedSecrets(int times)
        {
            // Generate new secret number for all buyers
            long sum = 0;
            foreach (Buyer buyer in _buyers)
            {
                // Save for printing
                long initialSecret = buyer.Secret;

                // Generate new secret number
                buyer.Generate(times);

                // Accumulate sum
                sum += buyer.Secret;

                // Send to frontend
                _reporter.Report(TextProblemUpdate.FromLine($"{initialSecret,-8}: {buyer.Secret,-8} | {sum}"));
            }

            return sum;
        }

        public long GetMostBananas(int times)
        {
            // Save bananas bought for different price changes
            Dictionary<Buyer.PriceChange, long> bananas = [];

            // Buy bananas with all buyers
            foreach (Buyer buyer in _buyers)
                buyer.BuyBananas(times, bananas);

            // Return the most bananas you can get from a single sequence of price changes
            return bananas.Values.Max();
        }
    }

    public class Buyer(long secret)
    {
        public record PriceChange(int One, int Two, int Three, int Four);
        public long Secret = secret;

        private void NewSecret()
        {
            long val = Secret << 6; // Multiply secret by 64 (2^6)
            Secret ^= val; // Mix with secret
            Secret %= 16777216; // Prune secret
            val = Secret >> 5; // Divide secret by 32 (2^-5)
            Secret ^= val; // Mix with secret
            Secret %= 16777216; // Prune secret
            val = Secret << 11; // Multiply secret by 2048 (2^11)
            Secret ^= val; // Mix with secret
            Secret %= 16777216; // Prune secret
        }

        public void Generate(int times)
        {
            // Generate new secrets X times
            for (int i = 0; i < times; i++)
                NewSecret();
        }

        public void BuyBananas(int times, Dictionary<PriceChange, long> bananas)
        {
            // Times need to be larger than 4 for any banana entries
            if (times <= 4)
                return;

            // Get initial price
            int price = GetPrice();
            int newPrice;

            // Store the first 4 changes
            int[] changes = new int[4];
            for (int i = 0; i < 4; i++)
            {
                // Generate new secret
                NewSecret();

                // Get new price
                newPrice = GetPrice();

                // Save price change
                changes[i] = newPrice - price;

                // Update price
                price = newPrice;
            }

            // Create first price change key
            PriceChange priceChange = new(changes[0], changes[1], changes[2], changes[3]);

            // Already added price changes
            HashSet<PriceChange> seenPriceChanges = [priceChange];

            // Add to bananas
            bananas[priceChange] = bananas.TryGetValue(priceChange, out long value1) ? value1 + price : price;

            // Generate the rest of the secret numbers
            for (int i = 4; i < times; i++)
            {
                // Generate new secret
                NewSecret();

                // Get new price
                newPrice = GetPrice();

                // Create new price change
                PriceChange newPriceChange = new(priceChange.Two, priceChange.Three, priceChange.Four, newPrice - price);

                // Add to bananas if this is the first time this price change occurs for this buyer
                if (seenPriceChanges.Add(newPriceChange))
                    bananas[newPriceChange] = bananas.TryGetValue(newPriceChange, out long value2) ? value2 + newPrice : newPrice;

                // Update price change
                priceChange = newPriceChange;

                // Update price
                price = newPrice;
            }
        }

        private int GetPrice() => (int)(Secret % 10);
    }
}