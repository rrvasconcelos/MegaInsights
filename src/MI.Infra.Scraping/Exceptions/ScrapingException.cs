namespace MI.Infra.Scraping.Exceptions;

public class ScrapingException : Exception
{
    public ScrapingException(string message) : base(message)
    {
    }

    public ScrapingException(string message, Exception inner) : base(message, inner)
    {
    }
}