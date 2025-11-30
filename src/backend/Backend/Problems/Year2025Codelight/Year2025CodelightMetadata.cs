using System;
using Backend.Problems.Year2024;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Problems.Year2025Codelight;

public static class Year2025CodelightMetadata
{
    public static IServiceCollection AddYear2025Codelight(this IServiceCollection services) =>
        services.AddSingleton<ProblemCollection>(new Year2025CodelightCollection());

    public class Year2025CodelightCollection : ProblemCollection
    {
        public override Type ProblemRootType { get; } = typeof(Year2025CodelightMetadata);
        public override string Source => "Codelight";
        public override int Year => 2025;
    }
}
