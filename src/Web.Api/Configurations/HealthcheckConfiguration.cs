using System.Diagnostics.CodeAnalysis;

namespace API.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class HealthcheckConfiguration
    {
        public static void AddHealthcheckConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHealthChecks();

        }

        public static void UseHealthcheckSetup(this WebApplication app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.MapHealthChecks("/healthcheck");
        }
    }
}
