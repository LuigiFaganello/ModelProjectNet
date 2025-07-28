using Quartz;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Application.Services;

namespace WorkerService.Jobs
{
    public class ExampleSecondJob : IJob
    {
        private readonly ILogger<ExampleSecondJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        public ExampleSecondJob(ILogger<ExampleSecondJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var start = DateTime.Now;
            _logger.LogInformation($"[JOB START] {context.JobDetail.Key.Name} iniciado em: {start:yyyy-MM-dd HH:mm:ss}");

            // Simulação de trabalho real
            await Task.Delay(500); // Remova ou ajuste conforme necessário

            stopwatch.Stop();
            var end = DateTime.Now;
            _logger.LogInformation($"[JOB END] {context.JobDetail.Key.Name} finalizado em: {end:yyyy-MM-dd HH:mm:ss} | Tempo de execução: {stopwatch.Elapsed.TotalSeconds:F2} segundos");
        }
    }
} 