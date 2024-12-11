using MI.Infra.Data;
using MI.Scraper;
using MI.Scraper.Services;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MegaInsightsContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddTransient<ILotteryScraper, LotteryScraper>();

builder.Services.AddTransient<IWebDriver>(provider =>
{
    var options = new ChromeOptions();
    options.AddArgument("--headless");
    options.AddArgument("--no-sandbox");
    options.AddArgument("--disable-dev-shm-usage");
    return new ChromeDriver();
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();