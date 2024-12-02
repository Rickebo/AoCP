using Common.Updates;

namespace Backend.Problems.Year2024;

public class Day2 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 02, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Historian Hysteria";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description =>
            "<article class=\"day-desc\"><h2>--- Day 1: Historian Hysteria ---</h2><p>The <em>Chief Historian</em> is always present for the big Christmas sleigh launch, but nobody has seen him in months! Last anyone heard, he was visiting locations that are historically significant to the North Pole; a group of Senior Historians has asked you to accompany them as they check the places they think he was most likely to visit.</p>\r\n<p>As each location is checked, they will mark it on their list with a <em class=\"star\">star</em>. They figure the Chief Historian <em>must</em> be in one of the first fifty places they'll look, so in order to save Christmas, you need to help them get <em class=\"star\">fifty stars</em> on their list before Santa takes off on December 25th.</p>\r\n<p>Collect stars by solving puzzles.  Two puzzles will be made available on each day in the Advent calendar; the second puzzle is unlocked when you complete the first.  Each puzzle grants <em class=\"star\">one star</em>. Good luck!</p>\r\n<p>You haven't even left yet and the group of Elvish Senior Historians has already hit a problem: their list of locations to check is currently <em>empty</em>. Eventually, someone decides that the best place to check first would be the Chief Historian's office.</p>\r\n<p>Upon pouring into the office, everyone confirms that the Chief Historian is indeed nowhere to be found. Instead, the Elves discover an assortment of notes and lists of historically significant locations! This seems to be the planning the Chief Historian was doing before he left. Perhaps these notes can be used to determine which locations to search?</p>\r\n<p>Throughout the Chief's office, the historically significant locations are listed not by name but by a unique number called the <em>location ID</em>. To make sure they don't miss anything, The Historians split into two groups, each searching the office and trying to create their own complete list of location IDs.</p>\r\n<p>There's just one problem: by holding the two lists up <em>side by side</em> (your puzzle input), it quickly becomes clear that the lists aren't very similar. Maybe you can help The Historians reconcile their lists?</p>\r\n<p>For example:</p>\r\n<pre><code>3   4\r\n4   3\r\n2   5\r\n1   3\r\n3   9\r\n3   3\r\n</code></pre>\r\n<p>Maybe the lists are only off by a small amount! To find out, pair up the numbers and measure how far apart they are. Pair up the <em>smallest number in the left list</em> with the <em>smallest number in the right list</em>, then the <em>second-smallest left number</em> with the <em>second-smallest right number</em>, and so on.</p>\r\n<p>Within each pair, figure out <em>how far apart</em> the two numbers are; you'll need to <em>add up all of those distances</em>. For example, if you pair up a <code>3</code> from the left list with a <code>7</code> from the right list, the distance apart is <code>4</code>; if you pair up a <code>9</code> with a <code>3</code>, the distance apart is <code>6</code>.</p>\r\n<p>In the example list above, the pairs and distances would be as follows:</p>\r\n<ul>\r\n<li>The smallest number in the left list is <code>1</code>, and the smallest number in the right list is <code>3</code>. The distance between them is <code><em>2</em></code>.</li>\r\n<li>The second-smallest number in the left list is <code>2</code>, and the second-smallest number in the right list is another <code>3</code>. The distance between them is <code><em>1</em></code>.</li>\r\n<li>The third-smallest number in both lists is <code>3</code>, so the distance between them is <code><em>0</em></code>.</li>\r\n<li>The next numbers to pair up are <code>3</code> and <code>4</code>, a distance of <code><em>1</em></code>.</li>\r\n<li>The fifth-smallest numbers in each list are <code>3</code> and <code>5</code>, a distance of <code><em>2</em></code>.</li>\r\n<li>Finally, the largest number in the left list is <code>4</code>, while the largest number in the right list is <code>9</code>; these are a distance <code><em>5</em></code> apart.</li>\r\n</ul>\r\n<p>To find the <em>total distance</em> between the left list and the right list, add up the distances between all of the pairs you found. In the example above, this is <code>2 + 1 + 0 + 1 + 2 + 5</code>, a total distance of <code><em>11</em></code>!</p>\r\n<p>Your actual left and right lists contain many location IDs. <em>What is the total distance between your lists?</em></p>\r\n</article>";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = Parse(input)
                        .Count(line => IsSafe(line, 0))
                        .ToString()
                }
            );
            
            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override string Description =>
            "<article class=\"day-desc\"><h2 id=\"part2\">--- Part Two ---</h2><p>The engineers are surprised by the low number of safe reports until they realize they forgot to tell you about the <span title=\"I need to get one of these!\">Problem Dampener</span>.</p>\n<p>The Problem Dampener is a reactor-mounted module that lets the reactor safety systems <em>tolerate a single bad level</em> in what would otherwise be a safe report. It's like the bad level never happened!</p>\n<p>Now, the same rules apply as before, except if removing a single level from an unsafe report would make it safe, the report instead counts as safe.</p>\n<p>More of the above example's reports are now safe:</p>\n<ul>\n<li><code>7 6 4 2 1</code>: <em>Safe</em> without removing any level.</li>\n<li><code>1 2 7 8 9</code>: <em>Unsafe</em> regardless of which level is removed.</li>\n<li><code>9 7 6 2 1</code>: <em>Unsafe</em> regardless of which level is removed.</li>\n<li><code>1 <em>3</em> 2 4 5</code>: <em>Safe</em> by removing the second level, <code>3</code>.</li>\n<li><code>8 6 <em>4</em> 4 1</code>: <em>Safe</em> by removing the third level, <code>4</code>.</li>\n<li><code>1 3 6 7 9</code>: <em>Safe</em> without removing any level.</li>\n</ul>\n<p>Thanks to the Problem Dampener, <code><em>4</em></code> reports are actually <em>safe</em>!</p>\n<p>Update your analysis by handling situations where the Problem Dampener can remove a single level from unsafe reports. <em>How many reports are now safe?</em></p>\n</article>";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.Report(
                new FinishedProblemUpdate
                {
                    Solution = Parse(input)
                        .Count(line => IsSafe(line, 1) || IsSafe(line.Reverse().ToArray(), 1))
                        .ToString()
                }
            );

            return Task.CompletedTask;
        }
    }

    private static List<int[]> Parse(string input) => input
        .Split('\n')
        .Select(
            line => line
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(int.Parse)
                .ToArray()
        )
        .Where(line => line.Length >= 2)
        .ToList();

    private static bool IsSafe(int[] numbers, int maxRemoves)
    {
        const int maxDelta = 3;
        var lastDelta = 0;
        var removes = 0;
        var last = numbers[0];

        for (var i = 1; i < numbers.Length; i++)
        {
            var delta = numbers[i] - last;
            var absoluteDelta = Math.Abs(delta);
            if (lastDelta != 0 && Math.Sign(lastDelta) != Math.Sign(delta) ||
                absoluteDelta < 1 ||
                absoluteDelta > maxDelta)
            {
                removes++;
            }
            else
            {
                last = numbers[i];
                lastDelta = delta;
            }
        }

        return removes <= maxRemoves;
    }
}