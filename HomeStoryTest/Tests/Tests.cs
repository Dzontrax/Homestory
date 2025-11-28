using HomeStoryTest.Core;

namespace HomeStoryTest.Tests;

public class Tests : BaseTest
{
    [TestCase("Houston, TX")]
    public async Task Location_Search_Verification(string city)
    {       
        await Search.GotoAsync(); 
        await Search.SearchCityAsync("Houston, TX");
        await Assert.AssertTileAddressesContainAsync(city);
    }

    [TestCase("Houston, TX", 100000, 1000000)]
    public async Task Set_Price_Filter_By_Typing_Amount(string city, int minPrice, int maxPrice)
    {
        await Search.GotoAsync();
        await Search.SearchCityAsync("Houston, TX");
        await Search.SetMinPriceByTyping(minPrice);
        await Search.SetMaxPriceByTyping(maxPrice);
        await Assert.PricesInRangeAsync(minPrice, maxPrice);
    }

    [TestCase("Houston, TX", 100000, 300000)]
    public async Task Select_Price_Filter_From_Dropdown(string city, int minPrice, int maxPrice)
    {
        await Search.GotoAsync();
        await Search.SearchCityAsync("Houston, TX");
        await Search.SetMinPriceByMenuAsync(minPrice);
        await Search.SetMaxPriceByMenuAsync(maxPrice);
        await Assert.PricesInRangeAsync(minPrice, maxPrice);
    }

    [TestCase("Houston, TX", 300000)]
    public async Task Select_Price_Up_To(string city, int maxPrice)
    {
        await Search.GotoAsync();
        await Search.SearchCityAsync("Houston, TX");
        await Search.SetMaxPriceByMenuAsync(maxPrice);
        await Assert.PricesToAsync(maxPrice);
    }

    [TestCase("Houston, TX", 100000)]
    public async Task Select_Price_From(string city, int minPrice)
    {
        await Search.GotoAsync();
        await Search.SearchCityAsync("Houston, TX");
        await Search.SetMinPriceByMenuAsync(minPrice);
        await Assert.PricesFromAsync(minPrice);
    }

    [TestCase("Houston, TX", 300000)]
    public async Task Same_Price_Range_Should_Return_Exact_Amount(string city, int price)
    { 
        await Search.GotoAsync();
        await Search.SearchCityAsync("Houston, TX");
        await Search.SetMinPriceByMenuAsync(price);
        await Search.SetMaxPriceByMenuAsync(price);
        await Assert.PricesExactAsync(price);
    }

    [TestCase("Houston, TX")]
    public async Task Out_Of_Range_Amounts_Should_Throw_No_result(string city)
    {
        await Search.GotoAsync();
        await Search.SearchCityAsync("Houston, TX");
        await Search.SetMinPriceByTyping(600000000);
        await Search.SetMaxPriceByTyping(600000000);
        await Assert.NoResult();
    }
    
}
