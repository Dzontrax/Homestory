using System;
using Microsoft.Playwright;

namespace HomeStoryTest.Core;

public class BaseTest
{
    private IPlaywright _pw = null!;
    protected IBrowser _browser = null!;
    private IBrowserContext _ctx = null!;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        _pw = await Playwright.CreateAsync();

        _browser = await _pw.Chromium.LaunchAsync(new()
        {
            Headless = false
        });
        _ctx = await _browser.NewContextAsync();
    }

    [TearDown]      
    public async Task TearDown()
    {
        await _ctx.CloseAsync();   
        await _browser.CloseAsync();   
        _pw.Dispose();
    }
    protected async Task<IPage> CreatePageSessionAsync()
    {
        var ctx = await _browser.NewContextAsync();
        return await ctx.NewPageAsync();
    }

    
}
