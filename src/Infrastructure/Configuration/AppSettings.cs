using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public Viacep Viacep { get; set; }
    }

    public class Viacep
    {
        public string BaseUrl { get; set; }
        public int TimeOut { get; set; }
    }
}
