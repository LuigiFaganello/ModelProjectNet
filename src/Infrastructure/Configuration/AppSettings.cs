using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configuration
{
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
