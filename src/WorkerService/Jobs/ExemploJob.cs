using Quartz;
using Microsoft.Extensions.Logging;

namespace WorkerService.Jobs
{
    public class ExemploJob : IJob
    {
        private readonly ILogger<ExemploJob> _logger;
        public ExemploJob(ILogger<ExemploJob> logger)
        {
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"ExemploJob executado em: {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
} 