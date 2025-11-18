using System.ComponentModel.DataAnnotations;
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

    [TestCase("Houston, TX", 1, 100000)]
    public async Task Set_Price_Filter_By_Typing_Amount(string city, int minPrice, int maxPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPrice(minPrice);
        await searchPage.SetMaxPrice(maxPrice);
        await searchPage.AssertTilesWithinPriceAsync(minPrice, maxPrice);
    }

    [TestCase("Houston, TX", 100000, 300000)]
    public async Task Select_Price_Filter_From_Dropdown(string city, int minPrice, int maxPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPriceByMenuAsync(minPrice);
        await searchPage.SetMaxPriceByMenuAsync(maxPrice);
        await searchPage.AssertTilesWithinPriceAsync(minPrice, maxPrice);
    }

    [TestCase("Houston, TX", 100000, 300000)]
    public async Task Select_Price_Up_To(string city, int minPrice, int maxPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMaxPriceByMenuAsync(maxPrice);
        await searchPage.AssertTilesWithinPriceAsync(minPrice, maxPrice);
    }

    [TestCase("Houston, TX", 100000, 300000)]
    public async Task Select_Price_From(string city, int minPrice, int maxPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPriceByMenuAsync(minPrice);
        await searchPage.AssertTilesWithinPriceAsync(minPrice, maxPrice);
    }

    //TO DO: jos jedan assert kada si min i mac=x JEDNAKI -> trazi se cena za TACAN Iznos, a ne range

    
}
