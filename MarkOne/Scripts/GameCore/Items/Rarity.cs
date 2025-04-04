﻿using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.Utils;
using System.Collections.ObjectModel;

namespace MarkOne.Scripts.GameCore.Items;

public enum Rarity : byte
{
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3
}

public class WeightedRarity : IWeightedItem
{
    public int weight { get; init; }
    public Rarity rarity { get; init; }

    public static ObservableCollection<Rarity> allRarities { get; } = new ObservableCollection<Rarity>(System.Enum.GetValues<Rarity>());
}

public static class RarityExtensions
{
    public static string GetView(this Rarity rarity, GameSession session)
    {
        return Localization.Get(session, $"item_rarity_{rarity.ToString().ToLower()}");
    }

    public static string GetUnknownItemView(this Rarity rarity, GameSession session, ItemType itemType = ItemType.Any, int itemsCount = 1)
    {
        var localizationKey = itemType == ItemType.Any
            ? $"unknown_item_view_rarity_{rarity.ToString().ToLower()}"
            : $"unknown_item_view_rarity_{rarity.ToString().ToLower()}_type_{itemType.ToString().ToLower()}";
        var emoji = itemType == ItemType.Any ? Emojis.ItemUnknown : itemType.GetEmoji();

        return itemsCount == 1
            ? emoji + Localization.Get(session, localizationKey).Bold()
            : emoji + (Localization.Get(session, localizationKey) + ':').Bold() + $" {itemsCount}";
    }

    public static int GetKeywordsCount(this Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Epic => 1,
            Rarity.Legendary => 2,
            _ => 0
        };
    }

}
