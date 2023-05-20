using System;
using MarkOne.Scripts.GameCore.Items.Generators.CodeGenerators;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Items.Generators;

/// <summary>
/// Основной класс, который управляет логикой генерации предметов
/// </summary>
public static class ItemGenerationManager
{
    private static readonly int _minItemType = (int)ItemType.Sword;
    private static readonly int _maxItemType = (int)ItemType.Scroll + 1;

    public static InventoryItem GenerateItem(byte _townhallLevel, Rarity _rarity, byte? grade = null)
    {
        var success = false;
        var itemType = ItemType.Any;
        while (!success)
        {
            itemType = (ItemType)new Random().Next(_minItemType, _maxItemType);
            success = _rarity != Rarity.Common || itemType.IsSupportCommonRarity();
        }
        return GenerateItemInternal(_townhallLevel, itemType, _rarity, grade);
    }

    /// <summary>
    /// Умная генерация предметов:
    /// - последние три выпавших типа предмета не будут повторяться
    /// </summary>
    public static InventoryItem GenerateItemWithSmartRandom(GameSession session, byte _townhallLevel, Rarity _rarity, byte? grade = null)
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

        return GenerateItemInternal(_townhallLevel, itemType, _rarity, grade);
    }

    public static InventoryItem GenerateItem(byte _townhallLevel, ItemType _type, Rarity _rarity, byte? grade = null)
    {
        return GenerateItemInternal(_townhallLevel, _type, _rarity, grade);
    }

    private static InventoryItem GenerateItemInternal(byte _townhallLevel, ItemType _type, Rarity _rarity, byte? grade = null)
    {
        var dataCode = _type switch
        {
            ItemType.Sword => new SwordCodeGenerator(_type, _rarity, _townhallLevel, grade).GetCode(),
            ItemType.Bow => new BowCodeGenerator(_type, _rarity, _townhallLevel, grade).GetCode(),
            ItemType.Stick => new StickCodeGenerator(_type, _rarity, _townhallLevel, grade).GetCode(),
            ItemType.Armor => new ArmorCodeGenerator(_type, _rarity, _townhallLevel, grade).GetCode(),
            ItemType.Boots => new ArmorCodeGenerator(_type, _rarity, _townhallLevel, grade).GetCode(),
            ItemType.Helmet => new ArmorCodeGenerator(_type, _rarity, _townhallLevel, grade).GetCode(),
            ItemType.Shield => new ShieldCodeGenerator(_type, _rarity, _townhallLevel, grade).GetCode(),
            ItemType.Amulet => new AmuletCodeGenerator(_type, _rarity, _townhallLevel, grade).GetCode(),
            ItemType.Ring => new RingCodeGenerator(_type, _rarity, _townhallLevel, grade).GetCode(),
            ItemType.Scroll => new ScrollCodeGenerator(_type, _rarity, _townhallLevel, grade).GetCode(),
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
