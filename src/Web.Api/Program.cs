using API.Configurations;
using API.Configurations.Swagger;
using Application;
using Infrastructure;
using Infrastructure.Configuration;
using Web.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile("appsettings.json", true, true)
   .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
   .AddEnvironmentVariables();  

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
app.MapControllers();
app.UseCorsSetup();
app.UseAuthorization();
app.UseSwaggerSetup();
app.UseHttpsRedirection();
app.UseHealthcheckSetup();
app.UseStaticFiles();
app.MapOpenApi();
app.Run();
