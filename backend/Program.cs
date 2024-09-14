using Backend;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConnections()
    .AddAocPlatform()
    .AddControllers();

var app = builder.Build();

app.UseCors("allow_all");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();  