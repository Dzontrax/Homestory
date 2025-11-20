using System.ComponentModel.DataAnnotations;
using HomeStoryTest.Core;
using HomeStoryTest.Pages;
using HomeStoryTest.Validations;

namespace HomeStoryTest.Tests;

public class Tests : BaseTest
{
    [TestCase("Houston, TX")]
    public async Task Location_Search_Verification(string city)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);
        var assert = new Assertions(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city);       
        await assert.HeaderMatchesAsync(city); 
        await assert.AssertTileAddressesContainAsync(city);
    }

    [TestCase("Houston, TX", 100000, 1000000)]
    public async Task Set_Price_Filter_By_Typing_Amount(string city, int minPrice, int maxPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);
        var assert = new Assertions(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPriceByTyping(minPrice);
        await searchPage.SetMaxPriceByTyping(maxPrice);
        await assert.PricesInRangeAsync(minPrice, maxPrice);
    }

    [TestCase("Houston, TX", 100000, 300000)]
    public async Task Select_Price_Filter_From_Dropdown(string city, int minPrice, int maxPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);
        var assert = new Assertions(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPriceByMenuAsync(minPrice);
        await searchPage.SetMaxPriceByMenuAsync(maxPrice);
        await assert.PricesInRangeAsync(minPrice, maxPrice);
    }

    [TestCase("Houston, TX", 300000)]
    public async Task Select_Price_Up_To(string city, int maxPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);
        var assert = new Assertions(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMaxPriceByMenuAsync(maxPrice);
        await assert.PricesToAsync(maxPrice);
    }

    [TestCase("Houston, TX", 100000)]
    public async Task Select_Price_From(string city, int minPrice)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);
        var assert = new Assertions(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPriceByMenuAsync(minPrice);
        await assert.PricesFromAsync(minPrice);
    }

    [TestCase("Houston, TX", 300000)]
    public async Task Same_Price_Range_Should_Return_Exact_Amount(string city, int price)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);
        var assert = new Assertions(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPriceByMenuAsync(price);
        await searchPage.SetMaxPriceByMenuAsync(price);
        await assert.PricesExactAsync(price);
    }

    [TestCase("Houston, TX")]
    public async Task Out_Of_Range_Amounts_Should_Throw_No_result(string city)
    {
        var page = await CreatePageSessionAsync();
        var searchPage = new SearchPage(page);
        var assert = new Assertions(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city); 
        await searchPage.SetMinPriceByTyping(60000000);
        await searchPage.SetMaxPriceByTyping(60000000);
        await assert.NoResult();
    }
    
}
