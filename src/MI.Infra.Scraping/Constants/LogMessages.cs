using Microsoft.Extensions.Logging;

namespace MI.Infra.Scraping.Constants;

public static class LogMessages
{
    public static readonly EventId ScrapingStarted = new(100, "ScrapingStarted");
    public static readonly EventId ScrapingCompleted = new(101, "ScrapingCompleted");
    public static readonly EventId ScrapingError = new(102, "ScrapingError");
    public static readonly EventId DrawProcessed = new(103, "DrawProcessed");
}