using System.Collections.ObjectModel;
using MI.Domain.Models;
using MI.Scraper.Configuration;
using MI.Scraper.Exceptions;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Polly;
using Polly.CircuitBreaker;

namespace MI.Scraper.Services;

public sealed class LotteryScraper : ILotteryScraper
{
    private readonly ILogger<LotteryScraper> _logger;
    private readonly IWebDriver _driver;
    private readonly LotteryScraperOptions _options;
    private readonly IAsyncPolicy<IWebElement> _elementPolicy;
    private readonly IAsyncPolicy<ReadOnlyCollection<IWebElement>> _elementsPolicy;
    private bool _disposed;

    public LotteryScraper(ILogger<LotteryScraper> logger, IWebDriver driver, IOptions<LotteryScraperOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));

        // Element Policy
        _elementPolicy = Policy<IWebElement>
            .Handle<WebDriverException>()
            .Or<NoSuchElementException>()
            .WaitAndRetryAsync(
                _options.RetryAttempts,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, duration, retryCount, _) =>
                {
                    _logger.LogWarning(
                        exception.Exception,
                        "Attempt {RetryCount} to find element failed. Waiting {Duration} seconds before next attempt",
                        retryCount,
                        duration.TotalSeconds);
                    return Task.CompletedTask;
                });

        // Elements Policy
        _elementsPolicy = Policy<ReadOnlyCollection<IWebElement>>
            .Handle<WebDriverException>()
            .Or<NoSuchElementException>()
            .WaitAndRetryAsync(
                _options.RetryAttempts,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, duration, retryCount, _) =>
                {
                    _logger.LogWarning(
                        exception.Exception,
                        "Attempt {RetryCount} to find elements failed. Waiting {Duration} seconds before next attempt",
                        retryCount,
                        duration.TotalSeconds);
                    return Task.CompletedTask;
                });
    }

    public async Task<IEnumerable<LotteryResult>> GetLotteryResultsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(LogMessages.ScrapingStarted, "Starting lottery scraping process");
            cancellationToken.ThrowIfCancellationRequested();

            await _driver.Navigate().GoToUrlAsync(_options.LotteryUrl);

            _logger.LogInformation("Page Title: {Title}", _driver.Title);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(_options.WaitTimeoutSeconds));
            var results = new List<LotteryResult>();

            await ScrapeLotteryDataAsync(wait, results, cancellationToken);

            _logger.LogInformation(LogMessages.ScrapingCompleted,
                "Scraping completed successfully. Total results: {Count}", results.Count);

            return results;
        }
        catch (WebDriverTimeoutException ex)
        {
            _logger.LogError(LogMessages.ScrapingError, ex, "Timeout waiting for page update.");
            throw new ScrapingException("Timeout waiting for page update", ex);
        }
        catch (NoSuchElementException ex)
        {
            _logger.LogError(LogMessages.ScrapingError, ex, "Element not found. Check the selector.");
            throw new ScrapingException("Element not found. Check the selector", ex);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(LogMessages.ScrapingError, ex, "Circuit breaker is open. Too many failures occurred.");
            throw new ScrapingException("Circuit breaker is open. Too many failures occurred", ex);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(LogMessages.ScrapingError, ex, "An unexpected error occurred during scraping.");
            throw new ScrapingException("An unexpected error occurred during scraping", ex);
        }
    }

    private async Task ScrapeLotteryDataAsync(WebDriverWait wait, List<LotteryResult> results,
        CancellationToken cancellationToken)
    {
        try
        {
            var count = 0;
            while (count < _options.MaxDraws && !cancellationToken.IsCancellationRequested)
            {
                var drawElement =
                    await WaitForElementWithRetryAsync(By.CssSelector(Selectors.DrawTitle), wait, cancellationToken);

                var previousText = drawElement.Text;
                _logger.LogInformation(LogMessages.DrawProcessed, "Processing Draw: {Text}", previousText);

                var drawData = GetDrawData(drawElement);
                var numbers = await CaptureDrawNumbers(wait, cancellationToken);
                LogDrawInfo(drawData.drawNumber, numbers);

                var accumulated = await GetAccumulatedValue(wait, cancellationToken);

                results.Add(new LotteryResult(drawData.drawDate, drawData.drawNumber, accumulated, numbers));

                await NavigateToPreviousPage(wait, cancellationToken);
                await WaitForNextDrawAsync(wait, previousText, cancellationToken);

                count++;
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Scraping operation was cancelled");
            throw;
        }
    }

    private async Task<decimal> GetAccumulatedValue(WebDriverWait wait, CancellationToken cancellationToken)
    {
        try
        {
            var accumulated = await WaitForElementWithRetryAsync(
                By.CssSelector(Selectors.AccumulatedValue),
                wait,
                cancellationToken);

            var valueText = accumulated.Text.Replace("R$", "").Trim();

            if (decimal.TryParse(valueText, out var value))
                return value;

            _logger.LogWarning("Unable to convert accumulated value: {ValueText}", valueText);
            return 0;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation cancelled while getting accumulated value");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting accumulated value");
            return 0;
        }
    }

    private (int drawNumber, DateOnly drawDate) GetDrawData(IWebElement? element)
    {
        Guard.AgainstNullOrEmpty(element?.Text, nameof(element));

        var data = element!.Text.Split(" ");
        if (data.Length < 3)
            throw new ScrapingException($"Invalid draw data format: {element.Text}");

        var dateString = data[2].Trim('(', ')');
        if (!DateOnly.TryParse(dateString, out var drawDate))
        {
            _logger.LogError("Failed to convert date: {DateString}", dateString);
            throw new ScrapingException($"Invalid date format: {dateString}");
        }

        if (int.TryParse(data[1], out var drawNumber))
            return (drawNumber, drawDate);

        throw new ScrapingException($"Invalid draw number format: {data[1]}");
    }

    private async Task<IReadOnlyList<int>> CaptureDrawNumbers(WebDriverWait wait, CancellationToken cancellationToken)
    {
        try
        {
            var numberElements = await WaitForElementsWithRetryAsync(
                By.CssSelector(Selectors.DrawNumbers),
                wait,
                cancellationToken);

            return numberElements.Select(number => Convert.ToInt32(number.Text)).ToList();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation cancelled while capturing numbers");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error capturing draw numbers");
            throw new ScrapingException("Failed to capture draw numbers", ex);
        }
    }

    private async Task NavigateToPreviousPage(WebDriverWait wait, CancellationToken cancellationToken)
    {
        try
        {
            var buttonElement = await WaitForElementWithRetryAsync(
                By.XPath(Selectors.PreviousButton),
                wait,
                cancellationToken);

            buttonElement.Click();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation cancelled while navigating to previous page");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to previous page");
            throw new ScrapingException("Failed to navigate to previous page", ex);
        }
    }

    private async Task WaitForNextDrawAsync(WebDriverWait wait, string previousText,
        CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            return wait.Until(d =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var updatedDrawElement = d.FindElement(By.CssSelector(Selectors.DrawTitle));
                return updatedDrawElement.Text != previousText;
            });
        }, cancellationToken);
    }

    private void LogDrawInfo(int drawNumber, IEnumerable<int> numbers)
    {
        _logger.LogInformation(LogMessages.DrawProcessed,
            "Draw: {DrawNumber} - Numbers: {Numbers}",
            drawNumber,
            string.Join(", ", numbers));
    }

    private async Task<IWebElement> WaitForElementWithRetryAsync(By by, WebDriverWait wait,
        CancellationToken cancellationToken)
    {
        return await _elementPolicy.ExecuteAsync(async (ct) =>
            await WaitForElementAsync(by, wait, ct), cancellationToken);
    }

    private async Task<ReadOnlyCollection<IWebElement>> WaitForElementsWithRetryAsync(
        By by,
        WebDriverWait wait,
        CancellationToken cancellationToken)
    {
        return await _elementsPolicy.ExecuteAsync(async (ct) =>
                await WaitForElementsAsync(by, wait, ct) ??
                new ReadOnlyCollection<IWebElement>(new List<IWebElement>()),
            cancellationToken);
    }

    private static async Task<IWebElement> WaitForElementAsync(By by, WebDriverWait wait,
        CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            return wait.Until(drv =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return drv.FindElement(by);
            });
        }, cancellationToken);
    }

    private static async Task<ReadOnlyCollection<IWebElement>?> WaitForElementsAsync(
        By by,
        WebDriverWait wait,
        CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            return wait.Until(drv =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return drv.FindElements(by);
            });
        }, cancellationToken);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _driver.Quit();
        _driver.Dispose();
        _disposed = true;
    }

    private static class Selectors
    {
        public const string DrawTitle = ".title-bar .ng-binding";
        public const string AccumulatedValue = "div.next-prize.clearfix p.value.ng-binding";
        public const string DrawNumbers = "#ulDezenas li";
        public const string PreviousButton = "//a[contains(text(), '< Anterior')]";
    }

    private static class LogMessages
    {
        public static readonly EventId ScrapingStarted = new(100, "ScrapingStarted");
        public static readonly EventId ScrapingCompleted = new(101, "ScrapingCompleted");
        public static readonly EventId ScrapingError = new(102, "ScrapingError");
        public static readonly EventId DrawProcessed = new(103, "DrawProcessed");
    }

    private static class Guard
    {
        public static void AgainstNullOrEmpty(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty", paramName);
        }
    }
}