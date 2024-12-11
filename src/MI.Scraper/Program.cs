using MI.Infra.Data;
using MI.Scraper;
using MI.Scraper.Configuration;
using MI.Scraper.Factory;
using MI.Scraper.Services;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.Configure<LotteryScraperOptions>(builder.Configuration.GetSection("LotteryScraper"));

builder.Services.AddDbContext<MegaInsightsContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<ILotteryScraper, LotteryScraper>();
builder.Services.AddTransient<IPolicyFactory, PolicyFactory>();
builder.Services.AddTransient<IWebDriver>(provider => new ChromeDriver());

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();