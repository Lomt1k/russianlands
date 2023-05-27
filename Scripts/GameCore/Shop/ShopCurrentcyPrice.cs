﻿using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopCurrentcyPrice : ShopPriceBase
{
    public override ShopPriceType priceType => ShopPriceType.CurrencyPrice;

    public uint russianRubles { get; set; }

    public override string GetCompactPriceView()
    {
        throw new NotImplementedException();
    }

    public override string GetPlayerResourcesView(GameSession session)
    {
        throw new NotImplementedException();
    }

    public override Task<bool> TryPurchase(GameSession session)
    {
        throw new NotImplementedException();
    }
}
