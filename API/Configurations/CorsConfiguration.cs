using System.Diagnostics.CodeAnalysis;

namespace API.Configurations
{

    [ExcludeFromCodeCoverage]
    public static class CorsConfiguration
    {
        public static void AddCorsConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
        }

        public static void UseCorsSetup(this WebApplication app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseCors("AllowAllOrigins");
        }
    }
}
