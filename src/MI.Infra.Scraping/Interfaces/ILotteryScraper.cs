using MI.Domain.Models;

namespace MI.Infra.Scraping.Interfaces;

public interface ILotteryScraper : IDisposable
{
    Task<IEnumerable<LotteryResult>> GetLotteryResultsAsync(CancellationToken cancellationToken = default);
}