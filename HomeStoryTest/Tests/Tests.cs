using HomeStoryTest.Core;
using HomeStoryTest.Pages;

namespace HomeStoryTest.Tests;

public class Tests : BaseTest
{
    [TestCase("Houston, TX")]
    public async Task Location_Search_Verification(string city)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city);       
        await searchPage.AssertHeaderMatchesAsync(city); 
        await searchPage.AssertTileAddressesContainAsync(city);
    }

    [TestCase("Houston, TX")]
    public async Task Price_Filter_Functionality(string city)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPrice(1);
        await searchPage.SetMaxPrice(100000);
        await searchPage.AssertTilesWithinPriceAsync(100_000);
    
        // Assert that the value in the price rang e has been given the setted price
        // Change the price between 100000 and 400000$
        // Asert the tiles prices no more and no less 
        // Assert the value in the price filter to be between
        // Change the price to be max infinite
        // Assert that the price for tiles are higher than the minimal value setted
        // Write the price manually
        // Assert the values in the tiles.
    }
}
