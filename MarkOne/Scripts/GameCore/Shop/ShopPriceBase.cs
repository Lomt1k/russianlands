﻿using JsonKnownTypes;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
[JsonConverter(typeof(JsonKnownTypesConverter<ShopPriceBase>))]
public abstract class ShopPriceBase
{
    public abstract ShopPriceType priceType { get; }

    public abstract string GetCompactPriceView();
    public abstract string GetPlayerResourcesView(GameSession session);
    public abstract Task<bool> TryPurchase(GameSession session, ShopItemBase shopItem, Func<string, Task> onPurchaseError);
}
