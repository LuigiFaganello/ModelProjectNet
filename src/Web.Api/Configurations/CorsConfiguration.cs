using System.Diagnostics.CodeAnalysis;

namespace Web.Api.Configurations
{

    [ExcludeFromCodeCoverage]
    public static class CorsConfiguration
    {
        public static void AddCorsConfiguration(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

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
            ArgumentNullException.ThrowIfNull(app);

            app.UseCors("AllowAllOrigins");
        }
    }
}
