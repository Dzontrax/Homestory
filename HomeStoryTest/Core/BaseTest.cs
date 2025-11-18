using Microsoft.Playwright;

namespace HomeStoryTest.Core;

public class BaseTest
{
    private IPlaywright _pw  = null!;
    protected IBrowser _browser = null!;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        _pw      = await Playwright.CreateAsync();
        _browser = await _pw.Chromium.LaunchAsync(new() 
        { 
            Headless = false,
            SlowMo   = 250
        });
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        await _browser.CloseAsync();
        _pw.Dispose();
    }

    protected async Task<IPage> CreatePageSessionAsync()
    {
        var ctx  = await _browser.NewContextAsync();
        return await ctx.NewPageAsync();
    }
}
