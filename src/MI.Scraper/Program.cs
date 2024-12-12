using MI.Infra.Data;
using MI.Infra.Scraping.Extensions;
using MI.Scraper;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MegaInsightsContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddLotteryScraper(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();