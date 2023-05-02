﻿using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Services.Battles.Actions;

public class AddManaAction : IBattleAction
{
    public sbyte manaAmount;
    public HashSet<InventoryItem> fromItems = new HashSet<InventoryItem>();

    public void Add(InventoryItem _itemFrom, sbyte _manaAmount)
    {
        manaAmount += _manaAmount;
        fromItems.Add(_itemFrom);
    }

    public void ApplyActionWithMineStats(UnitStats stats)
    {
        stats.currentMana += manaAmount;
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
    }

    public string GetHeader(GameSession session)
    {
        return fromItems.Count switch
        {
            0 => Localization.Get(session, "battle_action_add_mana_header"),
            1 => fromItems.First().GetFullName(session).Bold(),
            _ => Localization.Get(session, "battle_action_multi_items_header", fromItems.Count)
        };
    }

    public string GetDescription(GameSession session)
    {
        return Localization.Get(session, "battle_action_add_mana_description", manaAmount);
    }

}
