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
    private ILocator TilesPanelTitle => _page.Locator("h2.listings__homesForSale___dnyPH");
    private ILocator PriceToggleBtn => _page.Locator("button.priceRange__toggleButton___smxgE");
    private ILocator MinPriceInput => _page.Locator("input[aria-label='Minimum Price']");
    private ILocator MaxPriceInput => _page.Locator("input[aria-label='Maximum Price']");
    private ILocator ListBox => _page.Locator("div[role='listbox']");
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

    public async Task SetMinPriceByTyping(int min)
    {
        await PriceToggleBtn.ClickAsync(); 
        await MinPriceInput.FillAsync(min.ToString());
        await _page.Keyboard.PressAsync("Tab");
        await _page.WaitForUpdateResultsAsync();
    }

    public async Task SetMaxPriceByTyping(int max)
    {
        await PriceToggleBtn.ClickAsync();
        await MaxPriceInput.FillAsync(max.ToString());
        await _page.Keyboard.PressAsync("Enter"); 
        await _page.WaitForUpdateResultsAsync();
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
    
}
        

