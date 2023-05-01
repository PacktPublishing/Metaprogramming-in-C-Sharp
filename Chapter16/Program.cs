using System.Diagnostics.Metrics;
using Fundamentals.Metrics;

GlobalMetrics.Meter = new Meter("Chapter16");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.Run();
