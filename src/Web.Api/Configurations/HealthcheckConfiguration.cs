using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Web.Api.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class HealthcheckConfiguration
    {
        public static void AddHealthcheckConfiguration(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddHealthChecks();

        }

        public static void UseHealthcheckSetup(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);

            // Executa TODOS os health checks
            app.MapHealthChecks("/healthcheck");

            // Executa apenas health checks do db
            app.MapHealthChecks("/healthcheck/db", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("db"),
                ResponseWriter = WriteHealthCheckResponse
            });

            // Executa health checks do app
            app.MapHealthChecks("/healthcheck/app", new HealthCheckOptions
            {
                Predicate = _ => false
            });
        }

        private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var response = new
            {
                status = result.Status.ToString(),
                checks = result.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    exception = entry.Value.Exception?.Message,
                    duration = entry.Value.Duration.ToString(),
                    description = entry.Value.Description,
                    data = entry.Value.Data
                }),
                totalDuration = result.TotalDuration
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }
    }
}
