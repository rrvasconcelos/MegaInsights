using MI.Domain.Models;

namespace MI.Scraper.Services;

public interface ILotteryScraper : IDisposable
{
    Task<IEnumerable<LotteryResult>> GetLotteryResultsAsync(CancellationToken cancellationToken = default);
}