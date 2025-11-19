using Microsoft.Playwright;
using HomeStoryTest.Helpers;


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
    private ILocator TilesPanelTitle => _page.Locator("h2.listings__homesForSale___dnyPH");
    private ILocator PriceToggleBtn => _page.Locator("button.priceRange__toggleButton___smxgE");
    private ILocator MinPriceInput => _page.Locator("input[aria-label='Minimum Price']");
    private ILocator MaxPriceInput => _page.Locator("input[aria-label='Maximum Price']");
    private ILocator ListingItemAddress => _page.Locator(".listingItem__address___CKkGl");
    private ILocator ListBox => _page.Locator("div[role='listbox']");
    private ILocator PriceBlocks => _page.Locator("div[class^='listingItem__price___']");
    private ILocator UpdateResultsSpinner => _page.Locator("div.mapboxMap__loaderBackground___gO7YV");
    private ILocator PriceOption(int value) {
        string formatted = value.ToString("#,0");
        return _page.Locator($"[role='option']:text-is('${formatted}')")
                    .Or(_page.Locator($"[role='option']:text-matches('{formatted}')"));
                    
    }
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

        await Suggestions.First.WaitForAsync(new() 
        { 
            State = WaitForSelectorState.Visible, 
            Timeout = 10_000 
        });
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
        await TilesPanelTitle.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 30_000 });
    }

    

    private async Task OpenPriceDropdownAndWaitAsync()
    {
        await PriceToggleBtn.ClickAsync();

        await MinPriceInput.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 10_000
        });
    }

    public async Task SetMinPriceByTyping(int min, bool useMenu = false)
    {
        await OpenPriceDropdownAndWaitAsync();     
        
        await MinPriceInput.FillAsync(min.ToString());       
        var opt = PriceOption(min);
        if (await opt.CountAsync() > 0)
            await opt.First.ClickAsync();
        else
            await _page.Keyboard.PressAsync("Enter");

        await PriceToggleBtn.WaitForAsync(new()
        {
            State   = WaitForSelectorState.Visible,
            Timeout = 30_000
        });

        await Utils.WaitForUpdateResultsAsync(_page);
    }

    public async Task SetMaxPriceByTyping(int max)
    {
        await OpenPriceDropdownAndWaitAsync();  

         await MaxPriceInput.FillAsync(max.ToString());
        var opt = PriceOption(max);

        if (await opt.CountAsync() > 0)
            await opt.First.ClickAsync();
        else
            await _page.Keyboard.PressAsync("Enter");

        await ListingItemAddress.First.WaitForAsync(
            new() { State = WaitForSelectorState.Visible, Timeout = 30_000 });

            await Utils.WaitForUpdateResultsAsync(_page);
    }
   
    public async Task SetMinPriceByMenuAsync(int value)
    {
        await PriceToggleBtn.ClickAsync();   
        await MinPriceInput.ClickAsync(); 
        await ListBox.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await PriceOption(value).First.ClickAsync(); 
        await PriceToggleBtn.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Utils.WaitForUpdateResultsAsync(_page); 
    }

    public async Task SetMaxPriceByMenuAsync(int amount)
    {
        await PriceToggleBtn.ClickAsync();  
        await MaxPriceInput.ClickAsync(); 
        await ListBox.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await PriceOption(amount).First.ClickAsync();
        await PriceToggleBtn.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Utils.WaitForUpdateResultsAsync(_page); 
    }

    public async Task AssertPricesFromAsync(int min)
    {
        int total = await PriceBlocks.CountAsync();
        Assert.That(total, Is.GreaterThan(0), "No tiles with price tag.");

        for (int i = 0; i < total; i++)
        {
            int val = Utils.ParsePrice(await PriceBlocks.Nth(i).InnerTextAsync());
            Assert.That(val >= min, $"Tile #{i}: {val} < {min}");
        }
    }

    public async Task AssertPricesToAsync(int max)
    {
        int total = await PriceBlocks.CountAsync();
        Assert.That(total, Is.GreaterThan(0), "No tiles with price tag.");

        for (int i = 0; i < total; i++)
        {
            int val = Utils.ParsePrice(await PriceBlocks.Nth(i).InnerTextAsync());
            Assert.That(val <= max, $"Tile #{i}: {val} > {max}");
        }
    }

    public async Task AssertPricesInRangeAsync(int min, int max)
    {
        int total = await PriceBlocks.CountAsync();
        Assert.That(total, Is.GreaterThan(0), "No tiles with price tag.");

        for (int i = 0; i < total; i++)
        {
            int val = Utils.ParsePrice(await PriceBlocks.Nth(i).InnerTextAsync());
            Assert.That(val >= min && val <= max,
                $"Tile #{i}: {val} not in range [{min} – {max}]");
        }
    }

    public async Task AssertPricesExactAsync(int exact)
    {
        int total = await PriceBlocks.CountAsync();
        Assert.That(total, Is.GreaterThan(0), "No tiles with price tag.");

        for (int i = 0; i < total; i++)
        {
            int val = Utils.ParsePrice(await PriceBlocks.Nth(i).InnerTextAsync());
            Assert.That(val == exact, $"Tile #{i}: {val} ≠ {exact}");
            
        }
    }

    public async Task AssertHeaderMatchesAsync(string expectedCity, int timeoutMs = 30_000)
    {
        await ResultsHeader.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = timeoutMs });

        string header    = (await ResultsHeader.InnerTextAsync()).Trim();
        string expected  = $"Homes for Sale in {expectedCity}";

        Assert.That(header, Is.EqualTo(expected), $"Header mismatch, expected: \"{expected}\" but actual: \"{header}\"");
    }

    public async Task AssertTileAddressesContainAsync(string cityState, int takeCount = int.MaxValue)
    {
        int total = await TileAddresses.CountAsync();
        Assert.That(total, Is.GreaterThan(0), "No list has been found.");

        int limit = Math.Min(total, takeCount);
        for (int i = 0; i < limit; i++)
        {
            string addr = (await TileAddresses.Nth(i).InnerTextAsync()).Trim();
            Assert.That(addr, Does.Contain(cityState).IgnoreCase, $"Tile #{i} doesn't contain expected city „{cityState}“.");
        }
    }
    
    }
        

