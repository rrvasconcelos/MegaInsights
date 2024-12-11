using MI.Scraper.Services;

namespace MI.Scraper
{
    public partial class Worker(
        IServiceProvider serviceProvider,
        ILogger<Worker> logger,
        ILotteryScraper lotteryScraper)
        : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var results = await lotteryScraper.GetLotteryResultsAsync();
                
                logger.LogInformation("Quantidade de resultados encontrados: {Count}", results.Count());

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
