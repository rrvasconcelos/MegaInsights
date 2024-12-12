namespace MI.Infra.Scraping.Constants;

public static class Selectors
{
    public const string DrawTitle = ".title-bar .ng-binding";
    public const string AccumulatedValue = "div.next-prize.clearfix p.value.ng-binding";
    public const string DrawNumbers = "#ulDezenas li";
    public const string PreviousButton = "//a[contains(text(), '< Anterior')]";
}