namespace MI.Infra.Scraping.Configuration;

public class LotteryScraperOptions
{
    public int MaxDraws { get; init; } = 200;
    public int WaitTimeoutSeconds { get; init; } = 20;
    public int RetryAttempts { get; init; } = 3;
    public string LotteryUrl { get; init; } = string.Empty;
}