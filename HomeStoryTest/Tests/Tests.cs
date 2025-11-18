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

    [Test]
    public async Task Price_Filter_Functionality()
    {

        
    }
}
