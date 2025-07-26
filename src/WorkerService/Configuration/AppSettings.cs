using System.Collections.Generic;

namespace WorkerService.Configuration
{
    public class AppSettings
    {
        public List<QuartzJobConfig> QuartzJobs { get; set; }
    }
} 