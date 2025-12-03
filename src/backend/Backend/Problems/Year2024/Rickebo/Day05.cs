using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Text;

namespace Backend.Problems.Year2024.Rickebo;

public class Day05 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 05);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Print Queue";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = CountPartOne(Parse(input)).ToString()
                }
            );

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = CountPartTwo(Parse(input)).ToString()
                }
            );

            return Task.CompletedTask;
        }
    }

    private static int CountPartOne(ProblemState state) => CountMiddlePages(FindUpdates(state, true));

    private static int CountPartTwo(ProblemState state) =>
        CountMiddlePages(FixUpdates(state, FindUpdates(state, false)));

    private static int CountMiddlePages(IEnumerable<Update> updates) =>
        updates.Sum(page => page.Pages[page.Pages.Length / 2]);

    private static List<Update> FixUpdates(ProblemState state, IEnumerable<Update> incorrect)
    {
        var noSet = new HashSet<int>();
        var reorderedUpdates = new List<Update>();

        foreach (var incorrectUpdate in incorrect)
        {
            var pageSet = incorrectUpdate.Pages.ToHashSet();
            var seen = new HashSet<int>();
            var after = new HashSet<int>(pageSet);
            var update = new List<int>();
            var queue = new Queue<int>(incorrectUpdate.Pages);

            while (queue.Count > 0)
            {
                var page = queue.Dequeue();

                var pre = state.Before.GetValueOrDefault(page, noSet).ToHashSet();
                var post = state.After.GetValueOrDefault(page, noSet).ToHashSet();

                pre.IntersectWith(pageSet);
                post.IntersectWith(pageSet);

                if (pre.IsSubsetOf(seen) && post.IsSubsetOf(after))
                {
                    update.Add(page);
                    seen.Add(page);
                    after.Remove(page);
                }
                else
                {
                    queue.Enqueue(page);
                }
            }

            reorderedUpdates.Add(new Update(update.ToArray()));
        }

        return reorderedUpdates;
    }

    private static List<Update> FindUpdates(ProblemState state, bool correct)
    {
        var noSet = new HashSet<int>();
        var result = new List<Update>();

        foreach (var update in state.Updates)
        {
            var pageSet = update.Pages.ToHashSet();
            var seen = new HashSet<int>();
            var after = new HashSet<int>(pageSet);
            var updateWrong = false;

            foreach (var number in update.Pages)
            {
                var beforeSet = state.Before.GetValueOrDefault(number, noSet);
                var afterSet = state.After.GetValueOrDefault(number, noSet);

                after.Remove(number);
                var isWrong = seen.Any(seenNumber => afterSet.Contains(seenNumber)) ||
                              after.Any(afterNumber => beforeSet.Contains(afterNumber));
                seen.Add(number);

                if (!isWrong) continue;

                updateWrong = true;
                break;
            }

            if (updateWrong == !correct)
                result.Add(update);
        }

        return result;
    }

    private static ProblemState Parse(string input)
    {
        var orders = new List<Order>();
        var updates = new List<Update>();

        foreach (var line in input.Split(["\r", "\n", "\r\n"], StringSplitOptions.RemoveEmptyEntries))
        {
            var numbers = Parser.GetValues<int>(line);
            if (line.Contains('|'))
                orders.Add(new Order(numbers[0], numbers[1]));
            else
                updates.Add(new Update(numbers));
        }

        var before = new Dictionary<int, HashSet<int>>();
        var after = new Dictionary<int, HashSet<int>>();

        foreach (var order in orders)
        {
            if (!before.TryGetValue(order.After, out var beforeSet))
                before[order.After] = beforeSet = [];

            if (!after.TryGetValue(order.Before, out var afterSet))
                after[order.Before] = afterSet = [];

            beforeSet.Add(order.Before);
            afterSet.Add(order.After);
        }

        return new ProblemState(updates, before, after);
    }

    private record ProblemState(
        ICollection<Update> Updates,
        Dictionary<int, HashSet<int>> Before,
        Dictionary<int, HashSet<int>> After
    );

    private record Order(int Before, int After);

    private record Update(int[] Pages);
}

