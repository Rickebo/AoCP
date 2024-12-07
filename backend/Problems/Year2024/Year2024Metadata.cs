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
            new Day1(),
            new Day2(),
            new Day3(),
            new Day4(),
            new Day5(),
            new Day6(),
            new Day7()
        ];
    }
}