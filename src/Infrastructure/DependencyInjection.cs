using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.ExternalService;
using Infrastructure.ExternalService.Interface;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration) =>
            services
                .AddServices()
                .AddDatabase(configuration)
                .AddHealthChecks(configuration);

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            //Config
            services.AddHttpClient();

            //Repository
            services.AddScoped<IExampleRepository, ExampleRepository>();

            //External Service
            services.AddScoped<IExampleService, ExampleService>();

            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

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
                    connectionString: configuration.GetConnectionString("DefaultConnection"),
                    name: "mysql-database",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "db", "mysql", "ready" },
                    timeout: TimeSpan.FromSeconds(30));

            return services;
        }
    }
}
