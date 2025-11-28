using System;
using System.Data;

namespace HomeStoryTest.Contracts;

public interface ISearchActions
{
    Task GotoAsync();
    Task SearchCityAsync(string city);
    Task SetMinPriceByMenuAsync(int value);
    Task SetMaxPriceByMenuAsync(int value);
    Task SetMinPriceByTyping(int value);
    Task SetMaxPriceByTyping(int value);
}
