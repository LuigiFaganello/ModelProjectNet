using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public Viacep Viacep { get; set; }

        public List<QuartzJobConfig> QuartzJobs { get; set; }
    }

    public class Viacep
    {
        public string BaseUrl { get; set; }
        public int TimeOut { get; set; }
    }

    public class QuartzJobConfig
    {
        public string Name { get; set; }
        public string CronExpression { get; set; }
        public bool Active { get; set; }
    }
}
