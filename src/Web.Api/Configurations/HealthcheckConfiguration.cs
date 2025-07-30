using System.Diagnostics.CodeAnalysis;

namespace API.Configurations
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

            app.MapHealthChecks("/healthcheck");
        }
    }
}
