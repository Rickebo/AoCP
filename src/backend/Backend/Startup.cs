using Backend.Problems.Year2023;
using Backend.Problems.Year2024;
using Backend.Problems.Year2025Codelight;
using Backend.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Backend;

public static class Startup
{
    public static IServiceCollection AddAocPlatform(this IServiceCollection services) =>
        services
            .AddCors(
                options =>
                {
                    var policy = new CorsPolicy();
                    policy.Origins.Add("*");
                    policy.Methods.Add("*");
                    
                    options.AddPolicy("allow_all", policy);
                }
            )
            .AddSingleton<ProblemService>()
            .AddHostedService<ProblemLoaderService>()
            .AddYear2023()
            .AddYear2024()
            .AddYear2025Codelight();
}