using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public Viacep Viacep { get; set; } = new();
    }

    public class Viacep
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int TimeOut { get; set; }
    }
}
