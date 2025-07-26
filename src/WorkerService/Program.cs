using WorkerService;
using WorkerService.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

var builder = Host.CreateApplicationBuilder(args);

var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);
builder.Services.AddQuartzJobsFromConfig(appSettings);

var host = builder.Build();
host.Run();
