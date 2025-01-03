using EFCore.BulkExtensions;
using MI.Domain.Interfaces.Repositories;
using MI.Domain.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task<LotteryResult?> GetLastResultAsync(CancellationToken stoppingToken)
    {
        try
        {
            return await context.LotteryResults
                .AsNoTracking()
                .OrderByDescending(e => e.DrawDate)
                .FirstOrDefaultAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}