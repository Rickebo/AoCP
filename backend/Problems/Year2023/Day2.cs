using System.Text.RegularExpressions;

namespace Backend.Problems.Year2023;

public class Day2 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2023, 12, 02, 0, 0, 0);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Cube Conundrum";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public async override IAsyncEnumerable<ProblemUpdate> Solve(string input)
        {
            var result = input
                .Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries)
                .Select(Game.Parse)
                .Where(game => game != null)
                .Where(game => game!.IsPossible(Game.Limits))
                .Sum(game => game!.Id)
                .ToString();

            yield return new FinishedProblemUpdate
            {
                Successful = true,
                Solution = result
            };
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public async override IAsyncEnumerable<ProblemUpdate> Solve(string input)
        {
            var result = input
                .Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries)
                .Select(Game.Parse)
                .Where(game => game != null)
                .Sum(game => game!.CalculatePower())
                .ToString();

            yield return new FinishedProblemUpdate()
            {
                Successful = true,
                Solution = result
            };
        }
    }

    public class Game
    {
        private static readonly Regex GameRegex = new(@"Game (\d+): (.*)");
        private static readonly Regex RollRegex = new(@"([\d \w+,]+)");
        private static readonly Regex CubeRegex = new(@"(\d+) (\w+)");

        public static Dictionary<string, int> Limits = new()
        {
            ["red"] = 12,
            ["green"] = 13,
            ["blue"] = 14
        };

        public int Id { get; init; }
        public Roll[] Rolls { get; init; }

        public static Game? Parse(string input)
        {
            var gameMatch = GameRegex.Match(input);
            if (!gameMatch.Success)
                return null;

            var gameId = int.Parse(gameMatch.Groups[1].Value);
            var rollMatches = RollRegex.Matches(gameMatch.Groups[2].Value);
            var rolls = new List<Roll>();

            foreach (Match roll in rollMatches)
            {
                var cubeMatches = CubeRegex.Matches(roll.Groups[1].Value);
                var cubes = new List<Cube>();

                foreach (Match cubeMatch in cubeMatches)
                    cubes.Add(
                        new Cube
                        {
                            Count = int.Parse(cubeMatch.Groups[1].Value),
                            Color = cubeMatch.Groups[2].Value,
                        }
                    );

                rolls.Add(
                    new Roll()
                    {
                        Cubes = cubes.ToArray()
                    }
                );
            }

            return new Game()
            {
                Id = gameId,
                Rolls = rolls.ToArray()
            };
        }

        public bool IsPossible(Dictionary<string, int> limits) =>
            Rolls.All(roll => roll.Cubes.All(cube => limits[cube.Color] >= cube.Count));

        public int CalculatePower()
        {
            var colors = new[] { "red", "green", "blue" };
            var power = 1;

            foreach (var color in colors)
                power *= Rolls.Max(roll => roll.CountMinimumCubes(color));

            return power;
        }
    }

    public class Roll
    {
        public Cube[] Cubes { get; init; }

        public int CountMinimumCubes(string color) =>
            Cubes.FirstOrDefault(cube => cube.Color == color)?.Count ?? 0;
    }

    public class Cube
    {
        public int Count { get; init; }
        public string Color { get; init; }
    }
}