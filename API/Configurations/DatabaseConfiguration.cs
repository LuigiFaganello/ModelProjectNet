using System.Diagnostics.CodeAnalysis;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Configurations
{

    [ExcludeFromCodeCoverage]
    public static class DatabaseConfiguration
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration _configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (_configuration == null) throw new ArgumentNullException(nameof(_configuration));

            services.AddDbContext<DataContext>(options =>
                options.UseMySQL(_configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
