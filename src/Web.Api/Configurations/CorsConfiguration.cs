using System.Diagnostics.CodeAnalysis;

namespace Web.Api.Configurations
{

    [ExcludeFromCodeCoverage]
    public static class CorsConfiguration
    {
        private const string PolicyName = "DefaultCorsPolicy";

        public static void AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            // Origens permitidas via configuração (Cors:AllowedOrigins). Se vazio, cai no
            // modo permissivo — adequado apenas para desenvolvimento. Restrinja por ambiente.
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, builder =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        builder
                            .WithOrigins(allowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                    else
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                });
            });
        }

        public static void UseCorsSetup(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.UseCors(PolicyName);
        }
    }
}
