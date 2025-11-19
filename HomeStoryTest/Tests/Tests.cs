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

    [TestCase("Houston, TX", 100000, 1000000)]
    public async Task Set_Price_Filter_By_Typing_Amount(string city, int minPrice, int maxPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPriceByTyping(minPrice);
        await searchPage.SetMaxPriceByTyping(maxPrice);
        await searchPage.AssertPricesInRangeAsync(minPrice, maxPrice);
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
        await searchPage.AssertPricesInRangeAsync(minPrice, maxPrice);
    }

    [TestCase("Houston, TX", 300000)]
    public async Task Select_Price_Up_To(string city, int maxPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMaxPriceByMenuAsync(maxPrice);
        await searchPage.AssertPricesToAsync(maxPrice);
    }

    [TestCase("Houston, TX", 100000)]
    public async Task Select_Price_From(string city, int minPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPriceByMenuAsync(minPrice);
        await searchPage.AssertPricesFromAsync(minPrice);
    }

    [TestCase("Houston, TX", 300000)]
    public async Task Same_Price_Range_Should_Return_Exact_Amount(string city, int price)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPriceByMenuAsync(price);
        await searchPage.SetMaxPriceByMenuAsync(price);
        await searchPage.AssertPricesExactAsync(price);
    }
    
}
