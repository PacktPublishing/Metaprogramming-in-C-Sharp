using Chapter13;
using Chapter13.Commands;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(mvcOptions => mvcOptions.Filters.Add<CommandActionFilter>());

builder.Services.AddAuthorization(options => options.AddPolicy("Chapter13Admins", policy => policy.Requirements.Add(new AdminForNamespace("Chapter13"))));
builder.Services.AddSingleton<IAuthorizationHandler, AdminForNamespaceHandler>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CrossCuttingAuthorizationMiddlewareResultHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CrossCuttingPoliciesProvider>();
builder.Services
    .AddAuthentication(options => options.DefaultScheme = HardCodedAuthenticationHandler.SchemeName)
    .AddScheme<HardCodedAuthenticationOptions, HardCodedAuthenticationHandler>(HardCodedAuthenticationHandler.SchemeName, _ => {});

var app = builder.Build();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
