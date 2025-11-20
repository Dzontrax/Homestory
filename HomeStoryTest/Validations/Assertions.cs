using System;
using HomeStoryTest.Helpers;
using Microsoft.Playwright;

namespace HomeStoryTest.Validations;


public class Assertions
{

    private readonly IPage _page;

    public Assertions(IPage page)
    {
        _page = page;
    }
    
    private ILocator ResultsHeader => _page.Locator("h2.listings__homesForSale___dnyPH");  
    private ILocator TileAddresses => _page.Locator(".listingItem__address___CKkGl");
    private ILocator PriceBlocks => _page.Locator("div[class^='listingItem__price___']");
    private ILocator NoResultsTitle => _page.Locator(".listings__noListings___CLDBd");
    
    public async Task PricesFromAsync(int min)
    {
        int total = await PriceBlocks.CountAsync();
        Assert.That(total, Is.GreaterThan(0), "No tiles with price tag.");

        for (int i = 0; i < total; i++)
        {
            int val = Utils.ParsePrice(await PriceBlocks.Nth(i).InnerTextAsync());
            Assert.That(val >= min, $"Tile #{i}: {val} < {min}");
        }
    }

    public async Task PricesToAsync(int max)
    {
        int total = await PriceBlocks.CountAsync();
        Assert.That(total, Is.GreaterThan(0), "No tiles with price tag.");

        for (int i = 0; i < total; i++)
        {
            int val = Utils.ParsePrice(await PriceBlocks.Nth(i).InnerTextAsync());
            Assert.That(val <= max, $"Tile #{i}: {val} > {max}");
        }
    }

    public async Task PricesInRangeAsync(int min, int max)
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

    public async Task PricesExactAsync(int exact)
    {
        int total = await PriceBlocks.CountAsync();
        Assert.That(total, Is.GreaterThan(0), "No tiles with price tag.");

        for (int i = 0; i < total; i++)
        {
            int val = Utils.ParsePrice(await PriceBlocks.Nth(i).InnerTextAsync());
            Assert.That(val == exact, $"Tile #{i}: {val} ≠ {exact}");
            
        }
    }

    public async Task HeaderMatchesAsync(string expectedCity, int timeoutMs = 30_000)
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

    public async Task NoResult()
    {
        var title = NoResultsTitle;
        await title.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        string actual = (await title.InnerTextAsync()).Trim();
        string expected = "There are no results for your search.";

        Assert.That(actual, Is.EqualTo(expected), $"Header mismatch – expected \"{expected}\", got \"{actual}\"");
    }
}
