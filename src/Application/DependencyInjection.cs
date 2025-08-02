using System.Diagnostics.CodeAnalysis;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IExampleAppService, ExampleAppService>();

            return services;
        }
    }
}
