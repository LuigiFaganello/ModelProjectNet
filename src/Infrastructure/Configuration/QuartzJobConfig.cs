namespace Infrastructure.Configuration
{
    public class QuartzJobConfig
    {
        public string Name { get; set; }
        public string CronExpression { get; set; }
        public bool Active { get; set; }
    }
} 