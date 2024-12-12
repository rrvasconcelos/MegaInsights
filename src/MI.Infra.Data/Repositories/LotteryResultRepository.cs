using EFCore.BulkExtensions;
using MI.Domain.Interfaces.Repositories;
using MI.Domain.Models;

namespace MI.Infra.Data.Repositories;

public class LotteryResultRepository(MegaInsightsContext context) : ILotteryResultRepository
{
    public async Task AddRangeAsync(IEnumerable<LotteryResult> lotteryResult, CancellationToken stoppingToken)
    {
        try
        {
            await context.BulkInsertAsync(lotteryResult, cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}