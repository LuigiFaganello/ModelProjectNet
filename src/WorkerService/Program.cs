using WorkerService;
using WorkerService.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddQuartzJobsFromConfig(builder.Configuration);

var host = builder.Build();
host.Run();
