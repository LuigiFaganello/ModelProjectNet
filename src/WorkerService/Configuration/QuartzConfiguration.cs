using System.Reflection;
using Quartz;

namespace WorkerService.Configuration
{
    public static class QuartzConfiguration
    {
        public static IServiceCollection AddQuartzJobsFromConfig(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddQuartz(q =>
            {
                var config = appSettings.QuartzJobs;
                if (config == null) return;

                // Busca todos os tipos que implementam IJob no assembly atual
                var jobTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => typeof(IJob).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    .ToDictionary(t => t.Name, t => t);

                foreach (var job in config)
                {
                    if (!job.Active) continue;
                    if (!jobTypes.TryGetValue(job.Name, out var jobType))
                        continue; // Ignora se não encontrar a classe

                    var jobKey = new JobKey(job.Name);
                    q.AddJob(jobType, jobKey);

                    q.AddTrigger(opts => opts
                        .ForJob(jobKey)
                        .WithIdentity($"{job.Name}.trigger")
                        .WithCronSchedule(job.CronExpression));
                }
            });
            services.AddQuartzHostedService();
            return services;
        }
    }
} 