using System;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Problems.Year2025;

public static class Year2025Metadata
{
    public static IServiceCollection AddYear2025(this IServiceCollection services) =>
        services.AddSingleton<ProblemCollection>(new Year2025Collection());

    public class Year2025Collection : ProblemCollection
    {
        public override Type ProblemRootType { get; } = typeof(Year2025Metadata);
        public override string Source => "AoC";
        public override int Year => 2025;
    }
}

