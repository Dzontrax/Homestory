using HomeStoryTest.Contracts;
using HomeStoryTest.Pages;
using HomeStoryTest.Validations;
using Microsoft.Playwright;


namespace HomeStoryTest.Core;

public class BaseTest
{
    private IPlaywright _pw  = null!;
    protected IBrowser _browser = null!;

    protected IPage Page   = null!;
    protected ISearchActions  Search = null!;
    protected Checks Checks  = null!; 

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        _pw = await Playwright.CreateAsync();        
    }
    
    [SetUp]
    public async Task LaunchBrowser()
    {
        _browser = await _pw.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo   = 250
        });
        var ctx  = await _browser.NewContextAsync();
        Page     = await ctx.NewPageAsync();

        var sp   = new SearchPage(Page);
        Search   = sp;                   
        Checks    = new Checks(Page);   
    }

    [TearDown] 
    public async Task CloseBrowser()
    {
        if (_browser != null) 
            await _browser.CloseAsync();
    }

    [OneTimeTearDown]
    public void GlobalTeardown()
    {
        _pw.Dispose();
    }

    protected async Task<IPage> CreatePageSessionAsync()
    {
        var ctx = await _browser.NewContextAsync();
        
        return await ctx.NewPageAsync();
    }
}
