using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Polly;

namespace MI.Infra.Scraping.Factories;

public interface IPolicyFactory
{
    IAsyncPolicy<IWebElement> CreateElementPolicy(int retryAttempts);
    IAsyncPolicy<ReadOnlyCollection<IWebElement>> CreateElementsPolicy(int retryAttempts);
}

public sealed class PolicyFactory(ILogger<PolicyFactory> logger):IPolicyFactory
{
    public IAsyncPolicy<IWebElement> CreateElementPolicy( int retryAttempts)
    {
        return Policy<IWebElement>
            .Handle<WebDriverException>()
            .Or<NoSuchElementException>()
            .WaitAndRetryAsync(
                retryAttempts,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (result, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(
                        result.Exception,
                        "Attempt {RetryCount} to find element failed. Waiting {Duration} seconds before next attempt",
                        retryCount,
                        timeSpan.TotalSeconds);
                });
    }

    public IAsyncPolicy<ReadOnlyCollection<IWebElement>> CreateElementsPolicy( int retryAttempts)
    {
        return Policy<ReadOnlyCollection<IWebElement>>
            .Handle<WebDriverException>()
            .Or<NoSuchElementException>()
            .WaitAndRetryAsync(
                retryAttempts,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (result, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(
                        result.Exception,
                        "Attempt {RetryCount} to find elements failed. Waiting {Duration} seconds before next attempt",
                        retryCount,
                        timeSpan.TotalSeconds);
                });
    }
}