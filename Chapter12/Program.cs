using Fundamentals;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var types = new Types();
builder.Services.AddSingleton<ITypes>(types);
builder.Services.AddBindingsByConvention(types);
builder.Services.AddSelfBinding(types);

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(_ => _.MapControllers());
app.Run();
