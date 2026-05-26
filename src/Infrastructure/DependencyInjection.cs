using System.Diagnostics.CodeAnalysis;
using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.ExternalService;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration) =>
            services
                .AddServices(configuration)
                .AddDatabase(configuration)
                .AddHealthChecks(configuration);

        private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Repository
            services.AddScoped<IExampleRepository, ExampleRepository>();

            //External Service (typed client) — BaseAddress/Timeout vêm de Settings:Viacep
            var baseUrl = configuration["Settings:Viacep:BaseUrl"]
                ?? throw new InvalidOperationException("Configuration 'Settings:Viacep:BaseUrl' was not found.");
            var timeoutSeconds = int.TryParse(configuration["Settings:Viacep:TimeOut"], out var seconds) ? seconds : 30;

            services.AddHttpClient<IExampleService, ExampleService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            });

            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = GetConnectionString(configuration);

            services.AddDbContext<DataContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                ));

            return services;
        }

        private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddMySql(
                    connectionString: GetConnectionString(configuration),
                    name: "mysql-database",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "db", "mysql", "ready" },
                    timeout: TimeSpan.FromSeconds(30));

            return services;
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            return configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        }
    }
}
