using MI.Domain.Interfaces.Repositories;
using MI.Domain.Models;
using MI.Infra.Scraping.Interfaces;

namespace MI.Scraper;

public class Worker(
    IServiceProvider serviceProvider,
    ILogger<Worker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Processo iniciado.");

            await using var scope = serviceProvider.CreateAsyncScope();

            var repository = scope.ServiceProvider.GetRequiredService<ILotteryResultRepository>();
            var lotteryScraper = scope.ServiceProvider.GetRequiredService<ILotteryScraper>();

            var results = await lotteryScraper.GetLotteryResultsAsync(stoppingToken);

            var lotteryResults = results as LotteryResult[] ?? results.ToArray();
            
            logger.LogInformation("count results... {Length}", lotteryResults.Length);

            if (lotteryResults.Length == 0)
            {
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                return;
            }
            
            logger.LogInformation("registering results...");

            await repository.AddRangeAsync(lotteryResults, stoppingToken);

            logger.LogInformation("Aguardando proxima execução.");

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}