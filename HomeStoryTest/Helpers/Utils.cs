using System;
using Microsoft.Playwright;

namespace HomeStoryTest.Helpers;

public static class Utils
{
public static async Task WaitForUpdateResultsAsync(this IPage page, int extraDelayMs = 1000)
    {
        await page.Locator("div.mapboxMap__loaderBackground___gO7YV")
                  .WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        await page.WaitForTimeoutAsync(extraDelayMs);
    }

    public static int ParsePrice(string raw)
    {
        string num = string.Concat(raw.Split('\n')[0]
                                    .Where(char.IsDigit));
        return int.Parse(num);
    }
}


