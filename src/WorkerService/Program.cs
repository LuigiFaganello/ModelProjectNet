using Application;
using Infrastructure;
using Infrastructure.Configuration;
using Serilog;
using WorkerService.Configuration;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up Worker Service");

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Configuration
       .SetBasePath(builder.Environment.ContentRootPath)
       .AddJsonFile("appsettings.json", true, true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
       .AddEnvironmentVariables();

    builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services
       .AddApplication()
       .AddInfrastructure(builder.Configuration);

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Settings"));

    var appSettings = new AppSettings();
    builder.Configuration.Bind(appSettings);
    builder.Services.AddQuartzJobsFromConfig(appSettings);

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
