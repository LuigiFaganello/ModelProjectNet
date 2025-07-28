using WorkerService;
using WorkerService.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Application;
using Infrastructure;
using Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile("appsettings.json", true, true)
   .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
   .AddEnvironmentVariables();

builder.Services
   .AddApplication()
   .AddInfrastructure(builder.Configuration);


builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Settings"));

var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);
builder.Services.AddQuartzJobsFromConfig(appSettings);

var host = builder.Build();
host.Run();
