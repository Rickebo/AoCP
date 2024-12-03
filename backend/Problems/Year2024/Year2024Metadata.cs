namespace Backend.Problems.Year2024;

public static class Year2024Metadata
{
    public static IServiceCollection AddYear2024(this IServiceCollection services) =>
        services.AddSingleton<ProblemCollection>(new Year2024Collection());

    public class Year2024Collection : ProblemCollection
    {
        public override int Year { get; } = 2024;

        public override List<ProblemSet> Problems { get; } =
        [
            new Day01(),
            new Day02(),
            new Day03(),
            new Day04(),
            new Day05(),
            new Day06(),
            new Day07(),
            new Day08(),
            new Day09(),
            new Day10()
        ];
    }
}