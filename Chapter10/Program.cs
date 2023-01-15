using Chapter10.Structured;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddSingleton<IDatabase, Database>();
builder.Services.AddSingleton<IUsersService, UsersService>();
builder.Services.AddSingleton<IUserDetailsService, UserDetailsService>();

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(_ => _.MapControllers());
app.Run();
