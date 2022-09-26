using Chapter3;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(mvcOptions => mvcOptions.Filters.Add<ValidationFilter>());
var app = builder.Build();
app.MapControllers();
app.Run();
