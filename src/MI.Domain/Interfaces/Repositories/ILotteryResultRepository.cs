using MI.Domain.Models;

namespace MI.Domain.Interfaces.Repositories;

public interface ILotteryResultRepository
{
    Task AddRangeAsync(IEnumerable<LotteryResult> lotteryResult, CancellationToken stoppingToken);
}