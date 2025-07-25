using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.IoC
{

    [ExcludeFromCodeCoverage]
    public class NativeBootstrapInjector
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
        }
    }
}
