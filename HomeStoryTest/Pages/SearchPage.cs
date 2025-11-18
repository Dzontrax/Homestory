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
    private ILocator PriceToggleBtn => _page.Locator("button.priceRange__toggleButton___smxgE");
    private ILocator MinPriceInput => _page.Locator("input[aria-label='Minimum Price']");
    private ILocator MaxPriceInput => _page.Locator("input[aria-label='Maximum Price']");
    private ILocator ListingTile => _page.Locator(".listing-tile .price");
    private ILocator ListingItemAddress => _page.Locator(".listingItem__address___CKkGl");
    private ILocator PriceToggleValue => _page.Locator("button.priceRange__toggleButton___smxgE span.priceRange__value___c4VbX");     
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

    public async Task AssertTileAddressesContainAsync(string cityState, int takeCount = int.MaxValue)
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

    private async Task OpenPriceDropdownAndWaitAsync()
    {
        await PriceToggleBtn.ClickAsync();

        await MinPriceInput.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 10_000
        });
    }

    public async Task SetMinPrice(int min, bool useMenu = false)
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
    }

    public async Task SetMaxPrice(int max)
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
    }
   
    public async Task SetMinPriceByMenuAsync(int amount)
    {
        await PriceToggleBtn.ClickAsync();   
        await MinPriceInput.ClickAsync(); 
        await _page.Locator("div[role='listbox']")
                .WaitForAsync(new() { State = WaitForSelectorState.Visible });

        await PriceOption(amount).First.ClickAsync(); 
        await PriceToggleBtn.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task SetMaxPriceByMenuAsync(int amount)
    {
        await PriceToggleBtn.ClickAsync();  
        await MaxPriceInput.ClickAsync(); 
        await _page.Locator("div[role='listbox']")
                .WaitForAsync(new() { State = WaitForSelectorState.Visible });

        await PriceOption(amount).First.ClickAsync();
        await PriceToggleBtn.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task AssertTilesWithinPriceAsync(int minPrice, int maxPrice)
    {
    var priceLocs = ListingTile;
    int n = await priceLocs.CountAsync();

    for (int i = 0; i < n; i++)
    {
        string raw = (await priceLocs.Nth(i).InnerTextAsync()).Trim();
        if (int.TryParse(raw.Replace("$", "").Replace(",", ""), out int value))
        {
            Assert.That(value, Is.InRange(minPrice, maxPrice),
                $"Tile #{i} ({raw}) is not in range [{minPrice}-{maxPrice}]");
        }
        else
        {
            Assert.Fail($"Could not parse price for tile #{i}: {raw}");
        }
    }
    }

    public async Task AssertPriceAboveMinAsync(int minPrice)
    {
        var priceLocs = ListingTile;
        var rawPrices = await priceLocs.AllInnerTextsAsync(); 

        foreach (var raw in rawPrices)
        {
            if (int.TryParse(raw.Replace("$", "").Replace(",", ""), out int value))
            {
                Assert.That(value, Is.GreaterThanOrEqualTo(minPrice),
                    $"Pločica ({raw}) nije iznad minimalne cene ({minPrice}) kada je maksimum 'No max'.");
            }
            else
            {
                
            }
        }
    }
    }
        

