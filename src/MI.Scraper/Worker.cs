using EFCore.BulkExtensions;
using MI.Domain.Models;
using MI.Infra.Data;
using MI.Scraper.Services;

namespace MI.Scraper
{
    public class Worker(
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
                var results = await lotteryScraper.GetLotteryResultsAsync(stoppingToken);

                await using var scope = _serviceProvider.CreateAsyncScope();

                var context = scope.ServiceProvider.GetRequiredService<MegaInsightsContext>();

                var lotteryResults = results as LotteryResult[] ?? results.ToArray();
                
                await context.BulkInsertAsync(lotteryResults, cancellationToken: stoppingToken);

                logger.LogInformation("Quantidade de resultados encontrados: {Length}", lotteryResults.Length);

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}