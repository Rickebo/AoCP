using System;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Problems.Year2023;

public static class Year2023Metadata
{
    public static IServiceCollection AddYear2023(this IServiceCollection services) =>
        services.AddSingleton<ProblemCollection>(new Year2023Collection());

    public class Year2023Collection : ProblemCollection
    {
        public override Type ProblemRootType { get; } = typeof(Year2023Metadata);
        public override string Source => "AoC";
        public override int Year => 2023;
    }
}
