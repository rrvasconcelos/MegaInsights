using MI.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MI.Infra.Data;

public class MegaInsightsContext(DbContextOptions<MegaInsightsContext> options) 
    : DbContext(options)
{
    public DbSet<LotteryResult> LotteryResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LotteryResult>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<LotteryResult>()
            .ToTable("LotteryResults");
    }
}