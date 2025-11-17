using System.Data;
using Microsoft.Playwright;
using Microsoft.VisualBasic.FileIO;

namespace HomeStoryTest.Pages;


public class SearchPage
{
    private readonly IPage _page;

    public SearchPage(IPage page)
    {
        _page = page;
    }
    
    private ILocator LocationSearchInput => _page.Locator("[data-qa='search-location-typeahead']");
    private ILocator Suggestion(string city) => _page.Locator($"[role='option'][aria-label='{city}']");
    private ILocator Suggestions => _page.Locator("[role='option']");
    private ILocator ResultsHeader => _page.Locator("h2.listings__homesForSale___dnyPH");

    public async Task GotoAsync()
    {
        await _page.GotoAsync("https://search.homestory.co/");
    }

    public async Task EnterCityPrefixAsync(string city, int prefixLength= 4)
    {
        var prefix = city.Substring(0, prefixLength);

        await _page.WaitForTimeoutAsync(7000);
        await LocationSearchInput.FillAsync(prefix);
        await Suggestions.First.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 10_000
        });
    }

    public async Task TypePrefixCharByCharAsync(string city, int prefixLength = 3)
{
    await LocationSearchInput.FillAsync(string.Empty); 

    string prefix = city.Substring(0, prefixLength);

    foreach (char c in prefix)
    {
        await _page.Keyboard.TypeAsync(c.ToString());
        await _page.WaitForTimeoutAsync(100);
    }

    await Suggestions.First.WaitForAsync(
        new() { State = WaitForSelectorState.Visible, Timeout = 10_000 });
}

    public async Task SelectCitySuggestionAsync(string city)
    {
        var option = Suggestion(city);

        await option.WaitForAsync(new()
        {
            State   = WaitForSelectorState.Visible,
            Timeout = 10_000
        });

        await option.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task SearchSuggestionAsync(string city)
    {
        await LocationSearchInput.FillAsync(city);
        var option = Suggestion(city);

        await option.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10_000});
        await option.ClickAsync();

           
    }

    public async Task<bool> AssertHeaderMatchingAsync(string expectedCity)
    {
        string header = (await ResultsHeader.InnerHTMLAsync()).Trim();
        return header.Equals($"Homes for sale in {expectedCity}");
    }

     
}
