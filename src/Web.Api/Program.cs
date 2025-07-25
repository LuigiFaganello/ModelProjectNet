using API.Configurations.Swagger;
using API.Configurations;
using CrossCutting.IoC;
using Serilog;
using Web.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Debug()
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Properties:j} {Message:lj}{NewLine}{Exception}")
    .CreateLogger());

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Services.AddCorsConfiguration();
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddHealthcheckConfiguration();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

NativeBootstrapInjector.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseRequestContextLogging();
app.MapControllers();
app.UseCorsSetup();
app.UseAuthorization();
app.UseSwaggerSetup();
app.UseHttpsRedirection();
app.UseHealthcheckSetup();
app.UseStaticFiles();
app.MapOpenApi();
app.Run();
