using Quartz;
using System.Diagnostics;
using Application.Services;

namespace WorkerService.Jobs
{
    public class ExampleJob : IJob
    {
        private readonly ILogger<ExampleJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        public ExampleJob(ILogger<ExampleJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var start = DateTime.Now;
            _logger.LogInformation($"[JOB START] {context.JobDetail.Key.Name} iniciado em: {start:yyyy-MM-dd HH:mm:ss}");

            using var scope = _serviceProvider.CreateScope();
            var exampleAppService = scope.ServiceProvider.GetRequiredService<IExampleAppService>();

            await exampleAppService.SyncCity(context.CancellationToken);

            stopwatch.Stop();
            var end = DateTime.Now;
            _logger.LogInformation($"[JOB END] {context.JobDetail.Key.Name} finalizado em: {end:yyyy-MM-dd HH:mm:ss} | Tempo de execução: {stopwatch.Elapsed.TotalSeconds:F2} segundos");
        }
    }
} 