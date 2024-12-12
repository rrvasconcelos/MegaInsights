using MI.Domain.Interfaces.Repositories;
using MI.Domain.Models;
using MI.Infra.Scraping.Interfaces;

namespace MI.Scraper;

public class Worker(
    IServiceProvider serviceProvider,
    ILogger<Worker> logger,
    ILotteryScraper lotteryScraper)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Processo iniciado.");

            var results = await lotteryScraper.GetLotteryResultsAsync(stoppingToken);

            await using var scope = serviceProvider.CreateAsyncScope();

            var repository = scope.ServiceProvider.GetRequiredService<ILotteryResultRepository>();

            var lotteryResults = results as LotteryResult[] ?? results.ToArray();
            
            logger.LogInformation("count results... {Length}", lotteryResults.Length);

            if (lotteryResults.Length == 0)
            {
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                return;
            }
            
            logger.LogInformation("registering results...");
            await repository.AddRangeAsync(lotteryResults, stoppingToken);

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}