using System.Text.Json;
using Backend;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConnections()
    .AddAocPlatform()
    .AddControllers()
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy =
                JsonNamingPolicy.CamelCase;
        }
    );

var corsName = "allow_all";
builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            name: corsName,
            policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
        );
    }
);

var app = builder.Build();

app.UseCors(corsName);
app.UseWebSockets();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();