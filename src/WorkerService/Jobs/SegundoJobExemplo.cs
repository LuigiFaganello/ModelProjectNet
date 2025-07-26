using Quartz;
using Microsoft.Extensions.Logging;

namespace WorkerService.Jobs
{
    public class SegundoJobExemplo : IJob
    {
        private readonly ILogger<SegundoJobExemplo> _logger;
        public SegundoJobExemplo(ILogger<SegundoJobExemplo> logger)
        {
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"SegundoJobExemplo executado em: {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
} 