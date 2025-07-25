using API.Configurations.Swagger;
using API.Configurations;
using CrossCutting.IoC;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

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


app.MapControllers();
app.UseCorsSetup();
app.UseAuthorization();
app.UseSwaggerSetup();
app.UseHttpsRedirection();
app.UseHealthcheckSetup();
app.UseStaticFiles();
app.Run();
app.MapOpenApi();
app.Run();
