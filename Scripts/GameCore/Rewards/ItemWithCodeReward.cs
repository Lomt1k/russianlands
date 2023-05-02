﻿using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Rewards;

[JsonObject]
public class ItemWithCodeReward : RewardBase
{
    public string itemCode { get; set; } = string.Empty;

    public override async Task<string?> AddReward(GameSession session)
    {
        try
        {
            var item = new InventoryItem(itemCode);
            var success = session.player.inventory.TryAddItem(item);
            return success ? item.GetFullName(session).Bold() : string.Empty;
        }
        catch (Exception ex)
        {
            await messageSender.SendErrorMessage(session.chatId, ex.Message);
            return null;
        }
    }

}
