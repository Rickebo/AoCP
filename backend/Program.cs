using Backend;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConnections()
    .AddAocPlatform()
    .AddControllers();

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

app.UseHttpsRedirection();

app.MapControllers();

app.Run();