using Quartz;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace WorkerService.Jobs
{
    public class ExemploJob : IJob
    {
        private readonly ILogger<ExemploJob> _logger;
        public ExemploJob(ILogger<ExemploJob> logger)
        {
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var jobName = context.JobDetail.Key.Name;
            var start = DateTime.Now;
            _logger.LogInformation($"[JOB START] {jobName} iniciado em: {start:yyyy-MM-dd HH:mm:ss}");
            var stopwatch = Stopwatch.StartNew();

            // Simulação de trabalho real
            await Task.Delay(500); // Remova ou ajuste conforme necessário

            stopwatch.Stop();
            var end = DateTime.Now;
            _logger.LogInformation($"[JOB END] {jobName} finalizado em: {end:yyyy-MM-dd HH:mm:ss} | Tempo de execução: {stopwatch.Elapsed.TotalSeconds:F2} segundos");
        }
    }
} 