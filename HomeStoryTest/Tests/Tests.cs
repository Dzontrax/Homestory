using HomeStoryTest.Core;
using HomeStoryTest.Pages;

namespace HomeStoryTest.Tests;

public class Tests : BaseTest
{
        [TestCase("Houston, TX")]
        public async Task Search_Results_Are_Displaying_Correct_Location(string city)
        {
            var page = await CreatePageSessionAsync();

        var searchPage = new SearchPage(page);

        await searchPage.GotoAsync();
        await searchPage.TypePrefixCharByCharAsync("Houston, TX", 3);
        await searchPage.SelectCitySuggestionAsync(city);
        Assert.Equals(await searchPage.AssertHeaderMatchingAsync(city), "Selected city is not displaye in headline location.");
        
        }
}
