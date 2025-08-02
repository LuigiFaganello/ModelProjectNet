using Application;
using Infrastructure;
using Infrastructure.Configuration;
using Serilog;
using Web.Api.Configurations;
using Web.Api.Configurations.Swagger;
using Web.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCorsConfiguration();
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddHealthcheckConfiguration();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Settings"));

var app = builder.Build();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapControllers();
app.UseCorsSetup();
app.UseAuthorization();
app.UseSwaggerSetup();
app.UseHttpsRedirection();
app.UseHealthcheckSetup();
app.UseStaticFiles();
app.MapOpenApi();
app.Run();
