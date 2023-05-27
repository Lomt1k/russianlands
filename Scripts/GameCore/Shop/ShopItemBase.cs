﻿using JsonKnownTypes;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
[JsonConverter(typeof(JsonKnownTypesConverter<ShopItemBase>))]
public abstract class ShopItemBase
{
    protected static readonly NotificationsManager notificationsManager = ServiceLocator.Get<NotificationsManager>();

    public string vendorCode { get; set; } = string.Empty;
    public ShopPriceBase? price { get; set; }

    public string GetPriceButtonView(GameSession session)
    {
        return price is null
        ? Localization.Get(session, "resource_price_free")
        : Localization.Get(session, "menu_item_buy_button", price.GetCompactPriceView());
    }

    public string GetNameForList(GameSession session)
    {
        var priceView = price?.GetCompactPriceView() ?? Emojis.ElementWarningRed.ToString();
        return string.Format("{0,-30}{1,20}", GetTitle(session).RemoveHtmlTags(), priceView);
    }

    public async Task TryPurchase(GameSession session, Func<Task> onSuccess, Func<string,Task> onPurchaseError)
    {
        var playerResources = session.player.resources;
        var freeSlots = playerResources.GetResourceLimit(ResourceId.InventoryItems) - playerResources.GetValue(ResourceId.InventoryItems);
        var inventoryItemsCount = GetRewards().GetInventoryItemsCount();
        if (inventoryItemsCount > freeSlots)
        {
            var error = Localization.Get(session, "dialog_shop_inventory_slots_required", inventoryItemsCount);
            await onPurchaseError(error).FastAwait();
            return;
        }

        var success = price == null ? true : await price.TryPurchase(session, onPurchaseError).FastAwait();
        if (success)
        {
            await GiveAndShowRewards(session, onSuccess).FastAwait();
        }
    }

    private async Task GiveAndShowRewards(GameSession session, Func<Task> onContinue)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "dialog_shop_purchased_items_header"));
        foreach (var reward in GetRewards())
        {
            var addedReward = await reward.AddReward(session).FastAwait();
            if (!string.IsNullOrEmpty(addedReward))
            {
                sb.AppendLine(addedReward);
            }
        }
        await notificationsManager.ShowNotification(session, sb, onContinue).FastAwait();
    }

    public abstract string GetMessageText(GameSession session);
    public abstract string GetTitle(GameSession session);
    protected abstract IEnumerable<RewardBase> GetRewards();
    
}
