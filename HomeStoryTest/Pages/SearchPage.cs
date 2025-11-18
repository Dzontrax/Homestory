using Microsoft.Playwright;

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
    private ILocator TileAddresses => _page.Locator(".listingItem__address___CKkGl");


    public async Task GotoAsync()
    {
        await _page.GotoAsync("https://search.homestory.co/");
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
        await _page.Locator("h2.listings__homesForSale___dnyPH")
                            .WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 30_000 });
    }

    public async Task AssertHeaderMatchesAsync(string expectedCity, int timeoutMs = 30_000)
    {
        await ResultsHeader.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = timeoutMs });

        string header    = (await ResultsHeader.InnerTextAsync()).Trim();
        string expected  = $"Homes for Sale in {expectedCity}";

        Assert.That(header, Is.EqualTo(expected), $"Header mismatch, expected: \"{expected}\" but actual: \"{header}\"");
    }

    public async Task AssertTileAddressesContainAsync(string cityState,
                                                  int takeCount = int.MaxValue)
{
    int total = await TileAddresses.CountAsync();
    Assert.That(total, Is.GreaterThan(0), "Nijedan listing tile nije pronađen.");

    int limit = Math.Min(total, takeCount);
    for (int i = 0; i < limit; i++)
    {
        string addr = (await TileAddresses.Nth(i).InnerTextAsync()).Trim();
        TestContext.Progress.WriteLine($"[DEBUG] Tile {i} → \"{addr}\"");

        Assert.That(addr, Does.Contain(cityState).IgnoreCase, $"Tile #{i} ne sadrži očekivani grad „{cityState}“.");
    }
}

     
}
