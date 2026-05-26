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

builder.Services.AddCorsConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddHealthcheckConfiguration();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Settings"));

var app = builder.Build();

// Ordem do pipeline (importa): correlação e tratamento de exceção primeiro para
// abranger todo o restante; depois infra de requisição; CORS antes de autorização;
// endpoints por último.
app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCorsSetup();
app.UseAuthorization();
app.UseSwaggerSetup();
app.UseHealthcheckSetup();
app.MapControllers();
app.MapOpenApi();
app.Run();
