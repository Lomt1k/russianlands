using System;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Items.Generators.CodeGenerators;

namespace MarkOne.Scripts.GameCore.Items.Generators;

/// <summary>
/// Основной класс, который управляет логикой генерации предметов
/// </summary>
public static class ItemGenerationManager
{
    private static readonly int _minItemType = (int)ItemType.Sword;
    private static readonly int _maxItemType = (int)ItemType.Scroll + 1;

    public static InventoryItem GenerateItem(int _townhallLevel, Rarity _rarity)
    {
        var success = false;
        var itemType = ItemType.Any;
        while (!success)
        {
            itemType = (ItemType)new Random().Next(_minItemType, _maxItemType);
            success = _rarity != Rarity.Common || itemType.IsSupportCommonRarity();
        }
        return GenerateItemInternal(_townhallLevel, itemType, _rarity);
    }

    /// <summary>
    /// Умная генерация предметов:
    /// - последние три выпавших типа предмета не будут повторяться
    /// </summary>
    public static InventoryItem GenerateItemWithSmartRandom(GameSession session, int _townhallLevel, Rarity _rarity)
    {
        var lastGeneratedItems = session.profile.dynamicData.lastGeneratedItemTypes;
        var success = false;
        var itemType = ItemType.Any;
        while (!success)
        {
            itemType = (ItemType)new Random().Next(_minItemType, _maxItemType);
            if (lastGeneratedItems.Contains(itemType))
                continue;

            success = _rarity != Rarity.Common || itemType.IsSupportCommonRarity();
        }

        lastGeneratedItems.Add(itemType);
        if (lastGeneratedItems.Count > 3)
        {
            lastGeneratedItems.RemoveAt(0);
        }

        return GenerateItemInternal(_townhallLevel, itemType, _rarity);
    }

    public static InventoryItem GenerateItem(int _townhallLevel, ItemType _type, Rarity _rarity)
    {
        return GenerateItemInternal(_townhallLevel, _type, _rarity);
    }

    private static InventoryItem GenerateItemInternal(int _townhallLevel, ItemType _type, Rarity _rarity)
    {
        var dataCode = _type switch
        {
            ItemType.Sword => new SwordCodeGenerator(_type, _rarity, _townhallLevel).GetCode(),
            ItemType.Bow => new BowCodeGenerator(_type, _rarity, _townhallLevel).GetCode(),
            ItemType.Stick => new StickCodeGenerator(_type, _rarity, _townhallLevel).GetCode(),
            ItemType.Armor => new ArmorCodeGenerator(_type, _rarity, _townhallLevel).GetCode(),
            ItemType.Boots => new ArmorCodeGenerator(_type, _rarity, _townhallLevel).GetCode(),
            ItemType.Helmet => new ArmorCodeGenerator(_type, _rarity, _townhallLevel).GetCode(),
            ItemType.Shield => new ShieldCodeGenerator(_type, _rarity, _townhallLevel).GetCode(),
            ItemType.Amulet => new AmuletCodeGenerator(_type, _rarity, _townhallLevel).GetCode(),
            ItemType.Ring => new RingCodeGenerator(_type, _rarity, _townhallLevel).GetCode(),
            ItemType.Scroll => new ScrollCodeGenerator(_type, _rarity, _townhallLevel).GetCode(),
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(dataCode))
        {
            Program.logger.Error($"ItemGenerationManager: Can not create item with type '{_type}'. Creating a sword...");
            dataCode = new SwordCodeGenerator(ItemType.Sword, _rarity, _townhallLevel).GetCode();
        }

        return new InventoryItem(dataCode);
    }


}
