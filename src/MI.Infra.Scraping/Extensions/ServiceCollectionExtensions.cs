using MI.Infra.Scraping.Configuration;
using MI.Infra.Scraping.Factories;
using MI.Infra.Scraping.Interfaces;
using MI.Infra.Scraping.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace MI.Infra.Scraping.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLotteryScraper(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LotteryScraperOptions>(options => 
            configuration.GetSection("LotteryScraper")
                .Bind(options));

        services.AddTransient<ILotteryScraper, LotteryScraper>();
        services.AddTransient<IPolicyFactory, PolicyFactory>();
        services.AddTransient<IWebDriver>(provider => new ChromeDriver());

        return services;
    }
}
