using System.Collections.ObjectModel;
using MI.Domain.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MI.Scraper.Services;

public interface ILotteryScraper
{
    Task<IEnumerable<LotteryResult>> GetLotteryResultsAsync();
}

public class LotteryScraper(ILogger<LotteryScraper> logger, IWebDriver driver) : ILotteryScraper
{
    public async Task<IEnumerable<LotteryResult>> GetLotteryResultsAsync()
    {
        try
        {
            await driver.Navigate().GoToUrlAsync("https://loterias.caixa.gov.br/Paginas/Mega-Sena.aspx");
            
            logger.LogInformation("Título da Página: {Title}", driver.Title);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            var results = new List<LotteryResult>();

            await ScrapeLotteryDataAsync(wait, results);
            return results;
        }
        catch (WebDriverTimeoutException ex)
        {
            logger.LogError(ex, "Tempo esgotado esperando pela atualização da página.");
            return [];
        }
        catch (NoSuchElementException ex)
        {
            logger.LogError(ex, "Elemento não encontrado. Verifique o seletor.");
            return [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro inesperado durante o scraping.");
            return [];
        }
    }

    private async Task ScrapeLotteryDataAsync(WebDriverWait wait, List<LotteryResult> results)
    {
        try
        {
            var count = 0;
            while (count < 200)
            {
                var concursoElement = await WaitForElementAsync(By.CssSelector(".title-bar .ng-binding"), wait);

                var previousText = concursoElement.Text;
                logger.LogInformation("Concurso: {Text}", previousText);

                var dataContest = GetContest(concursoElement);
                var numeros = await CaptureNumbers(wait);
                LogConcurso(dataContest.contest, numeros);

                var accumulated = GetAccumulatedValue();

                results.Add(new LotteryResult(dataContest.dateContest, dataContest.contest, accumulated, numeros));

                NavigateToPreviousPage();

                await AwaitForNextElementAsync(wait, previousText);

                count++;
            }
        }
        catch (NoSuchElementException ex)
        {
            logger.LogWarning(ex, "Erro ao tentar capturar dados do concurso.");
        }
    }

    private static async Task AwaitForNextElementAsync(WebDriverWait wait, string previousText)
    {
        await Task.Run(() =>
        {
            wait.Until(d =>
            {
                var updatedConcursoElement = d.FindElement(By.CssSelector(".title-bar .ng-binding"));
                return updatedConcursoElement.Text != previousText;
            });
        });
    }

    private decimal GetAccumulatedValue()
    {
        try
        {
            var accumulated = driver.FindElement(By.CssSelector("div.next-prize.clearfix p.value.ng-binding"));
            var valueText = accumulated.Text.Replace("R$", "").Trim();

            if (decimal.TryParse(valueText, out var value)) return value;

            logger.LogWarning("Não foi possível converter o valor acumulado: {ValueText}", valueText);
            
            return 0;
        }
        catch (NoSuchElementException)
        {
            logger.LogInformation("Elemento não encontrado. Verifique o seletor.");
            return 0;
        }
    }

    private (int contest, DateOnly dateContest) GetContest(IWebElement? element)
    {
        if (element is null || string.IsNullOrWhiteSpace(element.Text))
        {
            return default;
        }

        var data = element.Text.Split(" ");
        if (data.Length < 3) return default;

        var dateString = data[2].Trim('(', ')');
        if (!DateOnly.TryParse(dateString, out var dateContest))
        {
            logger.LogError("Falha ao converter a data: {DateString}", dateString);
            return default;
        }

        if (int.TryParse(data[1], out var contest))
            return (contest, dateContest);

        logger.LogError("Falha ao converter o concurso: {ConcursoString}", data[1]);
        return default;
    }

    private async Task<IReadOnlyList<int>> CaptureNumbers(WebDriverWait wait)
    {
        try
        {
            var dezenasElementos = await WaitForElementsAsync(By.CssSelector("#ulDezenas li"), wait);

            return dezenasElementos?.Select(dezena
                => Convert.ToInt32(dezena.Text)
            ).ToList() ?? [];
        }
        catch (NoSuchElementException ex)
        {
            logger.LogWarning(ex, "Números do resultado não encontrados.");
            return [];
        }
    }

    private void NavigateToPreviousPage()
    {
        var buttonElement = driver.FindElement(By.XPath("//a[contains(text(), '< Anterior')]"));

        buttonElement.Click();
    }

    private void LogConcurso(int concurso, IEnumerable<int> numeros)
    {
        logger.LogInformation("Concurso: {Concurso} - Números: {Numeros}", concurso, string.Join(", ", numeros));
    }

    private static async Task<IWebElement> WaitForElementAsync(By by, WebDriverWait wait)
    {
        return await Task.Run(() => { return wait.Until(drv => drv.FindElement(by)); });
    }

    private static async Task<ReadOnlyCollection<IWebElement>?> WaitForElementsAsync(By by, WebDriverWait wait)
    {
        return await Task.Run(() => { return wait.Until(drv => drv.FindElements(by)); });
    }
}