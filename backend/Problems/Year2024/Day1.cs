using Common.Updates;

namespace Backend.Problems.Year2024;

public class Day1 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 01, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Historian Hysteria";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "<article class=\"day-desc\"><p>The <em>Chief Historian</em> is always present for the big Christmas sleigh launch, but nobody has seen him in months! Last anyone heard, he was visiting locations that are historically significant to the North Pole; a group of Senior Historians has asked you to accompany them as they check the places they think he was most likely to visit.</p>\r\n<p>As each location is checked, they will mark it on their list with a <em class=\"star\">star</em>. They figure the Chief Historian <em>must</em> be in one of the first fifty places they'll look, so in order to save Christmas, you need to help them get <em class=\"star\">fifty stars</em> on their list before Santa takes off on December 25th.</p>\r\n<p>Collect stars by solving puzzles.  Two puzzles will be made available on each day in the Advent calendar; the second puzzle is unlocked when you complete the first.  Each puzzle grants <em class=\"star\">one star</em>. Good luck!</p>\r\n<p>You haven't even left yet and the group of Elvish Senior Historians has already hit a problem: their list of locations to check is currently <em>empty</em>. Eventually, someone decides that the best place to check first would be the Chief Historian's office.</p>\r\n<p>Upon pouring into the office, everyone confirms that the Chief Historian is indeed nowhere to be found. Instead, the Elves discover an assortment of notes and lists of historically significant locations! This seems to be the planning the Chief Historian was doing before he left. Perhaps these notes can be used to determine which locations to search?</p>\r\n<p>Throughout the Chief's office, the historically significant locations are listed not by name but by a unique number called the <em>location ID</em>. To make sure they don't miss anything, The Historians split into two groups, each searching the office and trying to create their own complete list of location IDs.</p>\r\n<p>There's just one problem: by holding the two lists up <em>side by side</em> (your puzzle input), it quickly becomes clear that the lists aren't very similar. Maybe you can help The Historians reconcile their lists?</p>\r\n<p>For example:</p>\r\n<pre><code>3   4\r\n4   3\r\n2   5\r\n1   3\r\n3   9\r\n3   3\r\n</code></pre>\r\n<p>Maybe the lists are only off by a small amount! To find out, pair up the numbers and measure how far apart they are. Pair up the <em>smallest number in the left list</em> with the <em>smallest number in the right list</em>, then the <em>second-smallest left number</em> with the <em>second-smallest right number</em>, and so on.</p>\r\n<p>Within each pair, figure out <em>how far apart</em> the two numbers are; you'll need to <em>add up all of those distances</em>. For example, if you pair up a <code>3</code> from the left list with a <code>7</code> from the right list, the distance apart is <code>4</code>; if you pair up a <code>9</code> with a <code>3</code>, the distance apart is <code>6</code>.</p>\r\n<p>In the example list above, the pairs and distances would be as follows:</p>\r\n<ul>\r\n<li>The smallest number in the left list is <code>1</code>, and the smallest number in the right list is <code>3</code>. The distance between them is <code><em>2</em></code>.</li>\r\n<li>The second-smallest number in the left list is <code>2</code>, and the second-smallest number in the right list is another <code>3</code>. The distance between them is <code><em>1</em></code>.</li>\r\n<li>The third-smallest number in both lists is <code>3</code>, so the distance between them is <code><em>0</em></code>.</li>\r\n<li>The next numbers to pair up are <code>3</code> and <code>4</code>, a distance of <code><em>1</em></code>.</li>\r\n<li>The fifth-smallest numbers in each list are <code>3</code> and <code>5</code>, a distance of <code><em>2</em></code>.</li>\r\n<li>Finally, the largest number in the left list is <code>4</code>, while the largest number in the right list is <code>9</code>; these are a distance <code><em>5</em></code> apart.</li>\r\n</ul>\r\n<p>To find the <em>total distance</em> between the left list and the right list, add up the distances between all of the pairs you found. In the example above, this is <code>2 + 1 + 0 + 1 + 2 + 5</code>, a total distance of <code><em>11</em></code>!</p>\r\n<p>Your actual left and right lists contain many location IDs. <em>What is the total distance between your lists?</em></p>\r\n</article>";

        public override async Task Solve(string input, Reporter reporter)
        {
            List<int> left = [];
            List<int> right = [];
            foreach (var line in input.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries))
            {
                List<int> numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                left.Add(numbers[0]);
                right.Add(numbers[1]);
            }

            left.Sort();
            right.Sort();

            int diff = 0;

            for (int i = 0; i < left.Count; i++)
            {
                int currDiff = Math.Abs(left[i] - right[i]);

                reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [$"|{left[i]} - {right[i]}| = {currDiff}"]
                    }
                );

                diff += currDiff;
            }

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = diff.ToString()
                }
            );
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "<article class=\"day-desc\"><p>Your analysis only confirmed what everyone feared: the two lists of location IDs are indeed very different.</p>\r\n<p>Or are they?</p>\r\n<p>The Historians can't agree on which group made the mistakes <em>or</em> how to read most of the Chief's handwriting, but in the commotion you notice an interesting detail: <span title=\"We were THIS close to summoning the Alot of Location IDs!\">a lot</span> of location IDs appear in both lists! Maybe the other numbers aren't location IDs at all but rather misinterpreted handwriting.</p>\r\n<p>This time, you'll need to figure out exactly how often each number from the left list appears in the right list. Calculate a total <em>similarity score</em> by adding up each number in the left list after multiplying it by the number of times that number appears in the right list.</p>\r\n<p>Here are the same example lists again:</p>\r\n<pre><code>3   4\r\n4   3\r\n2   5\r\n1   3\r\n3   9\r\n3   3\r\n</code></pre>\r\n<p>For these example lists, here is the process of finding the similarity score:</p>\r\n<ul>\r\n<li>The first number in the left list is <code>3</code>. It appears in the right list three times, so the similarity score increases by <code>3 * 3 = <em>9</em></code>.</li>\r\n<li>The second number in the left list is <code>4</code>. It appears in the right list once, so the similarity score increases by <code>4 * 1 = <em>4</em></code>.</li>\r\n<li>The third number in the left list is <code>2</code>. It does not appear in the right list, so the similarity score does not increase (<code>2 * 0 = 0</code>).</li>\r\n<li>The fourth number, <code>1</code>, also does not appear in the right list.</li>\r\n<li>The fifth number, <code>3</code>, appears in the right list three times; the similarity score increases by <code><em>9</em></code>.</li>\r\n<li>The last number, <code>3</code>, appears in the right list three times; the similarity score again increases by <code><em>9</em></code>.</li>\r\n</ul>\r\n<p>So, for these example lists, the similarity score at the end of this process is <code><em>31</em></code> (<code>9 + 4 + 0 + 0 + 9 + 9</code>).</p>\r\n<p>Once again consider your left and right lists. <em>What is their similarity score?</em></p>\r\n</article>";

        public override async Task Solve(
            string input,
            Reporter reporter
        )
        {
            List<int> left = [];
            List<int> right = [];
            foreach (var line in input.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries))
            {
                List<int> numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                left.Add(numbers[0]);
                right.Add(numbers[1]);
            }

            int sim = 0;

            for (int i = 0; i < left.Count; i++)
            {
                int multiplier = right.Where(x => x.Equals(left[i])).Count();
                int currSim = left[i] * multiplier;

                reporter.Report(
                    new TextProblemUpdate()
                    {
                        Lines = [$"{left[i]} * {multiplier} = {currSim}"]
                    }
                );

                sim += currSim;
            }

            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Successful = true,
                    Solution = sim.ToString()
                }
            );
        }
    }
}