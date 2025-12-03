using System;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Problems.Year2024;

public static class Year2024Metadata
{
    public static IServiceCollection AddYear2024(this IServiceCollection services) =>
        services.AddSingleton<ProblemCollection>(new Year2024Collection());

    public class Year2024Collection : ProblemCollection
    {
        public override Type ProblemRootType { get; } = typeof(Year2024Metadata);
        public override string Source => "AoC";
        public override int Year => 2024;
    }
}

