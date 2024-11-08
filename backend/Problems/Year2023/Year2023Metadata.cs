namespace Backend.Problems.Year2023;

public static class Year2023Metadata
{
    public static IServiceCollection AddYear2023(this IServiceCollection services) =>
        services.AddSingleton<ProblemCollection>(new Year2023Collection());

    public class Year2023Collection : ProblemCollection
    {
        public override int Year { get; } = 2023;

        public override List<ProblemSet> Problems { get; } =
        [
            new Day1(),
            new Day2()
        ];
    }
}